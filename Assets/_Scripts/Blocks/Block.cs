using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Block : MonoBehaviour, IGridEntity
{
    [SerializeField] private Image blockImage;

    public BlockTypeDefinition BlockType { get; private set; }

    public Vector2Int GridCoordinates { get; private set; }

    public List<Block> CurrentBlockGroup { get; set; }

    public int GroupSize { get { return CurrentBlockGroup.Count; } }

    public bool EntityNeedsUpdate { get; set; } = true;

    private Tweener _lastTween = null;

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

    public void OnClickBlock()
    {
        GridManager.Instance.TryExplode(this);
    }

    public void OnGridChange(Vector2Int changeCoordinate)
    {
        if (GridCoordinates - changeCoordinate != new Vector2Int(1, 0)) return;
        if(GridManager.Instance.EntityGrid[changeCoordinate.x, changeCoordinate.y] != null) return;
        Debug.Log("Fall Block " + gameObject.name,gameObject);
        GridManager.Instance.WriteEntityFall(this);
    }
    private bool moved = false;

    public void OnUpdateEntity()
    {
        if (!EntityNeedsUpdate) return;
        List<Block> blockGroup = new List<Block>();
        GridManager.Instance.CollectMatchingSurroundingEntities<Block>(this, ref blockGroup);

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
        float targetYPos = GridManager.Instance.GridPositions[newCoordinates.x, newCoordinates.y].y;
        float distanceToTarget = transform.position.y - targetYPos;
        Debug.Log("Distance:" + distanceToTarget);
        float moveDuration = .8f / Mathf.Pow(distanceToTarget / GridManager.Instance.RowSpacing, 1 / 3f);
        Tweener moveTween = transform.DOMoveY(targetYPos, moveDuration);
        CacheTween(moveTween);
    }
}
