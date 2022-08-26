using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordController
{
    private WordControllerReferences References { get; set; }
    private WordControllerSettings Settings { get; set; }
    private WordControllerConfig Config { get; set; }

    private int _nextLetterIndex = 0;
    private string _currentWord = "";
    private List<string> submittedWords = new List<string>();
    private List<Vector3> letterPositions = new List<Vector3>();

    public WordController(WordControllerReferences references, WordControllerSettings settings, WordControllerConfig config)
    {
        References = references;
        Settings = settings;
        Config = config;

        References.submitWordButton.onClick.AddListener(OnClickSubmitWord);
        References.undoButton.onClick.AddListener(OnClickUndoButton);
    }

    public void SetupWordController(TileController tileController)
    {
        ResizeWordFormingArea(tileController);
    }

    public void AddTileToWord(ITile tile)
    {
        if (_nextLetterIndex == Settings.maxLetterCount) return;
        _currentWord += tile.TileData.Character;
        Vector2 positionToMoveTo = letterPositions[_nextLetterIndex];
        TweenHelper.CurvingMoveTo(tile.Monitor.transform, new Vector3(positionToMoveTo.x, positionToMoveTo.y, tile.TileData.Position.z), OnLetterReachedWordArea, .3f);
        _nextLetterIndex++;
    }

    private void ResizeWordFormingArea(TileController tileController)
    {
        Debug.Log("wordFormingPos: " + References.wordFormingAreaTransform.position);
        float tileSize = tileController.TileSize;
        References.wordFormingAreaRect.sizeDelta = new Vector2(tileSize * Settings.maxLetterCount + Settings.wordFormingAreaExtents.x, tileSize + Settings.wordFormingAreaExtents.y);

        // cache letter positions
        Vector3 wordFormingAreaPosition = References.wordFormingAreaTransform.position;
        Debug.Log(References.wordFormingAreaTransform.position);
        Debug.Log("wordFormingPos: "+wordFormingAreaPosition.y);
        float currentLetterX = 0;
        for (int i = 0; i < Settings.maxLetterCount; i++)
        {
            currentLetterX += tileSize;
            letterPositions.Add(new Vector2(currentLetterX - (tileSize * Settings.maxLetterCount / 2f), wordFormingAreaPosition.y));
        }
    }

    private void CheckWordFilled()
    {
        if (_nextLetterIndex != Settings.maxLetterCount) return;

        // return all letters to board
    }

    private void OnLetterReachedWordArea()
    {
        if (!IsWordValid()) 
        {
            CheckWordFilled();
        }
    }

    private void OnClickSubmitWord()
    {
        SubmitWord();
    }

    private void OnClickUndoButton()
    {
        
    }

    private bool IsWordValid()
    {
        if (!Config.possibleWords.Contains(_currentWord)) return false;
        if (submittedWords.Contains(_currentWord)) return false;
        return true;
    }

    private void SubmitWord()
    {
        submittedWords.Add(_currentWord);
        ResetWord();
    }

    private void ResetWord()
    {
        _currentWord = "";
        _nextLetterIndex = 0;
    }
}

[System.Serializable]
public class WordControllerReferences
{
    public RectTransform wordFormingAreaRect;
    public Transform wordFormingAreaTransform;
    public Button submitWordButton;
    public Button undoButton;
}

[System.Serializable]
public class WordControllerSettings
{
    public Vector2 wordFormingAreaExtents;
    public int maxLetterCount = 7;
}

[System.Serializable]
public class WordControllerConfig
{
    public List<string> possibleWords;
}
