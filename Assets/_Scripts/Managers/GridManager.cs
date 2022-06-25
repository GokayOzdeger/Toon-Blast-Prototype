using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GridManager : AutoSingleton<GridManager>
{
    [SerializeField] private GameObject blockPrefab;

    [SerializeField] private uint gridRows;
    [SerializeField] private uint gridCollumns;
    [SerializeField] private float collumnSpacing;
    [SerializeField] private float rowSpacing;

    public Vector2[,] GridPositions { get; private set; }
    public Block[,] BlockGrid { get; private set; }

    private void Start()
    {
        BlockGrid = new Block[gridRows, gridCollumns];
        CalculateGridPoints();
        SummonBlocks();
    }

    private void SummonBlocks()
    {
        for (int i = 0; i < GridPositions.GetLength(0); i++)
        {
            for (int j = 0; j < GridPositions.GetLength(1); j++)
            {
                Block newBlock = Instantiate(blockPrefab, GridPositions[i, j], Quaternion.identity).GetComponent<Block>();
                BlockGrid[i, j] = newBlock;
                newBlock.transform.SetParent(transform);
            }
        }
    }

    private void CalculateGridPoints()
    {
        GridPositions = new Vector2[gridRows, gridCollumns];
        float bottomLeftCornerX = transform.position.x - ((gridCollumns / 2f) - .5f) * collumnSpacing;
        float bottomLeftCornerY = transform.position.y - ((gridRows / 2f) - .5f) * rowSpacing;
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        ShowGizmoGrid();
    }

    private void ShowGizmoGrid()
    {
        Gizmos.color = Color.blue;

        CalculateGridPoints();
        foreach (Vector2 gridPoint in GridPositions)
        {
            Gizmos.DrawSphere(gridPoint, 5);
        }
    }
#endif
}
