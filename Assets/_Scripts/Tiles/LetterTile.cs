using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterTile : ITile
{
    private LetterMonitor _monitor;
    private TileData _tileData;
    public int Locks { get; set; } = 0;
    public LetterMonitor Monitor => _monitor;
    public TileData TileData => _tileData;

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

    protected void UpdateMonitor()
    {
        if (Monitor == null) return;
        Monitor.UpdateMonitor(this);
    }

    public void OnClick()
    {
        Debug.Log("Clicked: " + TileData.Character);
        LevelController.Instance.WordController.AddTileToWord(this);
    }
}
