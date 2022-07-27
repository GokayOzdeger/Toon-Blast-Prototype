using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchGridEvent : IGridEvent
{
    private GridController _gridController;

    private Vector3 _matchMergePosition;
    private Vector2Int _matchMergeCoordinates;
    private BlockMatchCondition? _activeMatchCondition;
    private int _entitiesToDestroy;
    private int _entitiesDestroyed;
    
    public MatchGridEvent(Vector3 matchMergePosition, Vector2Int matchMergeCoordinates, BlockMatchCondition? activeMatchCondition)
    {
        this._matchMergeCoordinates = matchMergeCoordinates;
        this._activeMatchCondition = activeMatchCondition;
        this._matchMergePosition = matchMergePosition;
    }

    public void OnEventEnd()
    {
        if (_activeMatchCondition != null)
        {
            GridEntitySpawnController.Instance.SpawnEntity(_activeMatchCondition.Value.GetRandomEntityToSpawn(), _matchMergeCoordinates);
            GridEntitySpawnController.Instance.RemoveEntitySpawnReqeust(_matchMergeCoordinates.y);
        }
        _gridController.OnGridEventEnd(this);
    }

    public void StartEvent<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity
    {
        // remove entity from list if entity is immune to destroy type
        for (int i = 0; i < effectedEntities.Count; i++)
        {
            if (effectedEntities[i].IsDestroyableBy(EntityDestroyTypes.DestroyedByMatch)) _entitiesToDestroy++;
            else
            {
                effectedEntities.RemoveAt(i);
                i--;
            }
        }

        if (effectedEntities.Count == 0) return;
        _gridController = grid;

        _gridController.OnGridEventStart(this);
        _gridController.RemoveEntitiesFromGridArray(effectedEntities, GridChangeEventType.EntityMatched);

        foreach (IGridEntity entityObject in effectedEntities)
        {
            _gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.OnEntityDestroyed.AddListener(OnEntityDestroyed);

            if (_activeMatchCondition == null)
            {
                // normal destroy
                entityObject.DestoryEntity(EntityDestroyTypes.DestroyedByMatch); 
            }
            else  
            {
                // on condition block merge destroy
                Block block = entityObject as Block;
                if (!block) Debug.LogError("Matched an object with match conditions but the object is not a block");
                block.MoveToPointThanDestroy(_matchMergePosition);
            }
        }
    }

    private void OnEntityDestroyed(IGridEntity entityDestroyed)
    {
        _entitiesDestroyed++;
        if (_entitiesDestroyed == _entitiesToDestroy) OnEventEnd();
    }
}
