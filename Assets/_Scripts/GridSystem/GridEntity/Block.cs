using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Utilities;
using AudioSystem;

public class Block : FallingGridEntity
{
    public List<Block> CurrentMatchGroup { get; private set; }
    public int GroupSize { get { if (CurrentMatchGroup == null) return 0; return CurrentMatchGroup.Count; } }

    private bool MatchGroupCalculated = false;

    public void OnClickBlock()
    {
        TryMatch();
    }

    private void TryMatch()
    {
        if (!_gridController.GridInterractable) return;
        bool matchSuccess = GameManager.Instance.CurrentLevel.MovesController.TryMakeMatchMove(this);
        if (!matchSuccess) MatchFail();
    }

    private void MatchFail()
    {
        AnimateShake();
    }

    public void AnimateShake()
    {
        CompleteLastTween();
        _lastTween = TweenHelper.Shake(transform);
    }

    public void AnimateDestroy()
    {
        CompleteLastTween();
        _lastTween = TweenHelper.PunchScale(transform, OnEntityDestroy);
    }
    
    public override void OnPoolSpawn()
    {
        base.OnPoolSpawn();
        CurrentMatchGroup = null;
    }

    public override void OnMoveEntity(Vector2Int newCoordinates, MovementMode movementMode)
    {
        // if this block moved, we need to recalculate the match group
        MatchGroupCalculated = false;
        base.OnMoveEntity(newCoordinates, movementMode);
    }
    public override void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        base.OnGridChange(changeCoordinate, gridChangeEventType, entityType);
        
        // if a surrounding block is changed, we need to recalculate the match group
        if ((GridCoordinates - changeCoordinate).magnitude == 1)
            MatchGroupCalculated = false;
    }

    public override void OnUpdateEntity()
    { 
        base.OnUpdateEntity();
        CheckMatchGroup();
    }

    private void CheckMatchGroup()
    {
        if (MatchGroupCalculated) return;
        List<Block> blockGroup = new List<Block>();
        _gridController.CollectMatchingSurroundingEntities<Block>(this, ref blockGroup);

        AssignMatchGroup(blockGroup);
        BlockMatchCondition? condition = ActiveBlockCondition();
        
        foreach (Block block in blockGroup)
        {
            block.AssignMatchGroup(blockGroup);
            block.SetSpriteOfCondition(condition);
        }
    }

    public override void DestoryEntity(EntityDestroyTypes destroyType)
    {
        AnimateDestroy();
    }

    public void MoveToPointThanDestroy(Vector3 position)
    {
        _lastTween = transform.DOMove(position, .3f).SetEase(Ease.InBack);
        _lastTween.onComplete += () => OnEntityDestroy();
    }

    private void OnEntityDestroy()
    {
        PlayOnDestroyAudio();
        OnEntityDestroyed.Invoke(this);
        poolObject.GoToPool();
    }

    private void SetSpriteOfCondition(BlockMatchCondition? condition)
    {
        if (condition != null) entityImage.sprite = condition.Value.Sprite;
        else entityImage.sprite = EntityType.DefaultEntitySprite;
    }

    public void AssignMatchGroup(List<Block> group)
    {
        MatchGroupCalculated = true;
        CurrentMatchGroup = group;
    }

    public BlockMatchCondition? ActiveBlockCondition()
    {
        BlockTypeDefinition blockTypeDefinition = EntityType as BlockTypeDefinition;
        foreach (var blockMatchCondition in blockTypeDefinition.BlockMatchConditions)
        {
            if (blockMatchCondition.Condition.IsConditionMet(this))
            {
                return blockMatchCondition;
            }
        }
        return null;
    }
}
