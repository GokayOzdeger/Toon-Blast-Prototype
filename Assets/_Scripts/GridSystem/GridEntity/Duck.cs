using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : BasicFallingGridEntity
{
    private readonly float DestroyAnimationDuration = .5f;
    
    public override void OnMoveEnded()
    {
        base.OnMoveEnded();
        if (GridCoordinates.x == 0)
        {
            Debug.Log("Duck destroy start");
            DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent();
            destroyEvent.TryEventStart(_gridController, new List<Duck>() { this });
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
