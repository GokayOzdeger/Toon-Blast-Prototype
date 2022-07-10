using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour, IPoolable
{
    private List<Tween> _lastTweens = new List<Tween>();
    [SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    public void PunchScale(float sizePercentage, float duration, bool endsLastTweens = true)
    {
        CompleteLastTweens();
        _lastTweens.Add(transform.DOPunchScale(transform.localScale * sizePercentage, duration, 1, 0));
    }

    public void DoColorFlash(Color color, float duration, bool endsLastTweens = true)
    {
        if(endsLastTweens) CompleteLastTweens();
        foreach (SpriteRenderer sprite in sprites)
        {
            Color defaultColor = sprite.color;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(sprite.DOColor(color, duration/2f));
            sequence.Append(sprite.DOColor(defaultColor, duration/2f));
            _lastTweens.Add(sequence);
        }
    }

    public void CompleteLastTweens()
    {
        foreach(Tween tween in _lastTweens) tween.Complete();
        _lastTweens.Clear();
    }

    public void OnGoToPool()
    {
        CompleteLastTweens();
    }

    public void OnPoolSpawn()
    {
        //
    }
}
