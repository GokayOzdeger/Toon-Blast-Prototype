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
    private static Vector2Int[] _surroundingCoordinateMatrises = new Vector2Int[]
    {
        new Vector2Int( 0,-1),
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
    };

    
    private CanvasScaler _canvasScaler;
    private RectTransform _gridCenterTransform;
    private RectTransform _gridOverlayTransform;
    
    private uint gridRowCount;
    private uint gridColumnCount;

    private float gridCellSpacing;

    public int RowCount { get { return (int)gridRowCount; } }
    public int ColumnCount { get { return (int)gridColumnCount; } }
    public float GridCellSpacing { get { return gridCellSpacing * _canvasScaler.transform.localScale.x; } }

    private UnityEvent<Vector2Int,GridChangeEventType,IGridEntityTypeDefinition> OnGridChange = new UnityEvent<Vector2Int,GridChangeEventType,IGridEntityTypeDefinition>();
    public Vector2[,] GridPositions { get; private set; }
    public IGridEntity[,] EntityGrid { get; private set; }
    public bool GridInterractable { get { return _gridEventsInProgress == 0 && _entitiesInProcess == 0; } }
    public RectTransform GridOverlay => _gridOverlayTransform;

    private List<(Vector2Int,GridChangeEventType,IGridEntityTypeDefinition)> _cachedGridChanges = new List<(Vector2Int,GridChangeEventType,IGridEntityTypeDefinition)>();
    private bool[,] _controlledGridCoordinates;
    private int _entitiesInProcess = 0;
    private int _gridEventsInProgress = 0;
    
    private ShuffleController _shuffleController; 

    private GridEntitySpawner _entitySpawner;
    private GridGoalsController _goalController;

    public GridController(GridControllerSettings settings , GridControllerSceneReferences references)
    {
        // cache settings and references
        this._canvasScaler = references.CanvasScaler;
        this._gridCenterTransform = references.GridRect;
        this._gridOverlayTransform = references.GridOverlay;
        
        gridRowCount = settings.RowCount;
        gridColumnCount = settings.ColumnCount;

        CalculateCellSpacing(settings, references);
        CreateGridAndCalculatePositions();
        ResizeGridFrame(references.GridFrame, settings);
    }

    public void StartGrid(ShuffleController shuffleController, GridEntitySpawner entitySpawner, GridGoalsController goalController)
    {
        _shuffleController = shuffleController;
        _entitySpawner = entitySpawner;
        _goalController = goalController;

        _entitySpawner.FillAllGridWithStartLayout();
        _shuffleController.CheckShuffleRequired();
        UpdateAllEntities();
    }

    #region Private Methods
    private void CalculateCellSpacing(GridControllerSettings settings, GridControllerSceneReferences references)
    {
        float rowCellSpacing = references.GridRect.rect.width / (settings.MaxEntitiesPerSide);
        float columnCellSpacing = references.GridRect.rect.height / (settings.MaxEntitiesPerSide);
        gridCellSpacing = Mathf.Min(rowCellSpacing, columnCellSpacing);
    }

    private void WriteEntityMovementToGrid(Vector2Int newCoordinates, IGridEntity entity)
    {
        Vector2Int oldEntityCoordinates = entity.GridCoordinates;
        EntityGrid[entity.GridCoordinates.x, entity.GridCoordinates.y] = null;
        EntityGrid[newCoordinates.x, newCoordinates.y] = entity;
        entity.OnMoveEntity(newCoordinates, MovementMode.Linear);
        CacheGridChange(newCoordinates, GridChangeEventType.EntityMoved, entity.EntityType);
        CacheGridChange(oldEntityCoordinates, GridChangeEventType.EntityMoved, entity.EntityType);
    }

    private void CacheGridChange(Vector2Int changeCords, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        _cachedGridChanges.Add((changeCords, gridChangeEventType, entityType));
    }

    // called on all grid entities after all grid changes are calculated
    public void UpdateAllEntities()
    {
        Array.Clear(_controlledGridCoordinates, 0, _controlledGridCoordinates.Length);
        foreach (IGridEntity entity in EntityGrid)
        {
            if (entity == null) continue;
            entity.OnUpdateEntity();
        }
    }

    private void CreateGridAndCalculatePositions()
    {
        EntityGrid = new IGridEntity[gridRowCount, gridColumnCount];
        GridPositions = new Vector2[gridRowCount, gridColumnCount];
        _controlledGridCoordinates = new bool[gridRowCount, gridColumnCount];

        // calculate bottom left corner entity position 
        float bottomLeftCornerX = _gridCenterTransform.position.x - ((gridColumnCount / 2f) - .5f) * GridCellSpacing;
        float bottomLeftCornerY = _gridCenterTransform.position.y - ((gridRowCount / 2f) - .5f) * GridCellSpacing;

        Vector2 bottomLeftCorner = new Vector2(bottomLeftCornerX, bottomLeftCornerY);
        Vector2 cursorPoint = bottomLeftCorner;
        for (int i = 0; i < GridPositions.GetLength(0); i++)
        {
            for (int j = 0; j < GridPositions.GetLength(1); j++)
            {
                GridPositions[i, j] = cursorPoint;
                cursorPoint.x += GridCellSpacing;
            }
            cursorPoint.y += GridCellSpacing;
            cursorPoint.x = bottomLeftCorner.x;
        }
    }

    private void ResizeGridFrame(RectTransform frame, GridControllerSettings settings)
    {
        Vector2 frameSizeAdd = new Vector2(settings.GridFrameWidthAdd, settings.GridFrameBottomAdd + settings.GridFrameTopAdd);
        frame.sizeDelta = new Vector2(ColumnCount * gridCellSpacing, RowCount * gridCellSpacing) + frameSizeAdd * GridCellSpacing;
        frame.transform.position += new Vector3(0, settings.GridFrameTopAdd - settings.GridFrameBottomAdd, 0) * GridCellSpacing;
    }

    private void CollectMatchingSurroundingEntitiesRecursive<T>(T entity, ref List<T> entityListToCollect) where T : IGridEntity
    {
        foreach (Vector2Int surroundingCoordinateAdd in _surroundingCoordinateMatrises)
        {
            Vector2Int surroundingCoordinate = entity.GridCoordinates + surroundingCoordinateAdd;
            // Skip Surrounding check position if it is out of range or already controlled
            if (surroundingCoordinate.x < 0 || surroundingCoordinate.x == gridRowCount) continue;
            if (surroundingCoordinate.y < 0 || surroundingCoordinate.y == gridColumnCount) continue;
            if (_controlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y]) continue;

            IGridEntity surroundingMatchingEntity = EntityGrid[surroundingCoordinate.x, surroundingCoordinate.y];
            // skip entity if type doesnt match
            if (surroundingMatchingEntity == null || surroundingMatchingEntity.EntityType != entity.EntityType) continue;

            // mark position as controlled to prevent controlling same entity more than once
            _controlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y] = true;
            T castEntity = (T)surroundingMatchingEntity;
            entityListToCollect.Add(castEntity);
            CollectMatchingSurroundingEntitiesRecursive(castEntity, ref entityListToCollect);
        }
    }
    #endregion

    #region Public Methods

    public List<IGridEntity> GetEntitiesInRow(int rowIndex)
    {
        List<IGridEntity> entitiesInRow = new List<IGridEntity>();
        for (int i = 0; i < gridColumnCount; i++)
        {
            IGridEntity entity = EntityGrid[rowIndex, i];
            if (entity != null)
            {
                entitiesInRow.Add(entity);
            }
        }
        return entitiesInRow;
    }

    public List<IGridEntity> GetEntitiesInColumn(int columnIndex)
    {
        List<IGridEntity> entitiesInRow = new List<IGridEntity>();
        for (int i = 0; i < gridRowCount; i++)
        {
            IGridEntity entity = EntityGrid[i, columnIndex];
            if (entity != null)
            {
                entitiesInRow.Add(entity);
            }
        }
        return entitiesInRow;
    }

    public void SwapEntities(Vector2Int entityACoordinates, Vector2Int entityBCoordinates)
    {
        IGridEntity entityA = EntityGrid[entityACoordinates.x, entityACoordinates.y];
        IGridEntity entityB = EntityGrid[entityBCoordinates.x, entityBCoordinates.y];
        EntityGrid[entityACoordinates.x, entityACoordinates.y] = EntityGrid[entityBCoordinates.x, entityBCoordinates.y];
        EntityGrid[entityBCoordinates.x, entityBCoordinates.y] = entityA;
        int entityASiblingIndex = entityA.EntityTransform.GetSiblingIndex();
        int entityBSiblingIndex = entityB.EntityTransform.GetSiblingIndex();
        entityA.EntityTransform.SetSiblingIndex(entityBSiblingIndex);
        entityB.EntityTransform.SetSiblingIndex(entityASiblingIndex);
        entityA.OnMoveEntity(entityBCoordinates, MovementMode.Curvy);
        entityB.OnMoveEntity(entityACoordinates, MovementMode.Curvy);
    }
    
    public void RegisterGridEntityToPosition(IGridEntity entity, int columnIndex, int rowIndex)
    {
        entity.OnEntityDestroyed.AddListener(_goalController.OnEntityDestroyed); // subscribe GridGoalController to new entity to keep track of destroyed entities
        EntityGrid[columnIndex, rowIndex] = entity;
        CacheGridChange(new Vector2Int(columnIndex, rowIndex), GridChangeEventType.EntityMoved, entity.EntityType);
        OnGridChange.AddListener(entity.OnGridChange);
    }

    public void OnGridEventStart(IGridEvent gridEvent)
    {
        _gridEventsInProgress++;
    }

    public void OnGridEventEnd(IGridEvent gridEvent)
    {
        _gridEventsInProgress--;
        CallCachedChanges();
        _entitySpawner.SummonRequestedEntities();
    }

    public void RemoveEntitiesFromGridArray<T>(List<T> entitiesToRemove) where T : IGridEntity
    {
        foreach (T entityToRemove in entitiesToRemove) OnGridChange.RemoveListener(entityToRemove.OnGridChange);
        foreach (T entityToRemove in entitiesToRemove)
        {
            EntityGrid[entityToRemove.GridCoordinates.x, entityToRemove.GridCoordinates.y] = null;
            CacheGridChange(entityToRemove.GridCoordinates, GridChangeEventType.EntityDestroyed, entityToRemove.EntityType);
        }
    }

    public void WriteEntityFall(IGridEntity gridEntity)
    { 
        int columnIndex = gridEntity.GridCoordinates.y;
        int coordinateToFallTo = gridEntity.GridCoordinates.x;
        for (int i = gridEntity.GridCoordinates.x - 1; i >= 0; i--)
        {
            if (EntityGrid[i, columnIndex] == null) coordinateToFallTo = i;
            else break;
        }
        WriteEntityMovementToGrid(new Vector2Int(coordinateToFallTo, columnIndex), gridEntity);
    }
    
    public void CallCachedChanges()
    {
        while (_cachedGridChanges.Count > 0)
        {
            OnGridChange.Invoke(_cachedGridChanges[0].Item1, _cachedGridChanges[0].Item2, _cachedGridChanges[0].Item3);
            _cachedGridChanges.RemoveAt(0);
        }
        UpdateAllEntities();
    }

    public void CallEntitySpawn(int columnIndex)
    {
        _entitySpawner.AddEntitySpawnReqeust(columnIndex);
    }
    
    public void EntityStartProcess()
    {
        _entitiesInProcess++;
    }

    public void EntityEndProcess()
    {
        _entitiesInProcess--;
        if (GridInterractable)
        {
            _shuffleController.CheckShuffleRequired();
        }
    }

    public void CollectMatchingSurroundingEntities<T>(T entity, ref List<T> entityListToCollect) where T : IGridEntity
    {
        entityListToCollect.Add(entity);
        _controlledGridCoordinates[entity.GridCoordinates.x, entity.GridCoordinates.y] = true;
        CollectMatchingSurroundingEntitiesRecursive(entity, ref entityListToCollect);
    }
    #endregion
}
