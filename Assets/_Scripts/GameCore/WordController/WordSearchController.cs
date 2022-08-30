using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSearchController
{
    private static string[] _allWords;
    private Stack<int> _cursorLocations = new Stack<int>();

    private int _maxWordLength;

    public WordSearchController(int maxWordLength)
    {
        _maxWordLength = maxWordLength;
        _cursorLocations.Push(0);
        if (_allWords == null) LoadAllWordsFromText();
    }

    private void LoadAllWordsFromText()
    {
        TextAsset allWordsTextAsset = Resources.Load<TextAsset>("allWords");
        string content = allWordsTextAsset.text;
        _allWords = content.Split("\r\n");
    }

    public bool IsWordMatchFound(string word)
    {
        return _allWords[_cursorLocations.Peek()] == word;
    }
    public void MoveCursorForwards(string word)
    {
        int cursor = _cursorLocations.Peek();
        for (int i = cursor; i < _allWords.Length; i++)
        {
            if (_allWords[i].Length > _maxWordLength) continue;
            int compareResult = string.Compare(word, _allWords[i]);
            if (compareResult == 1) continue;
            _cursorLocations.Push(i);
            return;
        }
        _cursorLocations.Push(_allWords.Length - 1);
    }

    public void RemoveCursors(int countToRemove)
    {
        for (int i = 0; i < countToRemove; i++) _cursorLocations.Pop();
    }
}
