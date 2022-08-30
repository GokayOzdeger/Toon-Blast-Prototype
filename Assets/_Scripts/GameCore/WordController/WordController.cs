using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordController
{
    public bool WordIsFull => _nextLetterIndex == Settings.maxLetterCount;
    public int MaxWordLength => Settings.maxLetterCount;
    public string CurrentWord { get; private set; } = "";
    public List<string> SubmittedWords { get; private set; } = new List<string>();
    public List<ITile> TilesInWordFormer { get; private set; } = new List<ITile>();
    private WordControllerReferences References { get; set; }
    private WordControllerSettings Settings { get; set; }
    private WordControllerConfig Config { get; set; }

    private TileController _tileController;
    private ScoreController _scoreController;
    private WordSearchController _wordSearchController;
    private WordFinder _wordFinder;
    private int _tilesInMovement = 0;
    private int _nextLetterIndex = 0;

    private List<Vector3> _letterPositions = new List<Vector3>();
    private Stack<SubmitInfo> _submitInfos = new Stack<SubmitInfo>();

    public WordController(WordControllerReferences references, WordControllerSettings settings, WordControllerConfig config)
    {
        References = references;
        Settings = settings;
        Config = config;

        if (References != null)
        {
            References.submitWordButton.onClick.AddListener(OnClickSubmitWord);
            References.undoButton.onClick.AddListener(OnClickUndoButton);
            References.undoButton.OnHold.AddListener(OnHoldUndoButton);
        }
    }

    public void StartWordController(TileController tileController, ScoreController scoreController)
    {
        _wordSearchController = new WordSearchController(Settings.maxLetterCount);
        _wordFinder = new WordFinder(tileController, this);
        _tileController = tileController;
        _scoreController = scoreController;
        SetSubmitButtonState(false);
        UpdateUndoButtonState();
        ResizeWordFormingArea();
    }

    public void LoadWordController(TileController tileController, ScoreController scoreController, List<string> submittedWords)
    {
        _wordSearchController = new WordSearchController(Settings.maxLetterCount);
        _wordFinder = new WordFinder(tileController, this);
        _tileController = tileController;
        _scoreController = scoreController;
        SubmittedWords = new List<string>(submittedWords);
        SetSubmitButtonState(false);
        UpdateUndoButtonState();
        ResizeWordFormingArea();
    }

    public void StartWordControllerAutoSolver(TileController tileController)
    {
        _tileController = tileController;
    }

    public void AddTileToWord(ITile tile)
    {
        if (_nextLetterIndex == Settings.maxLetterCount) return;
        CurrentWord += tile.TileData.Character.ToLowerInvariant();
        _wordSearchController.MoveCursorForwards(CurrentWord);
        TilesInWordFormer.Add(tile);

        Vector2 positionToMoveTo2D = _letterPositions[_nextLetterIndex];
        Vector3 positionToMoveTo3D = new Vector3(positionToMoveTo2D.x, positionToMoveTo2D.y, tile.TileData.Position.z);

        _tilesInMovement++;
        _nextLetterIndex++;
        tile.LeaveTileArea(positionToMoveTo3D, OnTileMovementCompleted);
        UpdateUndoButtonState();
    }

    public void AddTileToWordAutoSolver(ITile tile)
    {
        if (_nextLetterIndex == Settings.maxLetterCount) return;
        CurrentWord += tile.TileData.Character.ToLowerInvariant();
        TilesInWordFormer.Add(tile);

        _nextLetterIndex++;
        tile.LeaveTileArea(Vector3.zero, null);
    }

    public bool CheckHasPossibleWord()
    {
        return _wordFinder.CheckPossibleWordExists();
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
            _letterPositions.Add(new Vector2(currentLetterX - (tileSize * (Settings.maxLetterCount + 1) / 2f), wordFormingAreaPosition.y));
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
            _scoreController.DisplayScoreForWord(CurrentWord);
        }
    }

    private void SetSubmitButtonState(bool active)
    {
        References.submitWordButton.interactable = active;
    }

    private void UpdateUndoButtonState()
    {
        References.undoButton.interactable = (TilesInWordFormer.Count > 0);
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

    private bool IsWordValid()
    {
        if (_wordSearchController.IsWordMatchFound(CurrentWord)) return true;
        return false;
    }

    private void UndoLastLetter()
    {
        if (_nextLetterIndex == 0) return;
        int lastIndex = TilesInWordFormer.Count - 1;
        ITile tileToUndo = TilesInWordFormer[lastIndex];

        TilesInWordFormer.RemoveAt(lastIndex);
        CurrentWord = CurrentWord.Remove(lastIndex);
        _wordSearchController.RemoveCursors(1);

        _nextLetterIndex--;
        _tilesInMovement++;

        tileToUndo.ReturnToTileArea(null);
        tileToUndo.LockChildren();
        tileToUndo.UpdateMonitor();
        UpdateUndoButtonState();
        CheckWordIsSubmitable();
    }

    public void UndoAutoSolver()
    {
        if (_nextLetterIndex == 0) return;
        int lastIndex = TilesInWordFormer.Count - 1;
        ITile tileToUndo = TilesInWordFormer[lastIndex];

        TilesInWordFormer.RemoveAt(lastIndex);
        CurrentWord = CurrentWord.Remove(lastIndex);
        _nextLetterIndex--;
        _tilesInMovement++;

        tileToUndo.ReturnToTileArea(null);
        tileToUndo.LockChildren();
    }

    private void SubmitWord()
    {
        _scoreController.WordSubmitted();
        SubmittedWords.Add(CurrentWord);
        RemoveTiles();
        ResetWord();

        // save game after each submit
        LevelManager.Instance.SaveLevelState();
    }

    public void SubmitWordAutoSolver()
    {
        _submitInfos.Push(new SubmitInfo(new List<ITile>(TilesInWordFormer), CurrentWord));
        TilesInWordFormer.Clear();
        CurrentWord = "";
        _nextLetterIndex = 0;
    }

    public void UndoSubmitAutoSolver()
    {
        SubmitInfo info = _submitInfos.Pop();
        TilesInWordFormer = info.tilesUsed;
        CurrentWord = info.wordSubmitted;
        _nextLetterIndex = CurrentWord.Length;
    }

    private void ResetWord()
    {
        _wordSearchController.RemoveCursors(TilesInWordFormer.Count);
        TilesInWordFormer.Clear();

        CurrentWord = "";
        _nextLetterIndex = 0;
        SetSubmitButtonState(false);
        _scoreController.DisplayScoreForWord(CurrentWord);
    }

    private void RemoveTiles()
    {
        // send tiles to pool after animating
        float tileSize = _tileController.TileSize;
        float jumpAnimDuration = .25f;
        for (int i = TilesInWordFormer.Count - 1; i >= 0; i--)
        {
            TilesInWordFormer[i].RemoveFromPlay();

            float currentExtraDelay = ((TilesInWordFormer.Count - i) * .05f);
            float shrinkDuration = currentExtraDelay + .2f;
            Sequence tween = DOTween.Sequence();
            tween.AppendInterval(currentExtraDelay);
            tween.Append(TweenHelper.Jump(TilesInWordFormer[i].Monitor.transform, null, tileSize / 2, jumpAnimDuration));
            tween.Append(TweenHelper.ShrinkDisappear(TilesInWordFormer[i].Monitor.transform, TilesInWordFormer[i].RemoveFromPlay, shrinkDuration));
        }
    }

    private void UndoAllTiles()
    {
        foreach (ITile tile in TilesInWordFormer) tile.ReturnToTileArea(null);
        foreach (ITile tile in TilesInWordFormer) tile.LockChildren();
        ResetWord();
        UpdateUndoButtonState();
        CheckWordIsSubmitable();
    }

    public class SubmitInfo
    {
        public string wordSubmitted;
        public List<ITile> tilesUsed;
        
        public SubmitInfo(List<ITile> tilesUsed, string wordSubmitted)
        {
            this.wordSubmitted = wordSubmitted;
            this.tilesUsed = tilesUsed;
        }
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
    //
}
