using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    public LetterMonitor Monitor { get; }
    public TileData LetterData { get; }
    public int Locks { get; set; }

    public void LockTile();
    public void UnlockTile();
}
