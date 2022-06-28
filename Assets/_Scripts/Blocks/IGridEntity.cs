using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEntity
{
    public BlockTypeDefinition BlockType { get; }
    public Vector2Int GridCoordinates { get; }
    public bool EntityNeedsUpdate { get; set; }
    public void OnGridChange(Vector2Int changeCoordinate);
    public void OnMoveEntity(Vector2Int newCoordinates);
    public void OnUpdateEntity();
}
