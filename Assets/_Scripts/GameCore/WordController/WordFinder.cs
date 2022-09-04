using System;
using System.Collections.Generic;
using UnityEngine;

public class WordFinder
{
    private const int MAXIMUM_NUMBER_OF_ITERATIONS = 150;

    private readonly string[] _allWords;

    private readonly Stack<int> _cursorLocations = new();
    private readonly TileController _tileController;
    private readonly WordController _wordController;
    private int _numberOfIterationsDone;

    public WordFinder(TileController tileController, WordController wordController)
    {
        _tileController = tileController;
        _wordController = wordController;
        _allWords = WordSearchController.AllWords;
        _cursorLocations.Push(0);
    }

    public bool CheckPossibleWordExists()
    {
        _numberOfIterationsDone = 0;
        LinkedTree<ITile> letterTree = new();
        return StartWordSearch(letterTree.Root);
    }

    private bool StartWordSearch(TreeNode<ITile> letterTreeNode)
    {
        var foundWord = false;
        SetUsingMonitorToAllTiles(false);
        // cache than undo any moves made (enables editor use when letters are present in word former)
        var tilesInWordFormer = new List<ITile>(_wordController.TilesInWordFormer);
        while (_wordController.CurrentWord.Length > 0) _wordController.UndoAutoSolver();

        // start recursive word search for all possible starting letters
        foreach (ITile tile in _tileController.AllTiles)
        {
            if (!IsTileLegal(letterTreeNode, tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            if (SearchWord(newNode) == FindWordResult.WordFound)
            {
                foundWord = true;
                break;
            }
        }

        // redo any moves made in cache (enables editor use when letters are present in word former)
        foreach (ITile tile in tilesInWordFormer) tile.OnClick();
        SetUsingMonitorToAllTiles(true);

        if (_numberOfIterationsDone ==
            MAXIMUM_NUMBER_OF_ITERATIONS) // fail safe in case runtime word finding takes too long
        {
            Debug.LogError("Too Word Finder Iterations Made !");
            return foundWord;
        }

        return foundWord;
    }

    private FindWordResult SearchWord(TreeNode<ITile> letterTreeNode)
    {
        _numberOfIterationsDone++;
        if (_numberOfIterationsDone == MAXIMUM_NUMBER_OF_ITERATIONS) return FindWordResult.WordInvalid;
        if (_wordController.CurrentWord.Length == _wordController.MaxWordLength) return FindWordResult.WordInvalid;

        letterTreeNode.Data.OnClick();

        // check if word formed so far exists in allWorlds
        int cursorLocation = _cursorLocations.Peek();
        FindWordResult wordFindResult =
            MoveCursorTo(_wordController.CurrentWord, _wordController.MaxWordLength, ref cursorLocation);
        if (wordFindResult == FindWordResult.WordFound)
            Debug.Log("Found possible word: " + _wordController.CurrentWord);

        if (wordFindResult == FindWordResult.WordPossible)
        {
            _cursorLocations.Push(cursorLocation);
            foreach (ITile tile in _tileController.AllTiles)
            {
                if (!IsTileLegal(letterTreeNode, tile)) continue;
                TreeNode<ITile> newLetterTreeNode = letterTreeNode.AddChild(tile);
                wordFindResult = SearchWord(newLetterTreeNode);
                if (wordFindResult == FindWordResult.WordFound) break;
            }

            _cursorLocations.Pop();
        }

        _wordController.UndoAutoSolver();
        return wordFindResult;
    }

    private bool IsTileLegal(TreeNode<ITile> treeNode, ITile tile)
    {
        if (tile.IsRemovedFromPlay) return false;
        if (!tile.Clickable) return false;
        return !treeNode.HasChild(tile);
    }

    private FindWordResult MoveCursorTo(string word, int maxLetters, ref int cursor)
    {
        for (int i = cursor; i < _allWords.Length; i++)
        {
            if (_allWords[i].Length > maxLetters) continue;
            int compareResult = string.CompareOrdinal(word, _allWords[i]);
            switch (compareResult)
            {
                case 1:
                    continue;
                case -1 when _allWords[i].StartsWith(word, StringComparison.InvariantCulture):
                    cursor = i;
                    return FindWordResult.WordPossible;
                case -1:
                    return FindWordResult.WordInvalid;
            }
        }

        return FindWordResult.WordInvalid;
    }

    private void SetUsingMonitorToAllTiles(bool usingMonitor)
    {
        foreach (ITile tile in _tileController.AllTiles)
            tile.UsingMonitor = usingMonitor;
    }

    private enum FindWordResult
    {
        WordInvalid,
        WordPossible,
        WordFound
    }
}