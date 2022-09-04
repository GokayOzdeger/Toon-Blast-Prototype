using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController
{
    private int _currentWordScore;
    private string _levelTitle;

    public ScoreController(ScoreControllerReferences references, ScoreControllerSettings settings)
    {
        References = references;
        Settings = settings;
    }

    public ScoreControllerReferences References { get; set; }
    public ScoreControllerSettings Settings { get; set; }
    public bool IsNewHighScore => CurrentTotalScore >= LevelSaveData.Data(_levelTitle).HighScore;
    public int CurrentTotalScore { get; private set; }

    public void SetupScoreController(string levelTitle)
    {
        _levelTitle = levelTitle;
        HideWordScore();
        UpdateTotalScoreDisplay();
    }

    public void LoadScoreController(string levelTitle, int currentTotalScore)
    {
        _levelTitle = levelTitle;
        CurrentTotalScore = currentTotalScore;
        HideWordScore();
        UpdateTotalScoreDisplay();
    }

    public void ClearScoreController()
    {
        if (IsNewHighScore) References.highScoreText.SetHighScoreText(CurrentTotalScore);
    }

    public void DisplayScoreForWord(string word)
    {
        if (word == "")
        {
            HideWordScore();
            return;
        }

        ShowWordScore();
        var _currentRawWordScore = 0;
        foreach (char character in word) _currentRawWordScore += GetWordScore(character);
        _currentWordScore = _currentRawWordScore * word.Length * 10;
        References.wordScoreText.text = Settings.wordScoreText + " " + _currentWordScore;
    }

    public void UpdateTotalScoreDisplay()
    {
        References.totalScoreText.text = Settings.totalScoreText + " " + CurrentTotalScore;
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
        foreach (KeyValuePair<char, int> pair in Settings.CharacterScores)
            if (Equals(pair.Key, character))
                return pair.Value;
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

[Serializable]
public class ScoreControllerReferences
{
    public CanvasGroup wordScoreCanvasGroup;
    public TMP_Text wordScoreText;
    public TMP_Text totalScoreText;
    public HighScoreText highScoreText;
}

[Serializable]
public class ScoreControllerSettings
{
    public string totalScoreText;
    public string wordScoreText;

    public Dictionary<char, int> CharacterScores = new()
    {
        { 'a', 1 },
        { 'e', 1 },
        { 'o', 1 },
        { 'n', 1 },
        { 'r', 1 },
        { 't', 1 },
        { 'l', 1 },
        { 's', 1 },
        { 'u', 1 },
        { 'i', 1 },
        { 'd', 2 },
        { 'g', 2 },
        { 'b', 3 },
        { 'c', 3 },
        { 'm', 3 },
        { 'p', 3 },
        { 'f', 4 },
        { 'h', 4 },
        { 'v', 1 },
        { 'w', 4 },
        { 'y', 4 },
        { 'k', 5 },
        { 'j', 8 },
        { 'x', 8 },
        { 'q', 10 },
        { 'z', 10 }
    };
}