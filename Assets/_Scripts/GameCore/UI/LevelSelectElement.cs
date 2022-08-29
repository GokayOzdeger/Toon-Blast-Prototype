using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectElement : MonoBehaviour
{
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private Button playButton;

    private LevelConfig _config;

    public void SetupElement(LevelConfig config)
    {
        _config = config;
        playButton.onClick.AddListener(OnClickPlay);
        levelNameText.text = config.LevelTitle;
        //highscoreText.text = config.
    }

    public void OnClickPlay()
    {
        LevelManager.Instance.CreateLevel(_config);
    }
}
