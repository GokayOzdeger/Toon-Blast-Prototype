using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterTile : ITile
{
    private const float FLY_TO_WORDFORMER_DURATION = .3f;
    private const float FLY_TO_TILEAREA_DURATION = .15f;

    private WordController _wordController;
    private TileController _tileController;
    private LetterMonitor _monitor;
    private TileData _tileData;
    private Tween _activeTween;
    private ITile[] _childrenTiles;
    private bool _removedFromPlay;

    public int Locks { get; set; } = 0;
    public LetterMonitor Monitor => _monitor;
    public TileData TileData => _tileData;

    public ITile[] ChildrenTiles
    {
        get
        {
            if(_childrenTiles == null)
            {
                _childrenTiles = new ITile[TileData.Children.Length];
                for (int i = 0; i < TileData.Children.Length; i++)
                {
                    _childrenTiles[i] = _tileController.GetTileWithId(TileData.Children[i]);
                }
            }
            return _childrenTiles;
        }
    }
    public bool Clickable => Locks == 0 && InTileArea;
    public bool InTileArea { get; private set; } = true;

    public bool IsRemovedFromPlay => _removedFromPlay;

    public LetterTile(TileController tileController, WordController wordController, LetterMonitor monitor, TileData data)
    {
        _wordController = wordController;
        _tileController = tileController;
        _monitor = monitor;
        _tileData = data;
        UpdateMonitor();
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
        if (Monitor == null) return;
        Monitor.SetPixelSize(size);
    }

    public void UpdateMonitor()
    {
        if (Monitor == null) return;
        Monitor.UpdateMonitor(this);
    }

    public void OnClick()
    {
        if (_wordController.WordIsFull) return;
        if (!Clickable) return;
        if (Locks != 0) return;
        else OnClickSuccess();
    }

    private void OnClickSuccess()
    {
        if(Monitor != null) _wordController.AddTileToWord(this);
        else _wordController.AddTileToWordAutoSolver(this);
    }

    public void UnlockChildren()
    {
        foreach (ITile tile in ChildrenTiles) tile.UnlockTile();
    }

    public void LockChildren()
    {
        foreach(ITile tile in ChildrenTiles) tile.LockTile();
    }

    public void ReturnToTileArea(Action onComplete)
    {
        InTileArea = true;
        if (Monitor)
        {
            CompleteLastTween();
            Sequence tween = DOTween.Sequence();
            tween.Append(TweenHelper.Shake(Monitor.transform, null, 20, FLY_TO_TILEAREA_DURATION));
            tween.Append(TweenHelper.CurvingMoveTo(Monitor.transform, TileData.Position));
            if (onComplete != null) tween.onComplete = () => { onComplete(); };
            _activeTween = tween;
        }
    }

    private void CompleteLastTween()
    {
        if (_activeTween != null) _activeTween.Complete();
    }

    public void LeaveTileArea(Vector3 moveTo, Action onComplete)
    {
        InTileArea = false;
        UnlockChildren();
        if (Monitor)
        {
            CompleteLastTween();
            _activeTween = TweenHelper.CurvingMoveTo(Monitor.transform, moveTo, onComplete, FLY_TO_WORDFORMER_DURATION);
        }
    }

    public void RemoveFromPlay()
    {
        _removedFromPlay = true;
    }

    public void RemoveVisiuals()
    {
        Monitor?.SendToPool(0);
    }
}
