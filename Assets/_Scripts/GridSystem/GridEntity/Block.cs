using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Utilities;

public class Block : MonoBehaviour, IGridEntity, IPoolable
{
    private readonly float DestroyAnimationDuration = .2f;
    
    [SerializeField] private Image blockImage;
    [SerializeField] private PoolObject poolObject;
    public IGridEntityTypeDefinition EntityType { get; private set; }

    public Vector2Int GridCoordinates { get; private set; }

    public List<Block> CurrentBlockGroup { get; set; }

    public int GroupSize { get { return CurrentBlockGroup.Count; } }

    public bool EntityNeedsUpdate { get; set; } = true;

    private Tweener _lastTween = null;
    private GridController _gridController;

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

    private void CompleteLastTween()
    {
        if (_lastTween != null) _lastTween.Complete(true);
    }
    
    private void KillLastTween()
    {
        if (_lastTween != null) _lastTween.Kill(true);
    }
    
    public void CacheTween(Tweener tween)
    {
        _lastTween = tween;
    }

    public void SetupEntity(GridController grid, IGridEntityTypeDefinition blockType)
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
        blockImage.sprite = sprite;
    }

    public void OnClickBlock()
    {
        TryExplode();
    }

    public void TryExplode()
    {
        if (!_gridController.GridInterractable) return;
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

    public void OnGridChange(Vector2Int changeCoordinate)
    {
        if (GridCoordinates - changeCoordinate != new Vector2Int(1, 0)) return;
        if(_gridController.EntityGrid[changeCoordinate.x, changeCoordinate.y] != null) return;
        //entity reacting to grid change
        _gridController.WriteEntityFall(this);
    }

    public void OnUpdateEntity()
    {
        if (!EntityNeedsUpdate) return;
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

    public void OnMoveEntity(Vector2Int newCoordinates)
    {
        KillLastTween();
        _gridController.EntityStartProcess();
        GridCoordinates = newCoordinates;
        Vector2 targetPos = _gridController.GridPositions[newCoordinates.x, newCoordinates.y];
        float distanceToTarget = Vector2.Distance((Vector2)transform.position, targetPos);
        Debug.Log("Distance:" + distanceToTarget);
        float moveDuration = .8f / Mathf.Pow(distanceToTarget / _gridController.RowSpacing, 1 / 3f);
        Tweener moveTween = transform.DOMove(targetPos, moveDuration);
        moveTween.onComplete += () => _gridController.EntityEndProcess();
        CacheTween(moveTween);
    }

    public void OnGoToPool()
    {
        KillLastTween();
        EntityNeedsUpdate = true;
        CurrentBlockGroup = null;
        EntityType = null;
    }

    public void OnPoolSpawn()
    {
        
    }

    public void DestoryEntityWithCallback(Action onDestroy)
    {
        AnimateBlockShake(onDestroy);
        poolObject.GoToPool(DestroyAnimationDuration);        
    }
}
