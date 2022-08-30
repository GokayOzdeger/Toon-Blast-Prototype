using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFinder
{
    private const int MAXIMUM_NUMBER_OF_ITERATIONS = 150;

    private string[] allWords;
    private TileController _tileController;
    private WordController _wordController;

    private Stack<int> _cursorLocations = new Stack<int>();
    private int _numberOfIterationsDone = 0;

    public WordFinder(TileController tileController, WordController wordController)
    {
        _tileController = tileController;
        _wordController = wordController;
        allWords = WordSearchController.AllWords;
        _cursorLocations.Push(0);
        Debug.Log(allWords.Length);
    }

    public bool CheckPossibleWordExists()
    {
        _numberOfIterationsDone = 0;
        LinkedTree<ITile> LetterTree = new LinkedTree<ITile>();
        return StartWordSearch(LetterTree.Root);
    }

    private bool StartWordSearch(TreeNode<ITile> letterTreeNode)
    {
        bool foundWord = false;
        SetUsingMonitorToAllTiles(false);
        // cache than undo any moves made (enables editor use when letters are present in wordformer)
        List<ITile> tilesInWordFormer = new List<ITile>(_wordController.TilesInWordFormer);
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

        // redo any moves made in cache (enables editor use when letters are present in wordformer)
        foreach (ITile tile in tilesInWordFormer) tile.OnClick(); 
        SetUsingMonitorToAllTiles(true);

        if (_numberOfIterationsDone == MAXIMUM_NUMBER_OF_ITERATIONS) // fail safe incase runtime word finding takes too long
        {
            Debug.LogError("Too Word Finder Iteratiions Made !");
            return foundWord;
        }
        return foundWord;
    }

    private FindWordResult SearchWord(TreeNode<ITile> letterTreeNode)
    {
        FindWordResult wordFindResult;

        _numberOfIterationsDone++;
        if (_numberOfIterationsDone == MAXIMUM_NUMBER_OF_ITERATIONS) return FindWordResult.WordInvalid;
        if (_wordController.CurrentWord.Length == _wordController.MaxWordLength) return FindWordResult.WordInvalid;

        letterTreeNode.Data.OnClick();

        // check if word formed so far exists in allWorlds
        int cursorLocation = _cursorLocations.Peek();
        wordFindResult = MoveCursorTo(_wordController.CurrentWord, _wordController.MaxWordLength, ref cursorLocation);
        if (wordFindResult == FindWordResult.WordFound) Debug.Log("Found possible word: " + _wordController.CurrentWord);

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
        if (treeNode.HasChild(tile)) return false;
        return true;
    }

    private FindWordResult MoveCursorTo(string word, int maxLetters, ref int cursor)
    {
        for (int i = cursor; i < allWords.Length; i++)
        {
            if (allWords[i].Length > maxLetters) continue;
            int compareResult = string.Compare(word, allWords[i]);
            if (compareResult == 1) continue;
            else if (compareResult == -1)
            {
                if (allWords[i].StartsWith(word, System.StringComparison.InvariantCulture))
                {
                    cursor = i;
                    return FindWordResult.WordPossible;
                }
                else return FindWordResult.WordInvalid;
            }
            else
            {
                cursor = i;
                return FindWordResult.WordFound;
            }
        }
        return FindWordResult.WordInvalid;
    }

    private void SetUsingMonitorToAllTiles(bool usingMonitor)
    {
        foreach (ITile tile in _tileController.AllTiles) tile.UsingMonitor = usingMonitor;
    }

    private enum FindWordResult
    {
        WordInvalid,
        WordPossible,
        WordFound,
    }
}
