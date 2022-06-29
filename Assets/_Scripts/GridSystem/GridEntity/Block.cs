using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Block : MonoBehaviour, IGridEntity, IPoolable
{
    [SerializeField] private Image blockImage;

    public BlockTypeDefinition BlockType { get; private set; }

    public Vector2Int GridCoordinates { get; private set; }

    public List<Block> CurrentBlockGroup { get; set; }

    public int GroupSize { get { return CurrentBlockGroup.Count; } }

    public bool EntityNeedsUpdate { get; set; } = true;

    private Tweener _lastTween = null;
    protected GridController _gridController;

    public void AnimateBlockPunchScale(Action onComplete = null)
    {
        CompleteLastTween();
        _lastTween = transform.DOPunchScale(new Vector2(.15f, .15f), .2f);
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
        if (_lastTween != null) _lastTween.Kill();
    }
    
    public void CacheTween(Tweener tween)
    {
        _lastTween = tween;
    }

    public void SetupEntity(GridController grid, BlockTypeDefinition blockType)
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

    public void OnClickBlock()
    {
        TryExplode();
    }

    public void TryExplode()
    {
        if (_gridController.GridInAction) return;
        if (CurrentBlockGroup.Count < 2) ExplodeFail();
        else ExplodeStart();
    }

    private void ExplodeStart()
    {
        GridInAction = true;

        RemoveEntitiesFromGridArray(blockToExplode.CurrentBlockGroup);

        foreach (Block block in blockToExplode.CurrentBlockGroup)
        {
            block.AnimateBlockPunchScale();
            _blockSpawner.AddBlockSpawnReqeust(block.GridCoordinates.y);
        }
    }

    private void ExplosionEnd()
    {
        CallCachedChanges();
    }

    private void ExplodeFail()
    {
        AnimateBlockShake();
    }

    protected void StartExplode()
    {
        foreach (Block block in CurrentBlockGroup)
        {
            block.AnimateBlockPunchScale(()=> Destroy(block.gameObject));
        }
        _gridController.ex
    }

    public void OnGridChange(Vector2Int changeCoordinate)
    {
        if (GridCoordinates - changeCoordinate != new Vector2Int(1, 0)) return;
        if(_gridController.EntityGrid[changeCoordinate.x, changeCoordinate.y] != null) return;
        Debug.Log("Fall Block " + gameObject.name,gameObject);
        _gridController.WriteEntityFall(this);
    }

    public void OnUpdateEntity()
    {
        if (!EntityNeedsUpdate) return;
        List<Block> blockGroup = new List<Block>();
        _gridController.CollectMatchingSurroundingEntities<Block>(this, ref blockGroup);

        int groupSize = blockGroup.Count;
        Sprite blockImageForAllGroup = BlockType.GetBlockGroupIcon(blockGroup);

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
        GridCoordinates = newCoordinates;
        float targetYPos = _gridController.GridPositions[newCoordinates.x, newCoordinates.y].y;
        float distanceToTarget = transform.position.y - targetYPos;
        Debug.Log("Distance:" + distanceToTarget);
        float moveDuration = .8f / Mathf.Pow(distanceToTarget / _gridController.RowSpacing, 1 / 3f);
        Tweener moveTween = transform.DOMoveY(targetYPos, moveDuration);
        CacheTween(moveTween);
    }

    public void OnGoToPool()
    {
        KillLastTween();
        EntityNeedsUpdate = true;
        CurrentBlockGroup = null;
        BlockType = null;
    }

    public void OnPoolSpawn()
    {
        
    }
}
