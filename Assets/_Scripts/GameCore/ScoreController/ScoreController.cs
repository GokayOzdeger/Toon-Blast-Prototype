using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreController
{
    public ScoreControllerReferences References { get; set; }
    public ScoreControllerSettings Settings { get; set; }

    private int _currentWordScore;
    private int _currentTotalScore;

    public ScoreController(ScoreControllerReferences references, ScoreControllerSettings settings)
    {
        References = references;
        Settings = settings;
    }
    
    public void SetupScoreController()
    {
        HideWordScore();
        UpdateTotalScoreDisplay();
    }
    
    public void DisplayScoreForWord(string word)
    {
        if (word == "") return;
        ShowWordScore();
        _currentWordScore = 0;
        foreach (char character in word) _currentWordScore += GetWordScore(character);
        References.wordScoreText.text = Settings.wordScoreText + " " + _currentWordScore.ToString();
    }
    public void UpdateTotalScoreDisplay()
    {
        References.totalScoreText.text = Settings.totalScoreText + " " + _currentTotalScore.ToString();
    }

    public void WordSubmitted()
    {
        _currentTotalScore += _currentWordScore;
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
            if (pair.Key == character) return pair.Value;
        }
        Debug.LogError($"Can't find '{character}' in score list !");
        return 0;
    }
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
        {'A',1 },
        {'E',1 },
        {'O',1 },
        {'N',1 },
        {'R',1 },
        {'T',1 },
        {'L',1 },
        {'S',1 },
        {'U',1 },
        {'D',2 },
        {'G',2 },
        {'B',3 },
        {'C',3 },
        {'M',3 },
        {'P',3 },
        {'F',4 },
        {'H',4 },
        {'V',1 },
        {'W',4 },
        {'Y',4 },
        {'K',5 },
        {'J',8 },
        {'X',8 },
        {'Q',10 },
        {'Z',10 },
    };
}

