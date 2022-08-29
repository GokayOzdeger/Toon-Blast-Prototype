using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SpinEffect : MonoBehaviour
{
    Tween _activeTween;
   
    void OnEnable()
    {
        _activeTween = TweenHelper.Spin(transform, null, 3, true);
    }
    private void OnDisable()
    {
        _activeTween.Kill();
    }
}
