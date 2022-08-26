using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterTile : ITile
{
    private LetterMonitor _monitor;
    private TileData _letterData;
    public int Locks { get; set; } = 0;
    public LetterMonitor Monitor => _monitor;
    public TileData LetterData => _letterData;

    public LetterTile(LetterMonitor monitor, TileData data)
    {
        _monitor = monitor;
        _letterData = data;
        UpdateMonitor();
    }

    public void LockTile()
    {
        Locks++;
    }

    public void UnlockTile()
    {
        Locks--;
        if (Locks == 0) UpdateMonitor();
    }

    protected void UpdateMonitor()
    {
        if (Monitor == null) return;
        Monitor.UpdateMonitor(this);
    }
}
