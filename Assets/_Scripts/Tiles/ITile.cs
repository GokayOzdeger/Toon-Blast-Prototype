using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    public LetterMonitor Monitor { get; }
    public TileData TileData { get; }
    public int Locks { get; set; }

    public void ReturnToTileArea();
    public void LeaveTileArea(Vector3 moveTo, Action onComplete);
    public void OnClick();
    public void LockTile();
    public void UnlockTile();
    public void UpdateMonitor();
    public void SetPixelSize(float size);
}
