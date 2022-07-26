using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlocksGridEvent : IGridEvent
{
    private GridController _gridController;

    private int _entitiesToDestory;
    private int _entitiesDestroyed;

    public void OnEventEnd()
    {
        _gridController.OnGridEventEnd(this);
    }

    public bool TryEventStart<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity
    {
        _gridController = grid;
        _entitiesToDestory = effectedEntities.Count;

        _gridController.OnGridEventStart(this);
        _gridController.RemoveEntitiesFromGridArray(effectedEntities);

        foreach (IGridEntity entityObject in effectedEntities)
        {
            _gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.OnEntityDestroyed.AddListener(OnEntityDestroyed);
            entityObject.DestoryEntity();
        }
        return true;
    }

    private void OnEntityDestroyed(IGridEntity entityDestroyed)
    {
        _entitiesDestroyed++;
        if (_entitiesDestroyed == _entitiesToDestory) OnEventEnd();
    }
}
