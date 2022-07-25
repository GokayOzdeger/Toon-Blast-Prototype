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
   
    public List<Block> CurrentMatchGroup { get; private set; }
    public int GroupSize { get { if (CurrentMatchGroup == null) return 0; return CurrentMatchGroup.Count; } }
    
    private bool MatchGroupCalculated = false;
    private bool _matchOnClick;

    public void AnimateBlockPunchScale(Action onComplete = null)
    {
        CompleteLastTween();
        _lastTween = transform.DOPunchScale(new Vector2(.15f, .15f), DestroyAnimationDuration);
        if (onComplete != null) _lastTween.onComplete += () => onComplete();
    }

    public void AnimateShake()
    {
        int randomDirection = UnityEngine.Random.value < .5 ? 1 : -1;
        CompleteLastTween();
        _lastTween = transform.DOPunchRotation(new Vector3(0, 0, randomDirection * 14), .1f);
        _lastTween.onComplete += () => transform.DOPunchRotation(new Vector3(0, 0, -randomDirection * 7), .1f);
    }

    public void AnimateDestroy()
    {
        int randomDirection = UnityEngine.Random.value < .5 ? 1 : -1;
        CompleteLastTween();
        _lastTween = transform.DOPunchScale(new Vector3(.3f, .3f, .3f), DestroyAnimationDuration);
        _lastTween.onComplete += OnEntityDestroy;
    }

    public void OnClickBlock()
    {
        TryMatch();
    }

    public void TryMatch()
    {
        if (!_gridController.GridInterractable) return;
        if (!_matchOnClick) return;
        bool matchSuccess = CreateMatchEvent();
        Debug.Log("TryMatch: " + matchSuccess);
        if (!matchSuccess) MatchFail();
    }

    private bool CreateMatchEvent()
    {
        MatchGridEvent matchEvent = new MatchGridEvent();
        return matchEvent.TryEventStart(_gridController, CurrentMatchGroup);
    }

    private void MatchFail()
    {
        AnimateShake();
    }

    public override void SetupEntity(GridController grid, IGridEntityTypeDefinition blockType)
    {
        base.SetupEntity(grid, blockType);
        _matchOnClick = ((BlockTypeDefinition)blockType).MatchesOnClick;
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
    public override void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType)
    {
        base.OnGridChange(changeCoordinate, gridChangeEventType);
        
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

        foreach (Block block in blockGroup)
        {
            block.AssignMatchGroup(blockGroup);
        }
    }

    public override void DestoryEntity()
    {
        AnimateDestroy();
    }

    private void OnEntityDestroy()
    {
        Debug.Log("OnEntityDestroy: " + StackTraceUtility.ExtractStackTrace());
        OnEntityDestroyed.Invoke();
        poolObject.GoToPool();
    }

    public void AssignMatchGroup(List<Block> group)
    {
        MatchGroupCalculated = true;
        CurrentMatchGroup = group;
    }
}
