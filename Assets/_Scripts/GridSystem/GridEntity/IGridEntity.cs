using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent OnEntityDestroyed { get; }
    public void SetupEntity(GridController gridController, IGridEntityTypeDefinition entityType);
    public void DestoryEntity();
    public void OnGridChange(Vector2Int changeCoordinate);
    public void OnMoveEntity(Vector2Int newCoordinates, MovementMode movementMode);
    public void OnUpdateEntity();
}
