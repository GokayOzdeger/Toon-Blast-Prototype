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

    public bool Clickable { get; private set; } = true;

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
        Locks++;
        if (Locks == 1) UpdateMonitor();
    }

    public void UnlockTile()
    {
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
        Clickable = false;
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
        LockChildren();
        if (Monitor)
        {
            CompleteLastTween();
            Sequence tween = DOTween.Sequence();
            tween.Append(TweenHelper.Shake(Monitor.transform, null, 20, FLY_TO_TILEAREA_DURATION));
            tween.Append(TweenHelper.CurvingMoveTo(Monitor.transform, TileData.Position));
            tween.onComplete = () => { if(onComplete != null) onComplete(); OnReturnedToTileArea(); };
            _activeTween = tween;
        }
        else OnReturnedToTileArea(); // movements are instant when not using a monitor
    }

    private void OnReturnedToTileArea()
    {
        Clickable = true;
    }

    private void CompleteLastTween()
    {
        if (_activeTween != null) _activeTween.Complete();
    }

    public void LeaveTileArea(Vector3 moveTo, Action onComplete)
    {
        UnlockChildren();
        if (Monitor)
        {
            CompleteLastTween();
            _activeTween = TweenHelper.CurvingMoveTo(Monitor.transform, moveTo, onComplete, FLY_TO_WORDFORMER_DURATION);
        }
        else onComplete(); // movements are instant when not using a monitor
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
