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

    
    [SerializeField] private CanvasScaler canvasScaler;

    [SerializeField] private uint gridRowCount;
    [SerializeField] private uint gridCollumnCount;
    [SerializeField] private float collumnSpacing;
    [SerializeField] private float rowSpacing;

    public int RowCount { get { return (int)gridRowCount; } }
    public int CollumnCount { get { return (int)gridCollumnCount; } }
    public float CollumnSpacing { get { return collumnSpacing * canvasScaler.transform.localScale.x; } }
    public float RowSpacing { get { return rowSpacing * canvasScaler.transform.localScale.y; } }

    public UnityEvent<Vector2Int> OnGridChange = new UnityEvent<Vector2Int>();
    public Vector2[,] GridPositions { get; private set; }
    public IGridEntity[,] EntityGrid { get; private set; }
    public bool GridInAction { get; private set; } = false;

    private RectTransform gridCenterTransform;
    private List<Vector2Int> _cachedGridChanges = new List<Vector2Int>();
    private bool[,] controlledGridCoordinates;
    protected int _entitiesInProcess = 0; 

    private BlockSpawner _blockSpawner;

    private void Start()
    {
        CreateSystemComponents();
        CreateGridAndCalculatePositions();
        UpdateAllEntities();
    }

    private void CreateSystemComponents()
    {
        _blockSpawner = new BlockSpawner(this, gridCollumnCount);
    }

    public void RegisterGridEntityToPosition(IGridEntity entity, int collumnIndex, int rowIndex)
    {
        EntityGrid[collumnIndex, rowIndex] = entity;
        OnGridChange.AddListener(entity.OnGridChange);
    }

    public void TryExplode(Block blockToExplode)
    {
        if (GridInAction) return;
        if (blockToExplode.CurrentBlockGroup.Count < 2) ExplodeFail(blockToExplode);
        else ExplodeSuccess(blockToExplode);
    }

    private void ExplodeSuccess(Block blockToExplode)
    {
        GridInAction = true;
        
        RemoveEntitiesFromGridArray(blockToExplode.CurrentBlockGroup);
        
        foreach (Block block in blockToExplode.CurrentBlockGroup)
        {
            block.AnimateBlockPunchScale();
            _blockSpawner.AddBlockSpawnReqeust(block.GridCoordinates.y);
        }
    }

    public void ExplosionEnd()
    {
        CallCachedChanges();
    }

    private void ExplodeFail(Block blockToExplode)
    {
        blockToExplode.AnimateBlockShake();
    }

    private void RemoveEntitiesFromGridArray<T>(List<T> entitiesToRemove) where T: IGridEntity
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
            UpdateAllEntities();
            GridInAction = false;
        }
    }

    private void WriteEntityMovementToGrid(Vector2Int newCoordinates, IGridEntity entity)
    {
        Debug.Log($"Moving entity {entity.GridCoordinates} to {newCoordinates}");
        Vector2Int oldEntityCoordinates = entity.GridCoordinates;
        EntityGrid[entity.GridCoordinates.x, entity.GridCoordinates.y] = null;
        EntityGrid[newCoordinates.x, newCoordinates.y] = entity;
        entity.OnMoveEntity(newCoordinates);
        CacheGridChange(newCoordinates);
        CacheGridChange(oldEntityCoordinates);
    }

    private void CallCachedChanges()
    {
        while (_cachedGridChanges.Count > 0)
        {
            //List<Vector2Int> gridChangesInProcess = new List<Vector2Int>(_cachedGridChanges);
            //_cachedGridChanges.Clear();
            //foreach (Vector2Int change in gridChangesInProcess) OnGridChange.Invoke(change);
            //spherePoses.Clear();
            OnGridChange.Invoke(_cachedGridChanges[0]);
            _cachedGridChanges.RemoveAt(0);
        }
    }

    private void CacheGridChange(Vector2Int changeCords)
    {
        _cachedGridChanges.Add(changeCords);
    }

    //private IEnumerator WaitExplodeAnimation(List<Block> blockGroupExploded)
    //{
    //    yield return new WaitForSeconds(.2f);
    //    foreach (Block block in blockGroupExploded) Destroy(block.gameObject);
    //    yield return new WaitForSeconds(.3f);
    //    UpdateAllEntities();
    //    GridInAction = false;
    //}

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
        EntityGrid = new Block[gridRowCount, gridCollumnCount];
        GridPositions = new Vector2[gridRowCount, gridCollumnCount];
        
        // calculate bottom left corn9er block position 
        float bottomLeftCornerX = gridCenterTransform.position.x - ((gridCollumnCount / 2f) - .5f) * CollumnSpacing;
        float bottomLeftCornerY = gridCenterTransform.position.y - ((gridRowCount / 2f) - .5f) * RowSpacing;

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
            // skip block if type doesnt match
            if (surroundingMatchingEntity == null || surroundingMatchingEntity.BlockType != entity.BlockType) continue;

            // mark position as controlled to prevent controlling same block more than once
            controlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y] = true;
            T castEntity = (T)surroundingMatchingEntity;
            entityListToCollect.Add(castEntity);
            CollectMatchingSurroundingEntitiesRecursive(castEntity, ref entityListToCollect);
        }
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
