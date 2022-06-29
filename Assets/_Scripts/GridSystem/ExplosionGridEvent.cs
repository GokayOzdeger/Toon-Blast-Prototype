using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionGridEvent : IGridEvent
{
    private readonly int MinGroupSizeForExplosion = 2;
    
    public bool ProceedGridAfterEventEnds => true;
    public bool MakeGridUninterractableOnStart => true;
    

    private GridController _gridController;

    private int _entitiesToDestory;
    private int _entitiesDestroyed;
    
    public void OnEventEnd()
    {
        Debug.Log("ExplosionGridEvent.OnEventEnd");
        _gridController.OnGridEventEnd(this);
    }

    public bool TryEventStart<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity
    {
        if (!grid.GridInterractable) return false;
        if (effectedEntities.Count < MinGroupSizeForExplosion) return false;

        _gridController = grid;
        _entitiesToDestory = effectedEntities.Count;

        _gridController.OnGridEventStart(this);
        _gridController.RemoveEntitiesFromGridArray(effectedEntities);

        foreach (IGridEntity entityObject in effectedEntities)
        {
            _gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.DestoryEntityWithCallback(() => OnEntityDestroyed()); 
        }
        return true;
    }

    private void OnEntityDestroyed()
    {
        _entitiesDestroyed++;
        if (_entitiesDestroyed == _entitiesToDestory) OnEventEnd();
    }
}
