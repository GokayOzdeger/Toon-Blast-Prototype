using EasyButtons;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObjects/Level Config")]
public class LevelConfig : ScriptableObject
{
    [BHeader("Level Configs")] [SerializeField]
    private string levelTitle;

    [SerializeField] [Group] private TileControllerConfig tileManagerConfig;
    [SerializeField] [Group] private WordControllerConfig wordControllerConfig;

    public string LevelTitle => levelTitle;
    public TileControllerConfig TileManagerConfig => tileManagerConfig;
    public WordControllerConfig WordControllerConfig => wordControllerConfig;


    #region EDITOR

#if UNITY_EDITOR
    [Button("Load From Json")]
    private void LoadFromJson(TextAsset json)
    {
        Debug.Log("Loading from json...");
        EditorUtility.SetDirty(this);
        var jsonRead = JsonUtility.FromJson<LevelJson>(json.text);
        levelTitle = jsonRead.Title;
        tileManagerConfig.SaveTileDataEditor(jsonRead.Tiles);
    }

    [Button("Run Auto Solver")]
    public void AutoSolve()
    {
        Debug.Log("Started Auto Solver...");
        var allWordsTextAsset = Resources.Load<TextAsset>("allWords");
        string content = allWordsTextAsset.text;
        string[] allWords = content.Split("\r\n");
        var solver = new AutoSolver(allWords, TileManagerConfig, WordControllerConfig);
        solver.StartAutoSolver();
    }

    private class LevelJson
    {
        public TileData[] Tiles;
        public string Title;
    }

#endif

    #endregion
}