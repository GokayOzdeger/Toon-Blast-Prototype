using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordController
{
    public bool WordIsFull => _nextLetterIndex == Settings.maxLetterCount;
    private WordControllerReferences References { get; set; }
    private WordControllerSettings Settings { get; set; }
    private WordControllerConfig Config { get; set; }

    private TileController _tileController;
    private ScoreController _scoreController;
    private int _tilesInMovement = 0;
    private int _nextLetterIndex = 0;
    private string _currentWord = "";
    private List<string> _submittedWords = new List<string>();
    private List<Vector3> _letterPositions = new List<Vector3>();
    private List<ITile> tilesInWordFormer = new List<ITile>();

    public WordController(WordControllerReferences references, WordControllerSettings settings, WordControllerConfig config)
    {
        References = references;
        Settings = settings;
        Config = config;

        References.submitWordButton.onClick.AddListener(OnClickSubmitWord);
        References.undoButton.onClick.AddListener(OnClickUndoButton);
        references.undoButton.OnHold.AddListener(OnHoldUndoButton);
    }

    public void SetupWordController(TileController tileController, ScoreController scoreController)
    {
        _tileController = tileController;
        _scoreController = scoreController;
        SetSubmitButtonState(false);
        UpdateUndoButtonState();
        ResizeWordFormingArea();
    }

    public void AddTileToWord(ITile tile)
    {
        if (_nextLetterIndex == Settings.maxLetterCount) return;
        _currentWord += tile.TileData.Character;
        tilesInWordFormer.Add(tile);

        Vector2 positionToMoveTo2D = _letterPositions[_nextLetterIndex];
        Vector3 positionToMoveTo3D = new Vector3(positionToMoveTo2D.x, positionToMoveTo2D.y, tile.TileData.Position.z);

        _tilesInMovement++;
        _nextLetterIndex++;
        tile.LeaveTileArea(positionToMoveTo3D, OnTileMovementCompleted);
        UpdateUndoButtonState();
    }

    private void ResizeWordFormingArea()
    {
        float tileSize = _tileController.TileSize;
        References.wordFormingAreaRect.sizeDelta = new Vector2(tileSize * Settings.maxLetterCount + Settings.wordFormingAreaExtents.x, tileSize + Settings.wordFormingAreaExtents.y) / References.canvasScaler.transform.lossyScale.x;

        // cache letter positions
        Vector3 wordFormingAreaPosition = References.wordFormingAreaRect.position;
        float currentLetterX = 0;
        for (int i = 0; i < Settings.maxLetterCount; i++)
        {
            currentLetterX += tileSize;
            _letterPositions.Add(new Vector2(currentLetterX - (tileSize * (Settings.maxLetterCount+1) / 2f), wordFormingAreaPosition.y));
        }
    }

    private void OnTileMovementCompleted()
    {
        _tilesInMovement--;
        CheckWordIsSubmitable();
    }

    private void CheckWordIsSubmitable()
    {
        if (!IsWordValid())
        {
            SetSubmitButtonState(false);
            _scoreController.DisplayScoreForWord("");
        }
        else
        {
            SetSubmitButtonState(true);
            _scoreController.DisplayScoreForWord(_currentWord);
        }
    }

    private void SetSubmitButtonState(bool active)
    {
        References.submitWordButton.interactable = active;
    }

    private void UpdateUndoButtonState()
    {
        References.undoButton.interactable = (tilesInWordFormer.Count > 0);
    }

    private void OnClickSubmitWord()
    {
        SubmitWord();
    }

    private void OnClickUndoButton()
    {
        UndoLastLetter();
    }

    private void OnHoldUndoButton()
    {
        UndoAllTiles();
    }

    public bool IsWordValid()
    {
        if (!Config.possibleWords.Contains(_currentWord.ToLowerInvariant())) return false;
        if (_submittedWords.Contains(_currentWord)) return false;
        return true;
    }

    private void UndoLastLetter()
    {
        if (_nextLetterIndex == 0) return;
        int lastIndex = tilesInWordFormer.Count - 1;
        ITile tileToUndo = tilesInWordFormer[lastIndex];

        tilesInWordFormer.RemoveAt(lastIndex);
        _currentWord = _currentWord.Remove(lastIndex);
        _nextLetterIndex--;
        _tilesInMovement++;

        tileToUndo.ReturnToTileArea(null);
        tileToUndo.UpdateMonitor();
        UpdateUndoButtonState();
        CheckWordIsSubmitable();
    }

    private void SubmitWord()
    {
        _scoreController.WordSubmitted();
        _submittedWords.Add(_currentWord);
        RemoveTiles();
        ResetWord();
    }

    private void ResetWord()
    {
        tilesInWordFormer.Clear();

        _currentWord = "";
        _nextLetterIndex = 0;
        SetSubmitButtonState(false);
        _scoreController.DisplayScoreForWord(_currentWord);
    }

    private void RemoveTiles()
    {
        float tileSize = _tileController.TileSize;
        for (int i = tilesInWordFormer.Count-1; i >= 0; i--)
        {
            float currentExtraDelay = ((tilesInWordFormer.Count - i) * .05f);
            Sequence tween = DOTween.Sequence();
            tween.AppendInterval(currentExtraDelay);
            tween.Append(TweenHelper.Jump(tilesInWordFormer[i].Monitor.transform, null, tileSize/2, .25f));
            tween.Append(TweenHelper.ShrinkDisappear(tilesInWordFormer[i].Monitor.transform, tilesInWordFormer[i].GoToPool, .2f+ currentExtraDelay));
        }
    }

    private void UndoAllTiles()
    {
        foreach (ITile tile in tilesInWordFormer) tile.ReturnToTileArea(null);
        ResetWord();
        UpdateUndoButtonState();
        CheckWordIsSubmitable();
    }
}

[System.Serializable]
public class WordControllerReferences
{
    public CanvasScaler canvasScaler;
    public RectTransform wordFormingAreaRect;
    public Button submitWordButton;
    public HoldButton undoButton;
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
