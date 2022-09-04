using System;
using DG.Tweening;
using UnityEngine;

public class LetterTile : ITile
{
    private const float FLY_TO_WORDFORMER_DURATION = .3f;
    private const float FLY_TO_TILEAREA_DURATION = .15f;
    private readonly TileController _tileController;

    private readonly WordController _wordController;
    private Tween _activeTween;
    private ITile[] _childrenTiles;

    public LetterTile(TileController tileController, WordController wordController, LetterMonitor monitor,
        TileData data)
    {
        _wordController = wordController;
        _tileController = tileController;
        Monitor = monitor;
        TileData = data;
        if (monitor == null) UsingMonitor = false;

        UpdateMonitor();
    }

    public bool InTileArea { get; private set; } = true;
    public bool UsingMonitor { get; set; } = true;
    public LetterMonitor Monitor { get; }

    public TileData TileData { get; }

    public int Locks { get; set; }
    public bool Clickable => Locks == 0 && InTileArea;
    public bool IsRemovedFromPlay { get; private set; }

    public ITile[] ChildrenTiles
    {
        get
        {
            if (_childrenTiles == null)
            {
                _childrenTiles = new ITile[TileData.Children.Length];
                for (var i = 0; i < TileData.Children.Length; i++)
                    _childrenTiles[i] = _tileController.GetTileWithId(TileData.Children[i]);
            }

            return _childrenTiles;
        }
    }

    public void LockTile()
    {
        if (IsRemovedFromPlay) return;
        if (!InTileArea) return;
        Locks++;
        if (Locks == 1) UpdateMonitor();
    }

    public void UnlockTile()
    {
        if (IsRemovedFromPlay) return;
        if (!InTileArea) return;
        Locks--;
        if (Locks == 0) UpdateMonitor();
    }

    public void SetPixelSize(float size)
    {
        if (!UsingMonitor) return;
        Monitor.SetPixelSize(size);
    }

    public void UpdateMonitor()
    {
        if (!UsingMonitor) return;
        Monitor.UpdateMonitor(this);
    }

    public void OnClick()
    {
        if (_wordController.WordIsFull) return;
        if (!Clickable) return;
        if (Locks != 0) return;
        OnClickSuccess();
    }

    public void UnlockChildren()
    {
        foreach (ITile tile in ChildrenTiles) tile.UnlockTile();
    }

    public void LockChildren()
    {
        foreach (ITile tile in ChildrenTiles) tile.LockTile();
    }

    public void ReturnToTileArea(Action onComplete)
    {
        InTileArea = true;
        if (UsingMonitor)
        {
            CompleteLastTween();
            Sequence tween = DOTween.Sequence();
            tween.Append(TweenHelper.Shake(Monitor.transform, null, 20, FLY_TO_TILEAREA_DURATION));
            tween.Append(TweenHelper.CurvingMoveTo(Monitor.transform, TileData.Position));
            if (onComplete != null) tween.onComplete = () => { onComplete(); };
            _activeTween = tween;
        }
    }

    public void LeaveTileArea(Vector3 moveTo, Action onComplete)
    {
        InTileArea = false;
        UnlockChildren();
        if (UsingMonitor)
        {
            CompleteLastTween();
            _activeTween = TweenHelper.CurvingMoveTo(Monitor.transform, moveTo, onComplete, FLY_TO_WORDFORMER_DURATION);
        }
    }

    public void RemoveFromPlay()
    {
        IsRemovedFromPlay = true;
    }

    public void RemoveVisiuals()
    {
        if (!UsingMonitor) return;
        CompleteLastTween();
        Monitor.SendToPool(0);
    }

    private void OnClickSuccess()
    {
        if (UsingMonitor) _wordController.AddTileToWord(this);
        else _wordController.AddTileToWordAutoSolver(this);
    }

    private void CompleteLastTween()
    {
        if (_activeTween != null) _activeTween.Complete();
    }
}