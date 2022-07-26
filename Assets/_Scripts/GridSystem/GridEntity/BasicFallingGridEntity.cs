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

    public UnityEvent<IGridEntity> OnEntityDestroyed { get; private set; } = new UnityEvent<IGridEntity>();
    public IGridEntityTypeDefinition EntityType { get; protected set; }
    public Vector2Int GridCoordinates { get; protected set; }
    public Transform EntityTransform => transform;
    
    protected GridController _gridController;
    protected Tween _lastTween = null;
    protected bool _inProcess;
    
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

    public virtual void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
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

    public virtual void OnMoveEntity(Vector2Int newCoordinates, MovementMode movementMode)
    {
        MoveToCoordinate(newCoordinates, movementMode);
    }

    protected void MoveToCoordinate(Vector2Int newCoordinates, MovementMode movementMode)
    {
        KillLastTween();
        ProcessStarted();
        GridCoordinates = newCoordinates;
        Vector2 targetPos = _gridController.GridPositions[newCoordinates.x, newCoordinates.y];
        float moveDuration = .5f;
        Tween moveTween = null;
        switch (movementMode)
        {
            case MovementMode.Linear:
                moveTween = transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutBounce);
                break;
            case MovementMode.Curvy:
                float curveAmountMultiplier = .2f;
                float distanceToTarget = Vector2.Distance((Vector2)transform.position, targetPos);
                Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
                Vector2 moveDirNormal = new Vector2(-moveDir.y, moveDir.x);
                Sequence sequenceTween = DOTween.Sequence();
                sequenceTween.Join(transform.DOBlendableMoveBy(moveDirNormal * distanceToTarget * curveAmountMultiplier, moveDuration / 2).SetEase(Ease.InOutCubic));
                sequenceTween.Append(transform.DOBlendableMoveBy(-moveDirNormal * distanceToTarget * curveAmountMultiplier, moveDuration / 2).SetEase(Ease.InOutCubic));
                sequenceTween.Insert(0, transform.DOBlendableMoveBy(targetPos - (Vector2)transform.position, moveDuration).SetEase(Ease.InOutBack));
                moveTween = sequenceTween;
                break;
            default:
                break;
        }
        moveTween.onComplete += () => { ProcessEnded(); OnMoveEnded(); };
        CacheTween(moveTween);
    }

    private void ProcessStarted()
    {
        _gridController.EntityStartProcess();
        _inProcess = true;
    }

    private void ProcessEnded()
    {
        _gridController.EntityEndProcess();
        _inProcess = false;
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
        OnEntityDestroyed.Invoke(this);
    }    

    public virtual void OnGoToPool()
    {
        if (_inProcess) ProcessEnded();
        SummonOnDestroyParticle();
        KillLastTween();
        OnEntityDestroyed.RemoveAllListeners();
        EntityType = null;
    }

    private void SummonOnDestroyParticle()
    {
        if (!EntityType.OnDestroyParticle) return; // dont do anyting if no death particles are provided
        
        GameObject particle = ObjectPooler.Instance.Spawn(EntityType.OnDestroyParticle.name, transform.position);
        particle.transform.SetParent(_gridController.GridOverlay); // move particle to grid overlay transform
        
        // scale particle to match current grid entity cell size
        float particleScale = _gridController.GridCellSpacing;
        particle.transform.localScale = new Vector2(particleScale,particleScale);
    }

    public virtual void OnPoolSpawn()
    {
        //
    }
}
