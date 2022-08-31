using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSearchController
{
    public static string[] AllWords;
    private Stack<int> _cursorLocations = new Stack<int>();

    private int _maxWordLength;

    public WordSearchController(int maxWordLength)
    {
        _maxWordLength = maxWordLength;
        _cursorLocations.Push(0);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void LoadAllWordsFromText()
    {
        TextAsset allWordsTextAsset = Resources.Load<TextAsset>("allWords");
        string content = allWordsTextAsset.text;
        AllWords = content.Split("\r\n");
    }

    public bool IsWordMatchFound(string word)
    {
        return AllWords[_cursorLocations.Peek()] == word;
    }
    public void MoveCursorForwards(string word)
    {
        int cursor = _cursorLocations.Peek();
        for (int i = cursor; i < AllWords.Length; i++)
        {
            if (AllWords[i].Length > _maxWordLength) continue;
            int compareResult = string.Compare(word, AllWords[i], System.StringComparison.InvariantCulture);
            if (compareResult == 1) continue;
            _cursorLocations.Push(i);
            return;
        }
        _cursorLocations.Push(AllWords.Length - 1);
    }

    public void RemoveCursors(int countToRemove)
    {
        for (int i = 0; i < countToRemove; i++) _cursorLocations.Pop();
    }
}
