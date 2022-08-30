using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreController
{
    public ScoreControllerReferences References { get; set; }
    public ScoreControllerSettings Settings { get; set; }
    public bool IsNewHighScore => CurrentTotalScore >= LevelSaveData.Data(_levelTitle).HighScore;
    public int CurrentTotalScore { get; private set; }

    private int _currentWordScore;
    private string _levelTitle;

    public ScoreController(ScoreControllerReferences references, ScoreControllerSettings settings)
    {
        References = references;
        Settings = settings;
    }
    
    public void SetupScoreController(string levelTitle)
    {
        _levelTitle = levelTitle;
        HideWordScore();
        UpdateTotalScoreDisplay();
    }
    
    public void DisplayScoreForWord(string word)
    {
        if (word == "") { HideWordScore(); return; }
        ShowWordScore();
        int _currentRawWordScore = 0;
        foreach (char character in word) _currentRawWordScore += GetWordScore(character);
        _currentWordScore = _currentRawWordScore * word.Length * 10;
        References.wordScoreText.text = Settings.wordScoreText + " " + _currentWordScore.ToString();
    }
    public void UpdateTotalScoreDisplay()
    {
        References.totalScoreText.text = Settings.totalScoreText + " " + CurrentTotalScore.ToString();
    }

    public void WordSubmitted()
    {
        CurrentTotalScore += _currentWordScore;
        HideWordScore();
        UpdateTotalScoreDisplay();
    }

    private void ShowWordScore()
    {
        References.wordScoreCanvasGroup.alpha = 1;
    }
    private void HideWordScore()
    {
        References.wordScoreCanvasGroup.alpha = 0;
    }

    private int GetWordScore(char character)
    {
        foreach(var pair in Settings.CharacterScores)
        {
            if (Equals(pair.Key, character)) return pair.Value;
        }
        Debug.LogError($"Can't find '{character}' in score list !");
        return 0;
    }

    #region EDITOR

    public void SetCurrentScoreToHighScore()
    {
        _currentWordScore = LevelSaveData.Data(_levelTitle).HighScore;
    }

    #endregion
}

[System.Serializable]
public class ScoreControllerReferences
{
    public CanvasGroup wordScoreCanvasGroup;
    public TMP_Text wordScoreText;
    public TMP_Text totalScoreText;
}

[System.Serializable]
public class ScoreControllerSettings
{
    public string totalScoreText;
    public string wordScoreText;

    public Dictionary<char, int> CharacterScores = new Dictionary<char, int>()
    {
        {'a',1 },
        {'e',1 },
        {'o',1 },
        {'n',1 },
        {'r',1 },
        {'t',1 },
        {'l',1 },
        {'s',1 },
        {'u',1 },
        {'i',1 },
        {'d',2 },
        {'g',2 },
        {'b',3 },
        {'c',3 },
        {'m',3 },
        {'p',3 },
        {'f',4 },
        {'h',4 },
        {'v',1 },
        {'w',4 },
        {'y',4 },
        {'k',5 },
        {'j',8 },
        {'x',8 },
        {'q',10 },
        {'z',10 },
    };
}

