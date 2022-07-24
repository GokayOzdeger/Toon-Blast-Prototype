using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

public class BasicFallingGridEntity : MonoBehaviour, IGridEntity, IPoolable
{
    [SerializeField] protected Image entityImage;
    [SerializeField] protected PoolObject poolObject;

    public UnityEvent OnEntityDestroyed { get; private set; } = new UnityEvent();
    public IGridEntityTypeDefinition EntityType { get; protected set; }
    public Vector2Int GridCoordinates { get; protected set; }
    public Transform EntityTransform => transform;
    
    protected GridController _gridController;
    protected Tween _lastTween = null;
    
    public virtual void SetupEntity(GridController grid, IGridEntityTypeDefinition blockType)
    {
        _gridController = grid;
        EntityType = blockType;
        SetBlockImage(blockType.DefaultEntitySprite);
    }

    public void UpdateBlockCoordinates(Vector2Int coordinates)
    {
        GridCoordinates = coordinates;
    }

    public void SetBlockImage(Sprite sprite)
    {
        entityImage.sprite = sprite;
    }

    public virtual void OnGridChange(Vector2Int changeCoordinate)
    {
        if (GridCoordinates - changeCoordinate != new Vector2Int(1, 0)) return;
        if (_gridController.EntityGrid[changeCoordinate.x, changeCoordinate.y] != null) return;
        //entity reacting to grid change
        _gridController.WriteEntityFall(this);
    }

    public virtual void OnUpdateEntity()
    {
        //
    }

    public virtual void OnMoveEnded()
    {
        //
    }

    public virtual void OnMoveEntity(Vector2Int newCoordinates, IGridEntity.MovementMode movementMode)
    {
        MoveToCoordinate(newCoordinates, movementMode);
    }

    protected void MoveToCoordinate(Vector2Int newCoordinates, IGridEntity.MovementMode movementMode)
    {
        KillLastTween();
        _gridController.EntityStartProcess();
        GridCoordinates = newCoordinates;
        Vector2 targetPos = _gridController.GridPositions[newCoordinates.x, newCoordinates.y];
        float moveDuration = .5f;
        Tween moveTween = null;
        switch (movementMode)
        {
            case IGridEntity.MovementMode.Linear:
                moveTween = transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutBounce);
                break;
            case IGridEntity.MovementMode.Curvy:
                float curveAmountMultiplier = .2f;
                float distanceToTarget = Vector2.Distance((Vector2)transform.position, targetPos);
                Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
                Vector2 moveDirNormal = new Vector2(-moveDir.y, moveDir.x);
                Sequence sequenceTween = DOTween.Sequence();
                sequenceTween.Join(transform.DOBlendableMoveBy(moveDirNormal * distanceToTarget * curveAmountMultiplier, moveDuration/2).SetEase(Ease.InOutCubic));
                sequenceTween.Append(transform.DOBlendableMoveBy(-moveDirNormal * distanceToTarget * curveAmountMultiplier, moveDuration / 2).SetEase(Ease.InOutCubic));
                sequenceTween.Insert(0, transform.DOBlendableMoveBy(targetPos - (Vector2)transform.position, moveDuration).SetEase(Ease.InOutBack));
                moveTween = sequenceTween;
                break;
            default:
                break;
        }
        moveTween.onComplete += () => { _gridController.EntityEndProcess(); OnMoveEnded(); };
        CacheTween(moveTween);
    }

    public void CacheTween(Tween tween)
    {
        _lastTween = tween;
    }

    protected void CompleteLastTween()
    {
        if (_lastTween != null) _lastTween.Complete(true);
    }

    protected void KillLastTween()
    {
        if (_lastTween == null) return; 
        _lastTween.Kill(true);
    }

    public virtual void DestoryEntity()
    {
        poolObject.GoToPool();
        OnEntityDestroyed.Invoke();
    }    

    public virtual void OnGoToPool()
    {
        KillLastTween();
        EntityType = null;
        OnEntityDestroyed.RemoveAllListeners();
    }

    public virtual void OnPoolSpawn()
    {
        //
    }
}
