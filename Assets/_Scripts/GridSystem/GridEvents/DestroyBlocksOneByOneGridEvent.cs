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
        Debug.Log("Remove from grid DestroyBlocksOneByOneGridEvent: ");

        CoroutineHelper.Instance.StartCoroutine(DestroyOneByOneRoutine(effectedEntities));
    }

    private IEnumerator DestroyOneByOneRoutine<T>(List<T> effectedEntities) where T : IGridEntity
    {
        foreach (IGridEntity entityObject in effectedEntities)
        {
            yield return new WaitForSeconds(.08f);
            _gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.OnEntityDestroyed.AddListener(OnEntityDestroyed);
            entityObject.DestoryEntity(_destroyType);
        }
    }

    private void OnEntityDestroyed(IGridEntity entityDestroyed)
    {
        _entitiesDestroyed++;
        Debug.Log("Ended DestroyBlocksGridEvent: " + _entitiesDestroyed + "/" + _entitiesToDestroy);
        if (_entitiesDestroyed == _entitiesToDestroy) OnEventEnd();
    }
}
