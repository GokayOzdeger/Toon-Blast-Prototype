using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Block : MonoBehaviour
{
    [SerializeField] private Image blockImage;

    public BlockTypeDefinition BlockType { get; private set; }

    public Vector2Int GridCoordinates { get; private set; }

    public List<Block> CurrentBlockGroup { get; set; }

    public int GroupSize { get { return CurrentBlockGroup.Count; } }

    public bool BlockNeedsUpdate { get; set; } = true;

    private Tweener _lastTween = null;


    public void SetupBlock(BlockTypeDefinition blockType)
    {
        BlockType = blockType;
    }

    public void UpdateBlockCoordinates(Vector2Int coordinates)
    {
        GridCoordinates = coordinates;
    }

    public void SetBlockImage(Sprite sprite)
    {
        blockImage.sprite = sprite;
    }

    public void AnimateBlockPunchScale()
    {
        CompleteLastTween();
        _lastTween = transform.DOPunchScale(new Vector2(.15f, .15f), .2f);
    }

    public void AnimateBlockShake()
    {
        int randomDirection = Random.value < .5 ? 1 : -1;
        CompleteLastTween();
        _lastTween = transform.DOPunchRotation(new Vector3(0, 0, randomDirection * 14), .1f);
        _lastTween.onComplete += () => transform.DOPunchRotation(new Vector3(0, 0, -randomDirection * 7), .1f);
    }

    public void CompleteLastTween()
    {
        if (_lastTween != null) _lastTween.Complete(true);
    }

    public void CacheTween(Tweener tween)
    {
        _lastTween = tween;
    }

    public void OnClickBlock()
    {
        GridManager.Instance.TryExplode(this);
    }
}
