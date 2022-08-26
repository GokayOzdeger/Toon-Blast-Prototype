using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterMonitor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileImageRenderer;
    [SerializeField] private TMPro.TMP_Text characterText;

    private ITile Tile { get; set; }

    public void OnMouseUp()
    {
        Tile.OnClick();
    }

    public void UpdateMonitor(ITile tile)
    {
        Tile = tile;

        // update visiual elements
        characterText.text = tile.TileData.Character;
        if (tile.Locks == 0) tileImageRenderer.color = Color.white;
        else tileImageRenderer.color = Color.gray;
    }

    public void SetPixelSize(float size)
    {
        transform.localScale =  Vector3.one * size;
    }
}
