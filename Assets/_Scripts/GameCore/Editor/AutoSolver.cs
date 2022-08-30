using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class AutoSolver 
{
    private string[] allWords;
    private TileController _tileController;
    private WordController _wordController;

    private Stack<int> submittedWordLengths = new Stack<int>();
    private Stack<ITile> submittedTiles = new Stack<ITile>();
    private HashSet<string> submittedWords = new HashSet<string>();
    private Stack<int> cursorLocations = new Stack<int>();

    public AutoSolver(string[] _allWords, TileControllerConfig tileControllerConfig, WordControllerConfig wordControllerConfig)
    {
        allWords = _allWords;
        Debug.Log(allWords.Length);

        TileControllerSettings tileControllerSettings = new TileControllerSettings();        
        WordControllerSettings wordControllerSettings = new WordControllerSettings();
        wordControllerSettings.maxLetterCount = 3;
        
        _tileController = new TileController(null, tileControllerSettings, tileControllerConfig);
        _wordController = new WordController(null, wordControllerSettings, wordControllerConfig);

        SetupControllers();
    }

    private void SetupControllers()
    {
        _wordController.SetupWordControllerAutoSolver(_tileController);
        _tileController.SetupTileControllerAutoSolver(_wordController);
    }

    public void StartAutoSolver()
    {
        Debug.Log("Hello");
        LinkedTree<ITile> LetterTree = new LinkedTree<ITile>();
        LinkedTree<string> WordTree = new LinkedTree<string>();
        
        Stack<int> cursorLocations = new Stack<int>();
        StartWordSearch(WordTree.Root, LetterTree.Root);
    }

    private void StartWordSearch(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        cursorLocations.Push(0);

        foreach (ITile tile in _tileController.AllTiles)
        {
            if (tile.Locks != 0) continue;
            if (letterTreeNode.HasChild(tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newNode);
        }
    }

    private void StartWordSearchRecursive(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        _wordController.SubmitWordAutoSolver();
        cursorLocations.Push(0);

        foreach (ITile tile in _tileController.AllTiles)
        {
            if (tile.Locks != 0) continue;
            if (letterTreeNode.HasChild(tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newNode);
        }
        cursorLocations.Pop();
    }

    private void CreateWordTree(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode)
    {
        if (_wordController.CurrentWord.Length == _wordController.MaxWordLength) return;


        letterTreeNode.Data.OnClick();

        // check if word formed so far exists in allWorlds
        int cursorLocation = cursorLocations.Peek();
        FindWordResult result = MoveCursorTo(_wordController.CurrentWord, _wordController.MaxWordLength, ref cursorLocation);
        switch (result)
        {
            case FindWordResult.WordInvalid:
                _wordController.UndoAutoSolver();
                return;
            case FindWordResult.WordPossible:
                cursorLocations.Push(cursorLocation);
                break;
            case FindWordResult.WordFound:
                cursorLocations.Push(cursorLocation);
                TreeNode<string> newWordTreeNode = new TreeNode<string>(_wordController.CurrentWord);
                Debug.Log("Next Word...");
                StartWordSearchRecursive(newWordTreeNode, letterTreeNode);
                break;
            default:
                break;
        }

        foreach (ITile tile in _tileController.AllTiles)
        {
            if (!tile.Clickable) continue;
            if (letterTreeNode.HasChild(tile)) continue;
            TreeNode<ITile> newLetterTreeNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newLetterTreeNode);
        }

        _wordController.UndoAutoSolver();
        cursorLocations.Pop();
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
                    Debug.Log(wordLower + " / " + allWords[i]);
                    cursor = i;
                    return FindWordResult.WordPossible;
                }
                else return FindWordResult.WordInvalid;
            }
            else
            {
                Debug.Log("MATCH: "+wordLower);
                cursor = i;
                return FindWordResult.WordFound;
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
