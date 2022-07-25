using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : BasicFallingGridEntity
{
    private readonly float DestroyAnimationDuration = .5f;

    private bool willPop = false;

    public override void OnMoveEnded()
    {
        base.OnMoveEnded();
        if (willPop)
        {
            willPop = false;
            Debug.Log("Duck destroy start");
            DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent();
            destroyEvent.TryEventStart(_gridController, new List<Balloon>() { this });
        }
    }

    public override void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType)
    {
        base.OnGridChange(changeCoordinate, gridChangeEventType);
        if(gridChangeEventType == GridChangeEventType.EntityDestroyed)
        {
            Debug.Log("Balloon destroy start: "+ (GridCoordinates - changeCoordinate)+" -- "+ (GridCoordinates - changeCoordinate).magnitude);
            if ((GridCoordinates - changeCoordinate).magnitude <= 1)
            {
                willPop = true;
            }
        }
    }

    public override void DestoryEntity()
    {
        AnimateDestroy();
    }

    public void AnimateDestroy()
    {
        int randomDirection = UnityEngine.Random.value < .5 ? 1 : -1;
        CompleteLastTween();
        _lastTween = transform.DOPunchScale(new Vector3(.3f, .3f, .3f), DestroyAnimationDuration);
        _lastTween.onComplete += OnEntityDestroy;
    }

    private void OnEntityDestroy()
    {
        OnEntityDestroyed.Invoke();
        poolObject.GoToPool();
    }
}
