using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class AutoSolver 
{
    private const int MAXIMUM_NUMBER_OF_WORD_ITERATIONS = 1000;
    private const int MIN_LETTER_COUNT = 3;

    private string[] allWords;
    private TileController _tileController;
    private WordController _wordController;

    private Stack<int> _submittedWordLengths = new Stack<int>();
    private Stack<ITile> _submittedTiles = new Stack<ITile>();
    private List<string> _submittedWords = new List<string>();
    private Stack<int> _cursorLocations = new Stack<int>();
    private int _calculationIterationsDone = 0;
    private bool _inNewBrach = true;
    private int _currentTargetWordLegth;

    public AutoSolver(string[] _allWords, TileControllerConfig tileControllerConfig, WordControllerConfig wordControllerConfig)
    {
        allWords = _allWords;
        Debug.Log(allWords.Length);

        TileControllerSettings tileControllerSettings = new TileControllerSettings();        
        WordControllerSettings wordControllerSettings = new WordControllerSettings();
        wordControllerSettings.maxLetterCount = 7;
        
        _tileController = new TileController(null, tileControllerSettings, tileControllerConfig);
        _wordController = new WordController(null, wordControllerSettings, wordControllerConfig);

        SetupControllers();
    }

    private void SetupControllers()
    {
        _wordController.StartWordControllerAutoSolver(_tileController);
        _tileController.SetupTileControllerAutoSolver(_wordController);
    }

    public void StartAutoSolver()
    {
        LinkedTree<ITile> LetterTree = new LinkedTree<ITile>();
        LinkedTree<string> WordTree = new LinkedTree<string>();
        _currentTargetWordLegth = _wordController.MaxWordLength;
        StartWordSearch(WordTree.Root, LetterTree.Root);
    }

    private void StartWordSearch(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        _cursorLocations.Push(0);

        foreach (ITile tile in _tileController.AllTiles)
        {
            if (!IsTileLegal(letterTreeNode, tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newNode);
        }
        if (_calculationIterationsDone == MAXIMUM_NUMBER_OF_WORD_ITERATIONS)
        {
            Debug.LogError("Too Many Calls Made !");
            return;
        }
    }

    private void StartWordSearchRecursive(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        if (_calculationIterationsDone == MAXIMUM_NUMBER_OF_WORD_ITERATIONS) return;
        _calculationIterationsDone++;
        if (_submittedWords.Contains(_wordController.CurrentWord))
        {
            return;
        }

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
        if(_inNewBrach) Debug.Log(string.Join(", ", _submittedWords));
        _inNewBrach = false;
        _wordController.UndoSubmitAutoSolver();
        int wordLengthToUndo = _submittedWordLengths.Pop();
        for (int i = 0; i < wordLengthToUndo; i++) _submittedTiles.Pop();
        _submittedWords.RemoveAt(_submittedWords.Count - 1);
    }

    private void CreateWordTree(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        if (_calculationIterationsDone == MAXIMUM_NUMBER_OF_WORD_ITERATIONS) return;
        if (_wordController.CurrentWord.Length == _wordController.MaxWordLength) return;

        letterTreeNode.Data.OnClick();

        // check if word formed so far exists in allWorlds
        int cursorLocation = _cursorLocations.Peek();
        FindWordResult result = MoveCursorTo(_wordController.CurrentWord, _wordController.MaxWordLength, ref cursorLocation);
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
                TreeNode<string> newWordTreeNode = new TreeNode<string>(_wordController.CurrentWord);
                StartWordSearchRecursive(newWordTreeNode, letterTreeNode);
                break;
            default:
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
        if (treeNode.HasChild(tile)) return false;
        return true;
    }

    private FindWordResult MoveCursorTo(string word, int maxLetters, ref int cursor)
    {
        string wordLower = word.ToLowerInvariant();
        for (int i = cursor; i < allWords.Length; i++)
        {
            if (allWords[i].Length > maxLetters) continue;
            int compareResult = string.Compare(wordLower, allWords[i]);
            if (compareResult == 1) continue;
            else if (compareResult == -1)
            {
                if (allWords[i].StartsWith(wordLower, System.StringComparison.OrdinalIgnoreCase))
                {
                    cursor = i;
                    return FindWordResult.WordPossible;
                }
                else return FindWordResult.WordInvalid;
            }
            else
            {
                if (word.Length < _currentTargetWordLegth)
                {
                    cursor = i;
                    return FindWordResult.WordPossible;
                }
                else
                {
                    cursor = i;
                    return FindWordResult.WordFound;
                }
            }
        }
        return FindWordResult.WordInvalid;
    }

    private enum FindWordResult
    {
        WordInvalid,
        WordPossible,
        WordFound,
    }
}
#endif
