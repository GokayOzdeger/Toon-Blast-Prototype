using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridController
{
    private readonly float collumnSpacing = 79;
    private readonly float rowSpacing = 70;

    private static Vector2Int[] _surroundingCoordinateMatrises = new Vector2Int[]
    {
        new Vector2Int( 0,-1),
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
    };

    
    private CanvasScaler _canvasScaler;
    private RectTransform _gridCenterTransform;
    
    private uint gridRowCount;
    private uint gridCollumnCount;

    public int RowCount { get { return (int)gridRowCount; } }
    public int CollumnCount { get { return (int)gridCollumnCount; } }
    public float CollumnSpacing { get { return collumnSpacing * _canvasScaler.transform.localScale.x; } }
    public float RowSpacing { get { return rowSpacing * _canvasScaler.transform.localScale.y; } }

    public UnityEvent<Vector2Int> OnGridChange = new UnityEvent<Vector2Int>();
    public Vector2[,] GridPositions { get; private set; }
    public IGridEntity[,] EntityGrid { get; private set; }
    public bool GridInterractable { get; private set; } = true;

    private List<Vector2Int> _cachedGridChanges = new List<Vector2Int>();
    private bool[,] controlledGridCoordinates;
    protected int _entitiesInProcess = 0; 

    private GridEntitySpawner _entitySpawner;

    public GridController(GridControllerSettings settings , GridControllerSceneReferences references)
    {
        this._canvasScaler = references.CanvasScaler;
        this._gridCenterTransform = references.GridCenterTransform;
        gridRowCount = settings.RowCount;
        gridCollumnCount = settings.CollumnCount;
        CreateGridAndCalculatePositions();
        CreateSystemComponents(settings, references);
        UpdateAllEntities();
    }

    private void CreateSystemComponents(GridControllerSettings settings, GridControllerSceneReferences references)
    {
        _entitySpawner = new GridEntitySpawner(this, settings.GridEntitySpawnerSettings, references.GridEntitySpawnerSceneReferences);
    }

    public void RegisterGridEntityToPosition(IGridEntity entity, int collumnIndex, int rowIndex)
    {
        Debug.Log($"Register Entity To: {rowIndex}, {collumnIndex}");
        EntityGrid[collumnIndex, rowIndex] = entity;
        OnGridChange.AddListener(entity.OnGridChange);
    }

    public void OnGridEventStart(IGridEvent gridEvent)
    {
        if (!gridEvent.MakeGridUninterractableOnStart) return;
        GridInterractable = false;
    }

    public void OnGridEventEnd(IGridEvent gridEvent)
    {
        if (!gridEvent.ProceedGridAfterEventEnds) return;
        CallCachedChanges();
        _entitySpawner.SummonRequestedEntities();
    }

    public void RemoveEntitiesFromGridArray<T>(List<T> entitiesToRemove) where T: IGridEntity
    {
        foreach (T entityToRemove in entitiesToRemove) OnGridChange.RemoveListener(entityToRemove.OnGridChange);
        foreach (T entityToRemove in entitiesToRemove)
        {
            EntityGrid[entityToRemove.GridCoordinates.x, entityToRemove.GridCoordinates.y] = null;
            CacheGridChange(entityToRemove.GridCoordinates);
        }
    }

    

    public void WriteEntityFall(IGridEntity gridEntity)
    {
        gridEntity.EntityNeedsUpdate = true;
        int collumnIndex = gridEntity.GridCoordinates.y;
        int coordinateToFallTo = gridEntity.GridCoordinates.x;
        for (int i = gridEntity.GridCoordinates.x-1; i >= 0; i--)
        {
            if (EntityGrid[i, collumnIndex] == null) coordinateToFallTo = i;
            else break;
        }
        WriteEntityMovementToGrid(new Vector2Int(coordinateToFallTo, collumnIndex), gridEntity);
    }

    public void EntityStartProcess()
    {
        _entitiesInProcess++;
    }

    public void EntityEndProcess()
    {
        _entitiesInProcess--;
        if (_entitiesInProcess == 0)
        {
            GridInterractable = true;
        }
    }

    private void WriteEntityMovementToGrid(Vector2Int newCoordinates, IGridEntity entity)
    {
        Vector2Int oldEntityCoordinates = entity.GridCoordinates;
        EntityGrid[entity.GridCoordinates.x, entity.GridCoordinates.y] = null;
        EntityGrid[newCoordinates.x, newCoordinates.y] = entity;
        entity.OnMoveEntity(newCoordinates);
        CacheGridChange(newCoordinates);
        CacheGridChange(oldEntityCoordinates);
    }

    public void CallCachedChanges()
    {
        while (_cachedGridChanges.Count > 0)
        {
            OnGridChange.Invoke(_cachedGridChanges[0]);
            _cachedGridChanges.RemoveAt(0);
        }
        UpdateAllEntities();
    }

    public void CallEntitySpawn(int collumnIndex)
    {
        _entitySpawner.AddEntitySpawnReqeust(collumnIndex);
    }

    private void CacheGridChange(Vector2Int changeCords)
    {
        _cachedGridChanges.Add(changeCords);
    }

    private void UpdateAllEntities()
    {
        controlledGridCoordinates = new bool[gridRowCount, gridCollumnCount];
        foreach (IGridEntity entity in EntityGrid)
        {
            if (entity == null || !entity.EntityNeedsUpdate) continue;
            entity.OnUpdateEntity();
        }
    }

    private void CreateGridAndCalculatePositions()
    {
        EntityGrid = new IGridEntity[gridRowCount, gridCollumnCount];
        GridPositions = new Vector2[gridRowCount, gridCollumnCount];
        
        // calculate bottom left corner entity position 
        float bottomLeftCornerX = _gridCenterTransform.position.x - ((gridCollumnCount / 2f) - .5f) * CollumnSpacing;
        float bottomLeftCornerY = _gridCenterTransform.position.y - ((gridRowCount / 2f) - .5f) * RowSpacing;

        Vector2 bottomLeftCorner = new Vector2(bottomLeftCornerX, bottomLeftCornerY);
        Vector2 cursorPoint = bottomLeftCorner;
        for (int i = 0; i < GridPositions.GetLength(0); i++)
        {
            for (int j = 0; j < GridPositions.GetLength(1); j++)
            {
                GridPositions[i, j] = cursorPoint;
                cursorPoint.x += CollumnSpacing;
            }
            cursorPoint.y += RowSpacing;
            cursorPoint.x = bottomLeftCorner.x;
        }
    }

    public void CollectMatchingSurroundingEntities<T>(T entity, ref List<T> entityListToCollect) where T : IGridEntity
    {
        entityListToCollect.Add(entity);
        controlledGridCoordinates[entity.GridCoordinates.x, entity.GridCoordinates.y] = true;
        CollectMatchingSurroundingEntitiesRecursive(entity, ref entityListToCollect);
    }

    private void CollectMatchingSurroundingEntitiesRecursive<T>(T entity, ref List<T> entityListToCollect) where T : IGridEntity
    {
        foreach (Vector2Int surroundingCoordinateAdd in _surroundingCoordinateMatrises)
        {
            Vector2Int surroundingCoordinate = entity.GridCoordinates + surroundingCoordinateAdd;
            // Skip Surrounding check position if it is out of range or already controlled
            if (surroundingCoordinate.x < 0 || surroundingCoordinate.x == gridRowCount) continue;
            if (surroundingCoordinate.y < 0 || surroundingCoordinate.y == gridCollumnCount) continue;
            if (controlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y]) continue;

            IGridEntity surroundingMatchingEntity = EntityGrid[surroundingCoordinate.x, surroundingCoordinate.y];
            // skip entity if type doesnt match
            if (surroundingMatchingEntity == null || surroundingMatchingEntity.EntityType != entity.EntityType) continue;

            // mark position as controlled to prevent controlling same entity more than once
            controlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y] = true;
            T castEntity = (T)surroundingMatchingEntity;
            entityListToCollect.Add(castEntity);
            CollectMatchingSurroundingEntitiesRecursive(castEntity, ref entityListToCollect);
        }
    }

    [System.Serializable]
    public class GridControllerSceneReferences
    {
        [BHeader("Grid Controller References")] 
        [SerializeField] private RectTransform gridCenterTransform;
        [SerializeField] private CanvasScaler canvasScaler;
        [Group]
        [SerializeField] private GridEntitySpawner.GridEntitySpawnerSceneReferences gridEntitySpawnerSceneReferences;
        public RectTransform GridCenterTransform => gridCenterTransform;
        public CanvasScaler CanvasScaler => canvasScaler;
        public GridEntitySpawner.GridEntitySpawnerSceneReferences GridEntitySpawnerSceneReferences => gridEntitySpawnerSceneReferences;
    }

    [System.Serializable]
    public class GridControllerSettings
    {
        [BHeader("Grid Controller Settings")]
        [SerializeField] private uint rowCount;
        [SerializeField] private uint collumnCount;
        [Group]
        [SerializeField] private GridEntitySpawner.GridEntitySpawnerSettings gridEntitySpawnerSettings;

        public uint RowCount => rowCount;
        public uint CollumnCount => collumnCount;
        public GridEntitySpawner.GridEntitySpawnerSettings GridEntitySpawnerSettings => gridEntitySpawnerSettings;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        ShowGizmoGrid();
    }

    private void ShowGizmoGrid()
    {
        if (Application.isPlaying) return;
        
        Gizmos.color = Color.blue;

        CreateGridAndCalculatePositions();
        foreach (Vector2 gridPoint in GridPositions)
        {
            Gizmos.DrawSphere(gridPoint, 5);
        }
    }
#endif
}
