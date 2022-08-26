using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level Config")]
public class LevelConfig : ScriptableObject
{
    [BHeader("Level Configs")]
    [SerializeField] private string levelTitle;
    [SerializeField][Group] private TileControllerConfig tileManagerConfig;
    [SerializeField][Group] private WordControllerConfig wordControllerConfig;

    public string LevelTitle => levelTitle;
    public TileControllerConfig TileManagerConfig => tileManagerConfig;
    public WordControllerConfig WordControllerConfig => wordControllerConfig;


    #region EDITOR
#if UNITY_EDITOR

    [EasyButtons.Button("Cache Possible Words In Level")]
    private void FindAllPossibleWords()
    {
        TextAsset allWordsRaw = Resources.Load("allWords.txt") as TextAsset;
        string[] AllWords = allWordsRaw.text.Split('\n');
        // use auto solver to find all possible words
    }

    [EasyButtons.Button("Load From Json")]
    private void LoadFromJson(TextAsset json)
    {
        Debug.Log("Loading from json...");
        LevelJson jsonRead = JsonUtility.FromJson<LevelJson>(json.text);
        levelTitle = jsonRead.title;
        tileManagerConfig.SaveTileDataEditor(jsonRead.tiles);
    }

    private class LevelJson
    {
        public string title;
        public TileData[] tiles;
    }

#endif
    #endregion
}
