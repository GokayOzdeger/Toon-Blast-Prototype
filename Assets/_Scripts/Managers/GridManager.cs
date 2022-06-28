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

public class GridManager : AutoSingleton<GridManager>
{
    private static Vector2Int[] _surroundingCoordinateMatrises = new Vector2Int[]
    {
        new Vector2Int( 0,-1),
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
    };

    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private BlockTypeDefinition[] blockTypes;
    [SerializeField] private RectTransform gridParentTransform;
    [SerializeField] private CanvasScaler canvasScaler;

    [SerializeField] private uint gridRowCount;
    [SerializeField] private uint gridCollumnCount;
    [SerializeField] private float collumnSpacing;
    [SerializeField] private float rowSpacing;

    
    public float CollumnSpacing { get { return collumnSpacing * canvasScaler.transform.localScale.x; } }
    public float RowSpacing { get { return rowSpacing * canvasScaler.transform.localScale.y; } }

    public UnityEvent<Vector2Int> OnGridChange = new UnityEvent<Vector2Int>();
    public Vector2[,] GridPositions { get; private set; }
    public IGridEntity[,] EntityGrid { get; private set; }
    public bool GridInAction { get; private set; } = false;

    private List<Vector2Int> _cachedGridChanges = new List<Vector2Int>();
    private BlockSpawner _blockSpawner;
    private FallingBlocksController _fallingBlocksController;

    private bool[,] controlledGridCoordinates;


    private List<Vector3> spherePoses = new List<Vector3>();

    private void Start()
    {
        CreateSystemComponents();
        CreateGridAndCalculatePositions();
        SummonBlocks();
        UpdateAllEntities();
    }

    private void CreateSystemComponents()
    {
        _blockSpawner = new BlockSpawner(gridCollumnCount);
        _fallingBlocksController = new FallingBlocksController(RowSpacing);
    }

    private void SummonBlocks()
    {
        for (int i = 0; i < GridPositions.GetLength(0); i++)
        {
            for (int j = 0; j < GridPositions.GetLength(1); j++)
            {
                Block newBlock = Instantiate(blockPrefab, GridPositions[i, j], Quaternion.identity, gridParentTransform).GetComponent<Block>();
                EntityGrid[i, j] = newBlock;
                newBlock.gameObject.name = $"Block {i}_{j}";
                newBlock.UpdateBlockCoordinates(new Vector2Int(i,j));
                BlockTypeDefinition randomBlockType = blockTypes[UnityEngine.Random.Range(0,blockTypes.Length)];
                newBlock.SetupBlock(randomBlockType);
                OnGridChange.AddListener(newBlock.OnGridChange);
            }
        }
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
        CallCachedGridChanges();
        UpdateAllEntities();
        StartCoroutine(WaitExplodeAnimation(blockToExplode.CurrentBlockGroup));
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

    private void CallCachedGridChanges()
    {
        while (_cachedGridChanges.Count > 0)
        {
            OnGridChange.Invoke(_cachedGridChanges[0]);
            _cachedGridChanges.RemoveAt(0);
        }
    }

    private void CacheGridChange(Vector2Int changeCords)
    {
        spherePoses.Add(GridPositions[changeCords.x, changeCords.y]);
        _cachedGridChanges.Add(changeCords);
    }

    private IEnumerator WaitExplodeAnimation(List<Block> blockGroupExploded)
    {
        yield return new WaitForSeconds(.2f);
        foreach (Block block in blockGroupExploded) Destroy(block.gameObject);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.2f);
        GridInAction = false;
    }

    //private void FindFallingBlocksGroups(HashSet<int> collumnIndexes)
    //{
    //    List<FallingBlocksGroup> fallingBlocksGroups = new List<FallingBlocksGroup>();

    //    foreach(int collumnIndex in collumnIndexes)
    //    {
    //        int fallDistance = 0;
    //        for (int i = 0; i < gridRowCount; i++) if (BlockGrid[i, collumnIndex] == null) fallDistance++;

    //        int fallDistanceToSkip = 0;
    //        bool blockGroupCompleted = false;
    //        List<Block> blockGroup = new List<Block>();

    //        for (int i = (int)gridRowCount - 1; i >= 0; i--)
    //        {
    //            Block currentBlock = BlockGrid[i, collumnIndex];
    //            if (currentBlock == null)
    //            {
    //                if (blockGroup.Count == 0) 
    //                {
    //                    fallDistance--;
    //                    continue; 
    //                }
    //                fallDistanceToSkip++;
    //                blockGroupCompleted = true;
    //                if(i == 0)
    //                {
    //                    FallingBlocksGroup fallingGroup = new FallingBlocksGroup(blockGroup, fallDistance);
    //                    fallingBlocksGroups.Add(fallingGroup);
    //                    fallDistance = 0;
    //                    blockGroupCompleted = false;
    //                    blockGroup = new List<Block>();
    //                }
    //            }
    //            else
    //            {
    //                if (blockGroupCompleted)
    //                {
    //                    TagBlocksAsNeedsUpdate(blockGroup);
    //                    FallingBlocksGroup fallingGroup = new FallingBlocksGroup(blockGroup, fallDistance);
    //                    fallingBlocksGroups.Add(fallingGroup);
    //                    fallDistance -= fallDistanceToSkip;
    //                    fallDistanceToSkip = 0;
    //                    blockGroupCompleted = false;
    //                    blockGroup = new List<Block>();
    //                }
    //                blockGroup.Add(currentBlock);
    //                if (currentBlock.BlockNeedsUpdate == false) TagBlocksAsNeedsUpdate(currentBlock.CurrentBlockGroup);
    //            }
    //        }
    //    }

    //    WriteFallingBlocksToGrid(fallingBlocksGroups);
    //    _fallingBlocksController.CreateFallingBlockGroup(fallingBlocksGroups);
    //    UpdateAllBlocks();
    //}

    private void TagBlocksAsNeedsUpdate(List<Block> blocksToTag)
    {
        foreach (Block block in blocksToTag)
        {
            block.EntityNeedsUpdate = true;
        }
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

    //private void UpdateSingleBlock(Block singleBlock)
    //{
    //    if (!singleBlock.BlockNeedsUpdate) return;
    //    List<Block> blockGroup = new List<Block>();
    //    CollectMatchingSurroundingBlocks(singleBlock, ref blockGroup);

    //    int groupSize = blockGroup.Count;

    //    foreach(Block block in blockGroup)
    //    {
    //        block.CurrentBlockGroup = blockGroup;
    //        block.BlockNeedsUpdate = false;
    //    }

    //    Sprite blockImageForAllGroup = singleBlock.BlockType.GetBlockGroupIcon(blockGroup);
    //    foreach (Block block in blockGroup) block.SetBlockImage(blockImageForAllGroup);
    //}

    private void CreateGridAndCalculatePositions()
    {
        EntityGrid = new Block[gridRowCount, gridCollumnCount];
        GridPositions = new Vector2[gridRowCount, gridCollumnCount];
        
        // calculate bottom left corn9er block position 
        float bottomLeftCornerX = gridParentTransform.position.x - ((gridCollumnCount / 2f) - .5f) * CollumnSpacing;
        float bottomLeftCornerY = gridParentTransform.position.y - ((gridRowCount / 2f) - .5f) * RowSpacing;

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
        foreach(Vector3 pos in spherePoses)
        {
            Gizmos.DrawSphere(pos, 50f);
            
        }
        ShowGizmoGrid();
        //ShowBlockGroupSizes();
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
