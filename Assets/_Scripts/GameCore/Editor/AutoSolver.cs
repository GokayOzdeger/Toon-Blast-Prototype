using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class AutoSolver 
{ 
    private TileController tileController;
    private WordController wordController;
    private string[] allWords;

    public AutoSolver(string[] _allWords, TileControllerConfig tileControllerConfig, WordControllerConfig wordControllerConfig)
    {
        allWords = _allWords;
        Debug.Log(allWords.Length);

        TileControllerSettings tileControllerSettings = new TileControllerSettings();        
        WordControllerSettings wordControllerSettings = new WordControllerSettings();
        wordControllerSettings.maxLetterCount = 3;
        
        tileController = new TileController(null, tileControllerSettings, tileControllerConfig);
        wordController = new WordController(null, wordControllerSettings, wordControllerConfig);

        SetupControllers();
    }

    private void SetupControllers()
    {
        wordController.SetupWordControllerAutoSolver(tileController);
        tileController.SetupTileControllerAutoSolver(wordController);
    }

    public void StartAutoSolver()
    {
        Debug.Log("Hello");
        LinkedTree<ITile> LetterTree = new LinkedTree<ITile>();
        LinkedTree<string> WordTree = new LinkedTree<string>();
        
        Stack<int> cursorLocations = new Stack<int>();
        StartWordSearch(WordTree.Root, LetterTree.Root, tileController.AllTiles, cursorLocations);
    }

    private void StartWordSearch(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode, List<ITile> tilesLeft, Stack<int> cursorLocations)
    {
        cursorLocations.Push(0);
        tilesLeft = new List<ITile>(tilesLeft);
        foreach (ITile tile in tilesLeft)
        {
            if (tile.Locks != 0) continue;
            if (letterTreeNode.HasChild(tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newNode, tileController.AllTiles, cursorLocations, 0);
        }
    }

    private void StartWordSearchRecursive(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode, List<ITile> tilesLeft, Stack<int> cursorLocations)
    {
        wordController.SubmitWordAutoSolver();
        cursorLocations.Push(0);

        tilesLeft = new List<ITile>(tilesLeft);
        foreach (ITile tile in tilesLeft)
        {
            if (tile.Locks != 0) continue;
            if (letterTreeNode.HasChild(tile)) continue;
            TreeNode<ITile> newNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newNode, tileController.AllTiles, cursorLocations, 0);
        }
        cursorLocations.Pop();
    }

    private void CreateWordTree(TreeNode<string> wordTreeNode, TreeNode<ITile> letterTreeNode, List<ITile> tilesLeft, Stack<int> cursorLocations, int brachLength)
    {
        if (brachLength+1 == wordController.MaxWordLength) return;


        letterTreeNode.Data.OnClick();

        // check if word formed so far exists in allWorlds
        int cursorLocation = cursorLocations.Peek();
        FindWordResult result = MoveCursorTo(wordController.CurrentWord, wordController.MaxWordLength, ref cursorLocation);
        switch (result)
        {
            case FindWordResult.WordInvalid:
                wordController.UndoAutoSolver();
                return;
            case FindWordResult.WordPossible:
                cursorLocations.Push(cursorLocation);
                break;
            case FindWordResult.WordFound:
                cursorLocations.Push(cursorLocation);
                TreeNode<string> newWordTreeNode = new TreeNode<string>(wordController.CurrentWord);
                Debug.Log("Next Word...");
                StartWordSearchRecursive(newWordTreeNode, letterTreeNode, tileController.AllTiles, cursorLocations);
                break;
            default:
                break;
        }

        tilesLeft = new List<ITile>(tilesLeft);
        foreach (ITile tile in tilesLeft)
        {
            if (!tile.Clickable) continue;
            if (letterTreeNode.HasChild(tile)) continue;
            TreeNode<ITile> newLetterTreeNode = letterTreeNode.AddChild(tile);
            CreateWordTree(wordTreeNode, newLetterTreeNode, tileController.AllTiles, cursorLocations, brachLength + 1);
        }

        wordController.UndoAutoSolver();
        cursorLocations.Pop();
    }

    private FindWordResult MoveCursorTo(string word, int maxLetters, ref int cursor)
    {
        string wordLower = word.ToLowerInvariant();
        for (int i = cursor; i < allWords.Length; i++)
        {
            if (allWords[i].Length > maxLetters) continue;
            Debug.Log(wordLower + " / " + allWords[i]);
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
