using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    [EasyButtons.Button("Load From Json")]
    private void LoadFromJson(TextAsset json)
    {
        Debug.Log("Loading from json...");
        LevelJson jsonRead = JsonUtility.FromJson<LevelJson>(json.text);
        levelTitle = jsonRead.title;
        tileManagerConfig.SaveTileDataEditor(jsonRead.tiles);
    }

    [EasyButtons.Button("Run Auto Solver")]
    public void AutoSolve()
    {
        Debug.Log("Started Auto Solver...");
        TextAsset allWordsTextAsset = Resources.Load<TextAsset>("allWords");
        string content = allWordsTextAsset.text;
        string[] allWords = content.Split("\r\n");
        AutoSolver solver = new AutoSolver(allWords, TileManagerConfig, WordControllerConfig);
        solver.StartAutoSolver();
    }

    private class LevelJson
    {
        public string title;
        public TileData[] tiles;
    }

#endif
    #endregion
}
