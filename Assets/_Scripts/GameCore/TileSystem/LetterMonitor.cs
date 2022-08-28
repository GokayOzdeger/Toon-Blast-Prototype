using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class LetterMonitor : MonoBehaviour, IPoolable
{
    [BHeader("References")] 
    [SerializeField] private PoolObject poolObject;
    [SerializeField] private SpriteRenderer tileImageRenderer;
    [SerializeField] private TMPro.TMP_Text characterText;
    [BHeader("Settings")]
    [SerializeField] private float holdDurationForUndoAll;
    private ITile Tile { get; set; }

    public void OnMouseUpAsButton()
    {
        Tile.OnClick();
    }

    public void UpdateMonitor(ITile tile)
    {
        Tile = tile;

        // update visiual elements
        characterText.text = tile.TileData.Character;
        if (tile.Locks == 0) SetTileUnlocked();
        else SetTileLocked();
    }

    public void SetPixelSize(float size)
    {
        transform.localScale =  Vector3.one * size;
    }

    public void SendToPool(float delay)
    {
        poolObject.GoToPool(delay);
    }

    public void OnGoToPool()
    {
        Tile = null;
    }

    public void OnPoolSpawn()
    {
        //
    }

    private void SetTileLocked()
    {
        tileImageRenderer.color = Color.gray;
    }

    private void SetTileUnlocked()
    {
        tileImageRenderer.color = Color.white;
        TweenHelper.PunchScale(tileImageRenderer.transform);
    }
}
