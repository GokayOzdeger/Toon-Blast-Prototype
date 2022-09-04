using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class AutoSolver
{
    private const int MAXIMUM_NUMBER_OF_ITERATIONS = 20000;
    private readonly Stack<int> _cursorLocations = new();
    private readonly Stack<ITile> _submittedTiles = new();

    private readonly Stack<int> _submittedWordLengths = new();
    private readonly List<string> _submittedWords = new();
    private readonly TileController _tileController;
    private readonly WordController _wordController;

    private readonly string[] allWords;
    private int _calculationIterationsDone;
    private int _currentTargetWordLegth;
    private bool _inNewBrach = true;

    public AutoSolver(string[] allWords, TileControllerConfig tileControllerConfig,
        WordControllerConfig wordControllerConfig)
    {
        this.allWords = allWords;
        _cursorLocations.Push(0);
        Debug.Log(allWords.Length);

        var tileControllerSettings = new TileControllerSettings();
        var wordControllerSettings = new WordControllerSettings
        {
            maxLetterCount = 7
        };

        _tileController = new TileController(null, tileControllerSettings, tileControllerConfig);
        _wordController = new WordController(null, wordControllerSettings);

        SetupControllers();
    }

    private void SetupControllers()
    {
        _wordController.StartWordControllerAutoSolver(_tileController);
        _tileController.SetupTileControllerAutoSolver(_wordController);
    }

    public void StartAutoSolver()
    {
        var LetterTree = new LinkedTree<ITile>();
        var WordTree = new LinkedTree<string>();
        _currentTargetWordLegth = _wordController.MaxWordLength;
        StartWordSearch(WordTree.Root, LetterTree.Root);
    }

    private void StartWordSearch(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        foreach (ITile tile in _tileController.AllTiles)
        {
            if (!IsTileLegal(letterTreeNode, tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newNode);
        }

        if (_calculationIterationsDone == MAXIMUM_NUMBER_OF_ITERATIONS) Debug.LogError("Too Many Calls Made !");
    }

    private void StartWordSearchRecursive(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        if (_calculationIterationsDone == MAXIMUM_NUMBER_OF_ITERATIONS) return;

        if (_submittedWords.Contains(_wordController.CurrentWord)) return;

        DoSubmitWord();
        _cursorLocations.Push(0);

        foreach (ITile tile in _tileController.AllTiles)
        {
            if (!IsTileLegal(letterTreeNode, tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newNode);
        }

        _cursorLocations.Pop();
        UndoSubmitWord();
    }

    private void DoSubmitWord()
    {
        _inNewBrach = true;
        foreach (ITile tile in _wordController.TilesInWordFormer) _submittedTiles.Push(tile);
        _submittedWordLengths.Push(_wordController.CurrentWord.Length);
        _submittedWords.Add(_wordController.CurrentWord);
        _wordController.SubmitWordAutoSolver();
    }

    private void UndoSubmitWord()
    {
        if (_inNewBrach) Debug.Log(string.Join(", ", _submittedWords));
        _inNewBrach = false;
        _wordController.UndoSubmitAutoSolver();
        int wordLengthToUndo = _submittedWordLengths.Pop();
        for (var i = 0; i < wordLengthToUndo; i++) _submittedTiles.Pop();
        _submittedWords.RemoveAt(_submittedWords.Count - 1);
    }

    private void CreateWordTree(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        if (_calculationIterationsDone == MAXIMUM_NUMBER_OF_ITERATIONS) return;
        if (_wordController.CurrentWord.Length == _wordController.MaxWordLength) return;

        _calculationIterationsDone++;
        letterTreeNode.Data.OnClick();

        // check if word formed so far exists in allWorlds
        int cursorLocation = _cursorLocations.Peek();
        FindWordResult result =
            MoveCursorTo(_wordController.CurrentWord, _wordController.MaxWordLength, ref cursorLocation);
        switch (result)
        {
            case FindWordResult.WordInvalid:
                _wordController.UndoAutoSolver();
                return;
            case FindWordResult.WordPossible:
                _cursorLocations.Push(cursorLocation);
                break;
            case FindWordResult.WordFound:
                _cursorLocations.Push(cursorLocation);
                if (wordTreeNode.HasChild(_wordController.CurrentWord)) break;
                TreeNode<string> newWordTreeNode = wordTreeNode.AddChild(_wordController.CurrentWord);
                StartWordSearchRecursive(newWordTreeNode, letterTreeNode);
                break;
        }

        foreach (ITile tile in _tileController.AllTiles)
        {
            if (!IsTileLegal(letterTreeNode, tile)) continue;
            TreeNode<ITile> newLetterTreeNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newLetterTreeNode);
        }

        _wordController.UndoAutoSolver();
        _cursorLocations.Pop();
    }

    private bool IsTileLegal(TreeNode<ITile> treeNode, ITile tile)
    {
        if (!tile.Clickable) return false;
        if (_submittedTiles.Contains(tile)) return false;
        return !treeNode.HasChild(tile);
    }

    private FindWordResult MoveCursorTo(string word, int maxLetters, ref int cursor)
    {
        for (int i = cursor; i < allWords.Length; i++)
        {
            if (allWords[i].Length > maxLetters) continue;
            // ReSharper disable once StringCompareIsCultureSpecific.1
            int compareResult = string.Compare(word, allWords[i]);
            if (compareResult == 1) continue;

            if (compareResult == -1)
            {
                if (!allWords[i].StartsWith(word, StringComparison.OrdinalIgnoreCase))
                    return FindWordResult.WordInvalid;
                cursor = i;
                return FindWordResult.WordPossible;
            }

            if (word.Length < _currentTargetWordLegth)
            {
                cursor = i;
                return FindWordResult.WordPossible;
            }

            cursor = i;
            return FindWordResult.WordFound;
        }

        return FindWordResult.WordInvalid;
    }

    private enum FindWordResult
    {
        WordInvalid,
        WordPossible,
        WordFound
    }
}
#endif