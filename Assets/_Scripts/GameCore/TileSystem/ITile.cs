using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    public LetterMonitor Monitor { get; }
    public TileData TileData { get; }
    public int Locks { get; set; }
    public bool Clickable { get; }
    public bool IsRemovedFromPlay { get; }
    public ITile[] ChildrenTiles { get; }

    public void ReturnToTileArea(Action onComplete);
    public void LeaveTileArea(Vector3 moveTo, Action onComplete);
    public void OnClick();
    public void LockTile();
    public void UnlockTile();
    public void LockChildren();
    public void UnlockChildren();
    public void UpdateMonitor();
    public void RemoveFromPlay();
    public void RemoveVisiuals();
    public void SetPixelSize(float size);
}
