using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordController
{
    public bool WordIsFull => _nextLetterIndex == Settings.maxLetterCount;
    public string CurrentWord => _currentWord;
    public int MaxWordLength => Settings.maxLetterCount;
    public SubmitInfo LastSubmitInfo => submitInfos[submitInfos.Count - 1];
    private WordControllerReferences References { get; set; }
    private WordControllerSettings Settings { get; set; }
    private WordControllerConfig Config { get; set; }

    private ScoreController _scoreController;
    private TileController _tileController;
    private int _tilesInMovement = 0;
    private int _nextLetterIndex = 0;
    private string _currentWord = "";
    private List<string> submittedWords = new List<string>();
    private List<Vector3> letterPositions = new List<Vector3>();
    private List<ITile> tilesInWordFormer = new List<ITile>();
    private List<SubmitInfo> submitInfos = new List<SubmitInfo>();
    private bool UsingUI { get; set; }

    public WordController(WordControllerReferences references, WordControllerSettings settings, WordControllerConfig config)
    {
        References = references;
        Settings = settings;
        Config = config;

        References?.submitWordButton.onClick.AddListener(OnClickSubmitWord);
        References?.undoButton.onClick.AddListener(OnClickUndoButton);
    }

    public void SetupWordController(TileController tileController, ScoreController scoreController)
    {
        UsingUI = true;
        _tileController = tileController;
        _scoreController = scoreController;
        SetSubmitButtonState(false);
        ResizeWordFormingArea(tileController);
    }

    public void SetupWordControllerAutoPlayer(TileController tileController)
    {
        UsingUI = false;
        _tileController = tileController;
    }

    public void AddTileToWord(ITile tile)
    {
        if (_nextLetterIndex == Settings.maxLetterCount) return;
        _currentWord += tile.TileData.Character;
        tilesInWordFormer.Add(tile);
        _tilesInMovement++;

        // start movement for tile
        if (UsingUI)
        {
            Vector2 positionToMoveTo2D = letterPositions[_nextLetterIndex];
            Vector3 positionToMoveTo3D = new Vector3(positionToMoveTo2D.x, positionToMoveTo2D.y, tile.TileData.Position.z);
            tile.LeaveTileArea(positionToMoveTo3D, OnLetterMovementCompleted);
        }
        else tile.LeaveTileArea(Vector3.zero, OnLetterMovementCompleted);
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
        if (_tilesInMovement != 0) return;
        if (_nextLetterIndex != Settings.maxLetterCount) return;
        ReturnTiles();
        ResetWord();
    }

    private void OnLetterMovementCompleted()
    {
        _tilesInMovement--;
        if (!UsingUI) return;
        if (!IsWordValid())
        {
            SetSubmitButtonState(false);
            _scoreController?.DisplayScoreForWord("");
            //CheckWordFilled();
        }
        else
        {
            SetSubmitButtonState(true);
            _scoreController?.DisplayScoreForWord(_currentWord);
        }
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
        UndoLastLetter();
    }

    public bool IsWordValid()
    {
        if (!Config.possibleWords.Contains(_currentWord.ToLower())) return false;
        if (submittedWords.Contains(_currentWord)) return false;
        return true;
    }

    public void UndoLastLetter()
    {
        if (_nextLetterIndex == 0) return;
        int lastIndex = tilesInWordFormer.Count - 1;
        ITile tileToUndo = tilesInWordFormer[lastIndex];

        tilesInWordFormer.RemoveAt(lastIndex);
        _currentWord = _currentWord.Remove(lastIndex);
        _nextLetterIndex--;
        _tilesInMovement++;

        tileToUndo.ReturnToTileArea(OnLetterMovementCompleted);
        tileToUndo.LockChildren();
        tileToUndo.UpdateMonitor();
    }

    public void UndoLastSubmit()
    {
        submittedWords.RemoveAt(submittedWords.Count - 1);
        SubmitInfo lastSubmitInfo = submitInfos[submitInfos.Count - 1];
        submitInfos.RemoveAt(submitInfos.Count - 1);
        foreach (ITile tile in lastSubmitInfo.TilesUsed)
        {
            _tileController.AddTile(tile);
            tile.ReturnToTileArea(null);
        }
        foreach (ITile tile in lastSubmitInfo.TilesUsed) tile.LockChildren();
    }

    public void SubmitWord()
    {
        _scoreController?.WordSubmitted();
        submittedWords.Add(_currentWord);
        submitInfos.Add(new SubmitInfo(tilesInWordFormer, CurrentWord));
        RemoveTiles();
        ResetWord();
    }

    private void ResetWord()
    {
        tilesInWordFormer.Clear();

        _currentWord = "";
        _nextLetterIndex = 0;

        if (UsingUI)
        {
            SetSubmitButtonState(false);
            _scoreController?.DisplayScoreForWord(_currentWord);
        }
    }
    
    private void RemoveTiles()
    {
        if (UsingUI)
        {
            float tileSize = _tileController.TileSize;
            for (int i = tilesInWordFormer.Count - 1; i >= 0; i--)
            {
                float currentExtraDelay = ((tilesInWordFormer.Count - i) * .05f);
                Sequence tween = DOTween.Sequence();
                tween.AppendInterval(currentExtraDelay);
                tween.Append(TweenHelper.Jump(tilesInWordFormer[i].Monitor.transform, null, tileSize / 2, .25f));
                tween.Append(TweenHelper.ShrinkDisappear(tilesInWordFormer[i].Monitor.transform, tilesInWordFormer[i].GoToPool, .2f + currentExtraDelay));
            }
        }

        foreach (var tile in tilesInWordFormer) _tileController.RemoveTile(tile);
    }

    private void ReturnTiles()
    {
        foreach (ITile tile in tilesInWordFormer) tile.ReturnToTileArea(null);
        foreach (ITile tile in tilesInWordFormer) tile.LockChildren();
        foreach (ITile tile in tilesInWordFormer) tile.UpdateMonitor();
    }

    public struct SubmitInfo 
    {
        public string Word;
        public List<ITile> TilesUsed;

        public SubmitInfo(List<ITile> tilesUsed, string word)
        {
            TilesUsed = tilesUsed;
            Word = word;
        }
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
