using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEntity
{
    public enum MovementMode
    {
        Linear,
        Curvy
    }
    
    public Transform EntityTransform { get; }
    public IGridEntityTypeDefinition EntityType { get; }
    public Vector2Int GridCoordinates { get; }
    public bool EntityNeedsUpdate { get; set; }
    public void SetupEntity(GridController gridController, IGridEntityTypeDefinition entityType);
    public void DestoryEntityWithCallback(Action afterDestroy);
    public void OnGridChange(Vector2Int changeCoordinate);
    public void OnMoveEntity(Vector2Int newCoordinates, MovementMode movementMode);
    public void OnUpdateEntity();
}
