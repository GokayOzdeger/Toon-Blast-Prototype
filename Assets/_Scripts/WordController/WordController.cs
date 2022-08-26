using DG.Tweening;
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
    private List<ITile> tilesInWordFormer = new List<ITile>();

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
        tilesInWordFormer.Add(tile);

        Vector2 positionToMoveTo2D = letterPositions[_nextLetterIndex];
        Vector3 positionToMoveTo3D = new Vector3(positionToMoveTo2D.x, positionToMoveTo2D.y, tile.TileData.Position.z);
        tile.LeaveTileArea(positionToMoveTo3D, OnLetterReachedWordArea);
        _nextLetterIndex++;
    }

    private void ResizeWordFormingArea(TileController tileController)
    {
        float tileSize = tileController.TileSize;
        References.wordFormingAreaRect.sizeDelta = new Vector2(tileSize * Settings.maxLetterCount + Settings.wordFormingAreaExtents.x, tileSize + Settings.wordFormingAreaExtents.y) / References.canvasScaler.transform.lossyScale.x;

        // cache letter positions
        Vector3 wordFormingAreaPosition = References.wordFormingAreaRect.position;
        float currentLetterX = 0;
        for (int i = 0; i < Settings.maxLetterCount; i++)
        {
            currentLetterX += tileSize;
            letterPositions.Add(new Vector2(currentLetterX - (tileSize * (Settings.maxLetterCount+1) / 2f), wordFormingAreaPosition.y));
        }
    }

    private void CheckWordFilled()
    {
        if (_nextLetterIndex != Settings.maxLetterCount) return;
        ResetWord();
    }

    private void OnLetterReachedWordArea()
    {
        if (!IsWordValid())
        {
            SetSubmitButtonState(false);
            CheckWordFilled();
        }
        else SetSubmitButtonState(true);
    }

    private void SetSubmitButtonState(bool active)
    {
        References.submitWordButton.interactable = active;
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
        foreach (ITile tile in tilesInWordFormer) tile.ReturnToTileArea();
        foreach (ITile tile in tilesInWordFormer) tile.UpdateMonitor();
        tilesInWordFormer.Clear();

        _currentWord = "";
        _nextLetterIndex = 0;
    }
}

[System.Serializable]
public class WordControllerReferences
{
    public CanvasScaler canvasScaler;
    public RectTransform wordFormingAreaRect;
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
