using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

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

    [SerializeField] private uint gridRowCount;
    [SerializeField] private uint gridCollumnCount;
    [SerializeField] private float collumnSpacing;
    [SerializeField] private float rowSpacing;

    public Vector2[,] GridPositions { get; private set; }
    public Block[,] BlockGrid { get; private set; }
    public bool GridInAction { get; private set; } = false;

    private BlockSpawner _blockSpawner;
    private FallingBlocksController _fallingBlocksController;

    private bool[,] controlledGridCoordinates;


    private void Start()
    {
        CreateSystemComponents();
        CreateGridAndCalculatePositions();
        SummonBlocks();
        UpdateAllBlocks();
    }

    private void CreateSystemComponents()
    {
        _blockSpawner = new BlockSpawner(gridCollumnCount);
        _fallingBlocksController = new FallingBlocksController(rowSpacing);
    }

    private void SummonBlocks()
    {
        for (int i = 0; i < GridPositions.GetLength(0); i++)
        {
            for (int j = 0; j < GridPositions.GetLength(1); j++)
            {
                Block newBlock = Instantiate(blockPrefab, GridPositions[i, j], Quaternion.identity, gridParentTransform).GetComponent<Block>();
                BlockGrid[i, j] = newBlock;
                newBlock.UpdateBlockCoordinates(new Vector2Int(i,j));
                BlockTypeDefinition randomBlockType = blockTypes[UnityEngine.Random.Range(0,blockTypes.Length)];
                newBlock.SetupBlock(randomBlockType);
            }
        }
    }

    private void Update()
    {
        Debug.Log(BlockGrid[0, 0]);
    }

    public void TryExplode(Block blockToExplode)
    {
        if (GridInAction) return;
        if (blockToExplode.CurrentBlockGroup.Count < 2) ExplodeFail(blockToExplode);
        else ExplodeSuccess(blockToExplode);
    }

    private void ExplodeSuccess(Block blockToExplode)
    {
        Debug.Log("EXPLODE BOK");
        GridInAction = true;
        HashSet<int> collumnsEffectedByExplosion = new HashSet<int>();
        
        foreach (Block block in blockToExplode.CurrentBlockGroup)
        {
            block.AnimateBlockPunchScale();
            collumnsEffectedByExplosion.Add(block.GridCoordinates.y);
            _blockSpawner.AddBlockSpawnReqeust(block.GridCoordinates.y);
            RemoveBlockFromGridArray(block);
        }
        FindFallingBlocksGroups(collumnsEffectedByExplosion);
        StartCoroutine(WaitExplodeAnimation(blockToExplode.CurrentBlockGroup));
    }

    private void ExplodeFail(Block blockToExplode)
    {
        blockToExplode.AnimateBlockShake();
    }

    private void RemoveBlockFromGridArray(Block blockToRemove)
    {
        BlockGrid[blockToRemove.GridCoordinates.x, blockToRemove.GridCoordinates.y] = null;
    }

    private IEnumerator WaitExplodeAnimation(List<Block> blockGroupExploded)
    {
        yield return new WaitForSeconds(.2f);
        foreach (Block block in blockGroupExploded) Destroy(block.gameObject);
        GridInAction = false;
    }

    private void FindFallingBlocksGroups(HashSet<int> collumnIndexes)
    {
        List<FallingBlocksGroup> fallingBlocksGroups = new List<FallingBlocksGroup>();

        foreach(int collumnIndex in collumnIndexes)
        {
            Debug.Log("Collumn in process bok: " + collumnIndex);
            int fallDistance = 0;
            for (int i = 0; i < gridRowCount; i++) if (BlockGrid[i, collumnIndex] == null) fallDistance++;

            int fallDistanceToSkip = 0;
            bool blockGroupCompleted = false;
            List<Block> blockGroup = new List<Block>();

            for (int i = (int)gridRowCount - 1; i >= 0; i--)
            {
                Block currentBlock = BlockGrid[i, collumnIndex];
                if (currentBlock == null)
                {
                    Debug.Log("Block is null bok: " + i);
                    if (blockGroup.Count == 0) 
                    {
                        fallDistance--;
                        continue; 
                    }
                    fallDistanceToSkip++;
                    blockGroupCompleted = true;
                    if(i == 0)
                    {
                        FallingBlocksGroup fallingGroup = new FallingBlocksGroup(blockGroup, fallDistance);
                        fallingBlocksGroups.Add(fallingGroup);
                        fallDistance = 0;
                        blockGroupCompleted = false;
                        blockGroup = new List<Block>();
                    }
                }
                else
                {
                    Debug.Log("Block is not null bok: " + i);
                    if (blockGroupCompleted)
                    {
                        TagBlocksAsNeedsUpdate(blockGroup);
                        FallingBlocksGroup fallingGroup = new FallingBlocksGroup(blockGroup, fallDistance);
                        fallingBlocksGroups.Add(fallingGroup);
                        fallDistance -= fallDistanceToSkip;
                        fallDistanceToSkip = 0;
                        blockGroupCompleted = false;
                        blockGroup = new List<Block>();
                    }
                    blockGroup.Add(currentBlock);
                    Debug.Log(currentBlock.name);
                    if (currentBlock.BlockNeedsUpdate == false) TagBlocksAsNeedsUpdate(currentBlock.CurrentBlockGroup);
                }
            }
        }

        WriteFallingBlocksToGrid(fallingBlocksGroups);
        _fallingBlocksController.CreateFallingBlockGroup(fallingBlocksGroups);
        UpdateAllBlocks();
    }

    private void TagBlocksAsNeedsUpdate(List<Block> blocksToTag)
    {
        foreach (Block block in blocksToTag)
        {
            block.BlockNeedsUpdate = true;
        }
    }

    private void WriteFallingBlocksToGrid(List<FallingBlocksGroup> allFallingBlocksGroups)
    {
        foreach (FallingBlocksGroup fallingBlocksGroup in allFallingBlocksGroups)
        {
            int groupCollumn = fallingBlocksGroup.FallingBlocks[0].GridCoordinates.y;
            for (int i = (int)fallingBlocksGroup.FallingBlocks.Count - 1; i >= 0; i--)
            {
                Block blockToWrite = fallingBlocksGroup.FallingBlocks[i];
                Debug.Log($"{blockToWrite.name}, falled from {blockToWrite.GridCoordinates.x} to {blockToWrite.GridCoordinates.x - fallingBlocksGroup.FallDistance}");
                Vector2Int newBlockCoordinates = new Vector2Int(blockToWrite.GridCoordinates.x - fallingBlocksGroup.FallDistance, groupCollumn);
                BlockGrid[blockToWrite.GridCoordinates.x, blockToWrite.GridCoordinates.y] = null;
                BlockGrid[newBlockCoordinates.x, newBlockCoordinates.y] = blockToWrite;
                blockToWrite.UpdateBlockCoordinates(newBlockCoordinates);
            }
        }
    }

    private void UpdateAllBlocks()
    {
        controlledGridCoordinates = new bool[gridRowCount, gridCollumnCount];
        foreach (Block block in BlockGrid)
        {
            if (block == null || !block.BlockNeedsUpdate) continue;
            UpdateSingleBlock(block);
        }
    }

    private void UpdateSingleBlock(Block singleBlock)
    {
        if (!singleBlock.BlockNeedsUpdate) return;
        List<Block> blockGroup = new List<Block>();
        CollectMatchingSurroundingBlocks(singleBlock, ref blockGroup);

        int groupSize = blockGroup.Count;

        foreach(Block block in blockGroup)
        {
            block.CurrentBlockGroup = blockGroup;
            block.BlockNeedsUpdate = false;
        }

        Sprite blockImageForAllGroup = singleBlock.BlockType.GetBlockGroupIcon(blockGroup);
        foreach (Block block in blockGroup) block.SetBlockImage(blockImageForAllGroup);
    }

    private void CreateGridAndCalculatePositions()
    {
        BlockGrid = new Block[gridRowCount, gridCollumnCount];
        GridPositions = new Vector2[gridRowCount, gridCollumnCount];
        
        // calculate bottom left corn9er block position 
        float bottomLeftCornerX = transform.position.x - ((gridCollumnCount / 2f) - .5f) * collumnSpacing;
        float bottomLeftCornerY = transform.position.y - ((gridRowCount / 2f) - .5f) * rowSpacing;
        
        Vector2 bottomLeftCorner = new Vector2(bottomLeftCornerX, bottomLeftCornerY);
        Vector2 cursorPoint = bottomLeftCorner;
        for (int i = 0; i < GridPositions.GetLength(0); i++)
        {
            for (int j = 0; j < GridPositions.GetLength(1); j++)
            {
                GridPositions[i, j] = cursorPoint;
                cursorPoint.x += collumnSpacing;
            }
            cursorPoint.y += rowSpacing;
            cursorPoint.x = bottomLeftCorner.x;
        }
    }

    //private void CollectSurroundingBlocks(Block block, ref List<Block> blockListToCollect)
    //{
    //    foreach (Vector2Int surroundingCoordinateAdd in _surroundingCoordinateMatrises)
    //    {
    //        Vector2Int surroundingCoordinate = block.GridCoordinates + surroundingCoordinateAdd;
    //        // Skip Surrounding check position if it is out of range or already controlled
    //        if (surroundingCoordinate.x < 0 || surroundingCoordinate.x == gridRowCount) continue;
    //        if (surroundingCoordinate.y < 0 || surroundingCoordinate.y == gridCollumnCount) continue;

    //        Block surroundingMatchingBlock = BlockGrid[surroundingCoordinate.x, surroundingCoordinate.y];
    //        blockListToCollect.Add(surroundingMatchingBlock);
    //    }
    //}

    private void CollectMatchingSurroundingBlocks(Block block, ref List<Block> blockListToCollect)
    {
        blockListToCollect.Add(block);
        controlledGridCoordinates[block.GridCoordinates.x, block.GridCoordinates.y] = true;
        CollectMatchingSurroundingBlocksRecursive(block, ref blockListToCollect);
    }

    private void CollectMatchingSurroundingBlocksRecursive(Block block, ref List<Block> blockListToCollect)
    {
        foreach (Vector2Int surroundingCoordinateAdd in _surroundingCoordinateMatrises)
        {
            Vector2Int surroundingCoordinate = block.GridCoordinates + surroundingCoordinateAdd;
            // Skip Surrounding check position if it is out of range or already controlled
            if (surroundingCoordinate.x < 0 || surroundingCoordinate.x == gridRowCount) continue;
            if (surroundingCoordinate.y < 0 || surroundingCoordinate.y == gridCollumnCount) continue;
            if (controlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y]) continue;

            Block surroundingMatchingBlock = BlockGrid[surroundingCoordinate.x, surroundingCoordinate.y];
            // skip block if type doesnt match
            if (surroundingMatchingBlock == null || surroundingMatchingBlock.BlockType != block.BlockType) continue;

            // mark position as controlled to prevent controlling same block more than once
            controlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y] = true;

            blockListToCollect.Add(surroundingMatchingBlock);
            CollectMatchingSurroundingBlocksRecursive(surroundingMatchingBlock, ref blockListToCollect);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
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
