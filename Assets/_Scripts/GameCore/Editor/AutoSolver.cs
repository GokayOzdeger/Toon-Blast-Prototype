using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class AutoSolver 
{ 
    private TileController tileController;
    private string[] allWords;

    public AutoSolver(TileControllerConfig tileControllerConfig, string[] _allWords)
    {
        allWords = _allWords;
        Debug.Log(allWords.Length);
        TileControllerSettings tileControllerSettings = new TileControllerSettings();        
        tileController = new TileController(null, tileControllerSettings, tileControllerConfig);

        SetupControllers();
    }

    private void SetupControllers()
    {
        tileController.SetupTileManagerAutoSolver();
    }

    public void StartAutoSolver()
    {
        Debug.Log("Hello");
        LinkedTree<ITile> LetterTree = new LinkedTree<ITile>();
        
        Stack<ITile> usedTiles = new Stack<ITile>();
        Stack<int> cursorLocations = new Stack<int>();
        CreateWordTree(LetterTree.Root, tileController.AllTiles, usedTiles, cursorLocations);
    }

    private void CreateWordTree(TreeNode<ITile> currentNode, List<ITile> tilesLeft, Stack<ITile> usedTiles, Stack<int> cursorLocations, int brachLength = -1)
    {
        if (brachLength == 5) 
        {
            usedTiles.Pop();
            currentNode.Data.LockChildren();
            return; 
        }
        
        int cursorLocation = 0;
        if (cursorLocations.Count > 0) cursorLocation = cursorLocations.Peek();
        if (currentNode.Data != null)
        {
            if (MoveCursorTo(GetWordFromTreeNode(currentNode), ref cursorLocation))
            {
                cursorLocations.Push(cursorLocation);
            }
            else
            {
                usedTiles.Pop();
                currentNode.Data.LockChildren();
                return;
            }
        }

        foreach (ITile tile in tilesLeft)
        {
            if (tile.Locks != 0) continue;
            if (usedTiles.Contains(tile)) continue;
            if (currentNode.HasChild(tile)) continue;
            tile.UnlockChildren();
            TreeNode<ITile> newNode = currentNode.AddChild(tile);
            usedTiles.Push(tile);
            CreateWordTree(newNode, tilesLeft, usedTiles, cursorLocations, brachLength + 1);
        }
        
        if (usedTiles.Count > 0) usedTiles.Pop();
        
        if (currentNode.Data != null) currentNode.Data.LockChildren();
        if(cursorLocations.Count > 0) cursorLocations.Pop();
    }

    private bool MoveCursorTo(string word, ref int cursor)
    {
        for (int i = cursor; i < allWords.Length; i++)
        {
            int compareResult = string.Compare(word, allWords[i]);
            if (compareResult == 1) continue;
            else if (compareResult == -1)
            {
                if (allWords[i].StartsWith(word, System.StringComparison.OrdinalIgnoreCase))
                {
                    cursor = i;
                    return true;
                }
                else return false;
            }
            else
            {
                cursor = i;
                return true;
            }
        }
        return false;
    }

    private string GetWordFromTreeNode(TreeNode<ITile> node)
    {
        string word = "";
        TreeNode<ITile> currentNode = node;
        while(currentNode != null)
        {
            if (currentNode.Data == null) break;
            word = currentNode.Data.TileData.Character + word;
            currentNode = currentNode.ParentNode;
        }
        Debug.Log(word);
        return word;
    }
}
#endif
