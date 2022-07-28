using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : FallingGridEntity
{
    public override void OnMoveEnded()
    {
        base.OnMoveEnded();
        if (GridCoordinates.x == 0)
        {
            DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent(EntityDestroyTypes.DestroyedByFallOff);
            destroyEvent.StartEvent(_gridController, new List<Duck>() { this });
        }
    }

    public override void DestoryEntity(EntityDestroyTypes destroyType)
    {
        if (destroyType != EntityDestroyTypes.DestroyedByFallOff  && destroyType != EntityDestroyTypes.DestroyedByLevelEnd) return;
        AnimateDestroy();
    }

    public void AnimateDestroy()
    {
        CompleteLastTween();
        _lastTween = TweenHelper.PunchScale(transform, OnEntityDestroy);
    }

    private void OnEntityDestroy()
    {
        PlayOnDestroyAudio();
        OnEntityDestroyed.Invoke(this);
        poolObject.GoToPool();
    }
}
