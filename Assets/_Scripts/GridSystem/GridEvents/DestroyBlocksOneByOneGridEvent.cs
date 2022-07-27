using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlocksOneByOneGridEvent : IGridEvent
{
    private GridController _gridController;

    private EntityDestroyTypes _destroyType;
    private int _entitiesToDestroy;
    private int _entitiesDestroyed;

    public DestroyBlocksOneByOneGridEvent(EntityDestroyTypes destroyType)
    {
        this._destroyType = destroyType;
    }

    public void OnEventEnd()
    {
        _gridController.OnGridEventEnd(this);
    }

    public void StartEvent<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity
    {
        if (effectedEntities.Count == 0) return;
        Debug.Log("Started DestroyBlocksGridEvent: "+effectedEntities.Count);
        _gridController = grid;

        _gridController.OnGridEventStart(this);
        _gridController.RemoveEntitiesFromGridArray(effectedEntities);

        foreach (IGridEntity entityObject in effectedEntities)
        {
            if (!entityObject.IsDestroyableBy(_destroyType)) continue; // skip if entity is immune to destroy type
            _entitiesToDestroy++;
            _gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.OnEntityDestroyed.AddListener(OnEntityDestroyed);
            entityObject.DestoryEntity(_destroyType);
        }
    }

    private void OnEntityDestroyed(IGridEntity entityDestroyed)
    {
        _entitiesDestroyed++;
        Debug.Log("Ended DestroyBlocksGridEvent: " + _entitiesDestroyed);
        if (_entitiesDestroyed == _entitiesToDestroy) OnEventEnd();
    }
}
