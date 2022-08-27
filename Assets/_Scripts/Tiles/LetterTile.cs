using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterTile : ITile
{
    private LetterMonitor _monitor;
    private TileData _tileData;
    private Tween _activeTween;


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
    public LetterMonitor Monitor => _monitor;
    public TileData TileData => _tileData;
    public bool Clickable { get; private set; } = true;

    public LetterTile(LetterMonitor monitor, TileData data)
    {
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
        Monitor.SetPixelSize(size);
    }

    public void UpdateMonitor()
    {
        if (Monitor == null) return;
        Monitor.UpdateMonitor(this);
    }

    public void OnClick()
    {
        if (LevelController.Instance.WordController.WordIsFull) return;
        if (!Clickable) return;
        if (Locks != 0) return;
        else OnClickSuccess();
    }

    private void OnClickSuccess()
    {
        Clickable = false;
        LevelController.Instance.WordController.AddTileToWord(this);
    }

    public void UnlockChildren()
    {
        foreach (int childId in TileData.Children) LevelController.Instance.TileManager.GetTileWithId(childId).UnlockTile();
    }

    private void LockChildren()
    {
        foreach(int childId in TileData.Children) LevelController.Instance.TileManager.GetTileWithId(childId).LockTile();
    }

    public void ReturnToTileArea()
    {
        Sequence tween = DOTween.Sequence();
        tween.Append(TweenHelper.Shake(Monitor.transform, null, 20, .25f));
        tween.Append(TweenHelper.CurvingMoveTo(Monitor.transform, TileData.Position, OnReturnedToTileArea));
        ActiveTween = tween;
        LockChildren();
    }

    private void OnReturnedToTileArea()
    {
        Clickable = true;
    }

    public void LeaveTileArea(Vector3 moveTo, Action onComplete)
    {
        ActiveTween = TweenHelper.CurvingMoveTo(Monitor.transform, moveTo, onComplete, .3f);
        UnlockChildren();
    }

    public void GoToPool(float delay)
    {
        Monitor.SendToPool(delay);
    }
}
