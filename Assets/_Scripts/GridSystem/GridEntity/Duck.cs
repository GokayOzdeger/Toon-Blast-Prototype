using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : BasicFallingGridEntity
{
    public override void OnUpdateEntity()
    {
        base.OnUpdateEntity();
        if (GridCoordinates.y == 0) DestoryEntityWithCallback(null);
    }

    public override void DestoryEntityWithCallback(Action onDestroy)
    {
        base.DestoryEntityWithCallback(onDestroy);
        AnimateBlockShake(onDestroy);
    }

    public void AnimateBlockShake(Action onComplete = null)
    {
        int randomDirection = UnityEngine.Random.value < .5 ? 1 : -1;
        CompleteLastTween();
        _lastTween = transform.DOPunchRotation(new Vector3(0, 0, randomDirection * 14), .1f);
        _lastTween.onComplete += () => transform.DOPunchRotation(new Vector3(0, 0, -randomDirection * 7), .1f);
        if (onComplete != null) _lastTween.onComplete += () => onComplete();
    }
}
