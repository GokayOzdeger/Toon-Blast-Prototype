using System.Collections.Generic;
using SaveSystem;
using UnityEngine;

public class GameManagerSaveData : Saveable<GameManagerSaveData>
{
    [SerializeField] private readonly List<LevelSaveData> levelSaveDatas = new();

    public List<LevelSaveData> LevelSaveDatas => levelSaveDatas;

    public void SaveLevelData()
    {
    }


    public class LevelSaveData : SaveableWithKey<LevelSaveData>
    {
        public int highScore;
        public string levelTitle;
    }
}