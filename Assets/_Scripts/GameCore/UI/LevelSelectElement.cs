using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectElement : MonoBehaviour
{
    [BHeader("References")]
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private Button playButton;

    private LevelConfig _config;

    public void SetupElement(LevelConfig config, LevelSaveData data, bool unlocked)
    {
        _config = config;
        levelNameText.text = config.LevelTitle;
        playButton.onClick.AddListener(OnClickPlay);

        UpdateElement(data, unlocked);
    }

    public void UpdateElement(LevelSaveData data, bool unlocked)
    {
        playButton.interactable = unlocked;

        string highScoreString;
        if (data.HighScore == 0) highScoreString = "-";
        else highScoreString = data.HighScore.ToString();
        
        highscoreText.text = highScoreString;
    }

    public void OnClickPlay()
    {
        LevelManager.Instance.CreateLevel(_config);
    }
}
