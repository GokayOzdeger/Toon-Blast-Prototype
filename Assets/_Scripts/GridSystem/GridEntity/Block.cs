using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Utilities;

public class Block : BasicFallingGridEntity
{
    private readonly float DestroyAnimationDuration = .2f;
   
    public List<Block> CurrentBlockGroup { get; set; }
    public int GroupSize { get { if (CurrentBlockGroup == null) return 0; return CurrentBlockGroup.Count; } }
    
    private bool _explodesOnClick;

    public void AnimateBlockPunchScale(Action onComplete = null)
    {
        CompleteLastTween();
        _lastTween = transform.DOPunchScale(new Vector2(.15f, .15f), DestroyAnimationDuration);
        if (onComplete != null) _lastTween.onComplete += () => onComplete();
    }

    public void AnimateBlockShake(Action onComplete = null)
    {
        int randomDirection = UnityEngine.Random.value < .5 ? 1 : -1;
        CompleteLastTween();
        _lastTween = transform.DOPunchRotation(new Vector3(0, 0, randomDirection * 14), .1f);
        _lastTween.onComplete += () => transform.DOPunchRotation(new Vector3(0, 0, -randomDirection * 7), .1f);
        if (onComplete != null) _lastTween.onComplete += () => onComplete();
    }

    private void TagUpgradeNeededForAllGroupMembers()
    {
        if (CurrentBlockGroup != null) foreach (Block block in CurrentBlockGroup) block.EntityNeedsUpdate = true;
    }

    public void OnClickBlock()
    {
        TryExplode();
    }

    public void TryExplode()
    {
        if (!_gridController.GridInterractable) return;
        if (!_explodesOnClick) return;
        if (!CreateExplosionEvent()) ExplodeFail();
    }

    private bool CreateExplosionEvent()
    {
        ExplosionGridEvent explosionEvent = new ExplosionGridEvent();
        return explosionEvent.TryEventStart(_gridController, CurrentBlockGroup);
    }

    private void ExplodeFail()
    {
        AnimateBlockShake();
    }

    public override void SetupEntity(GridController grid, IGridEntityTypeDefinition blockType)
    {
        base.SetupEntity(grid, blockType);
        _explodesOnClick = ((BlockTypeDefinition)blockType).ExplodesOnClick;
    }

    public override void OnPoolSpawn()
    {
        base.OnPoolSpawn();
        CurrentBlockGroup = null;
    }

    public override void OnUpdateEntity()
    {
        base.OnUpdateEntity();
        
        TagUpgradeNeededForAllGroupMembers();
        List<Block> blockGroup = new List<Block>();
        _gridController.CollectMatchingSurroundingEntities<Block>(this, ref blockGroup);

        Sprite blockImageForAllGroup = ((BlockTypeDefinition)EntityType).GetBlockGroupIcon(blockGroup);

        foreach (Block block in blockGroup)
        {
            block.CurrentBlockGroup = blockGroup;
            block.EntityNeedsUpdate = false;
        }
        foreach (Block block in blockGroup) block.SetBlockImage(blockImageForAllGroup);
    }

    public override void DestoryEntityWithCallback(Action onDestroy)
    {
        AnimateBlockShake(onDestroy);
        poolObject.GoToPool(DestroyAnimationDuration);
    }
}
