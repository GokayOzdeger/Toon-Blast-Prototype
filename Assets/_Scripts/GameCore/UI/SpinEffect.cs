using DG.Tweening;
using UnityEngine;

public class SpinEffect : MonoBehaviour
{
    private Tween _activeTween;

    private void OnEnable()
    {
        _activeTween = TweenHelper.Spin(transform, null, 3, true);
    }

    private void OnDisable()
    {
        _activeTween.Kill();
    }
}