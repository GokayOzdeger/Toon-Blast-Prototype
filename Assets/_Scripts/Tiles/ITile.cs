using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    public LetterMonitor Monitor { get; }
    public TileData TileData { get; }
    public int Locks { get; set; }

    public void OnClick();
    public void LockTile();
    public void UnlockTile();
    public void SetPixelSize(float size);
}
