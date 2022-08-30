using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreText : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;

    public void SetHighScoreText(int highScore)
    {
        highScoreText.text = highScore.ToString();
    }
}
