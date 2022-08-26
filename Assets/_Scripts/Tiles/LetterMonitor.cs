using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterMonitor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileImageRenderer;
    [SerializeField] private TMPro.TMP_Text characterText;

    public void UpdateMonitor(ITile tile)
    {
        // update visiual elements
        characterText.text = tile.LetterData.Character;
        if (tile.Locks == 0) tileImageRenderer.color = Color.white;
        else tileImageRenderer.color = Color.gray;
    }
}
