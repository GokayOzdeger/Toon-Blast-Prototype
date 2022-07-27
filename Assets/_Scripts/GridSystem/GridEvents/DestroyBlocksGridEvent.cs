using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlocksGridEvent : IGridEvent
{
    private GridController _gridController;

    private EntityDestroyTypes _destroyType;
    private int _entitiesToDestroy;
    private int _entitiesDestroyed;

    public DestroyBlocksGridEvent(EntityDestroyTypes destroyType) 
    {
        this._destroyType = destroyType;
    }

    public void OnEventEnd()
    {
        _gridController.OnGridEventEnd(this);
    }

    public void StartEvent<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity
    {
        // remove entity from list if entity is immune to destroy type
        for (int i = 0; i < effectedEntities.Count; i++)
        {
            if (effectedEntities[i].IsDestroyableBy(_destroyType)) _entitiesToDestroy++;
            else
            {
                effectedEntities.RemoveAt(i);
                i--;
            }
        }
        if (effectedEntities.Count == 0) return;
        _gridController = grid;

        _gridController.OnGridEventStart(this);
        _gridController.RemoveEntitiesFromGridArray(effectedEntities, GridChangeEventType.EntityDestroyed);

        foreach (IGridEntity entityObject in effectedEntities)
        {
            _gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.OnEntityDestroyed.AddListener(OnEntityDestroyed);
            entityObject.DestoryEntity(_destroyType);
        }
    }

    private void OnEntityDestroyed(IGridEntity entityDestroyed)
    {
        _entitiesDestroyed++;
        if (_entitiesDestroyed == _entitiesToDestroy) OnEventEnd();
    }
}
