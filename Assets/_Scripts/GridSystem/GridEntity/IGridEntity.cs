using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IGridEntity : IPoolable
{    
    public Transform EntityTransform { get; }
    public IGridEntityTypeDefinition EntityType { get; }
    public Vector2Int GridCoordinates { get; }
    public UnityEvent<IGridEntity> OnEntityDestroyed { get; }
     
    public void SetupEntity(GridController grid, IGridEntityTypeDefinition entityType);
    public bool IsDestroyableBy(EntityDestroyTypes destroyType);
    public void DestoryEntity(EntityDestroyTypes destroyType);
    public void DestoryEntity(EntityDestroyTypes destroyType, float delay);
    public void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType);
    public void OnMoveEntity(Vector2Int newCoordinates, MovementMode movementMode);
    public void OnUpdateEntity();
    public void GotoPoolWithDelay(float delay);
}

public enum MovementMode
{
    Linear,
    Curvy
}

public enum GridChangeEventType
{
    EntityDestroyed,
    EntityMatched,
    EntityMoved,
}

public enum EntityDestroyTypes
{
    DestroyedByMatch,
    DestroyedByPowerUp,
    DestroyedByFallOff,
    DestroyedByNearbyMatch,
    DestroyedByLevelEnd,
}
