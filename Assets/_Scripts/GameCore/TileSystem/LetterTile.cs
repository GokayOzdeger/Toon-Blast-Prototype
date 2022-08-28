using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterTile : ITile
{
    private TileController _tileController;
    private WordController _wordController;
    private LetterMonitor _monitor;
    private TileData _tileData;
    private Tween _activeTween;
    private ITile[] _childrenTiles;

    public Tween ActiveTween
    {
        get => _activeTween;

        set
        {
            if (_activeTween != null) _activeTween.Complete(true);
            _activeTween = value;
        }
    }
    public int Locks { get; set; } = 0;
    public bool Clickable => Locks == 0 && InTileArea;
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

    public bool InTileArea { get; private set; } = true;

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
        if (!InTileArea) return;
        Locks++;
        if (Locks == 1) UpdateMonitor();
    }

    public void UnlockTile()
    {
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
        else OnClickSuccess();
    }

    private void OnClickSuccess()
    {
        InTileArea = false;
        _wordController.AddTileToWord(this);
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
        if (Monitor)
        {
            Sequence tween = DOTween.Sequence();
            tween.Append(TweenHelper.Shake(Monitor.transform, null, 20, .15f));
            tween.Append(TweenHelper.CurvingMoveTo(Monitor.transform, TileData.Position));
            tween.onComplete = () => { if(onComplete != null) onComplete(); OnReturnedToTileArea(); };
            ActiveTween = tween;
        }
        else OnReturnedToTileArea(); // movements are instant when not using a monitor
    }

    private void OnReturnedToTileArea()
    {
        InTileArea = true;
    }

    public void LeaveTileArea(Vector3 moveTo, Action onComplete)
    {
        if (Monitor)
        {
            ActiveTween = TweenHelper.CurvingMoveTo(Monitor.transform, moveTo, onComplete, .3f);
        }
        else onComplete(); // movements are instant when not using a monitor

        UnlockChildren();
    }

    public void GoToPool()
    {
        Monitor?.SendToPool(0);
    }

    public void GoToPool(float delay)
    {
        Monitor?.SendToPool(delay);
    }
}
