using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManagerSaveData : Saveable<GameManagerSaveData>
{
    [SerializeField] private List<LevelSaveData> levelSaveDatas = new List<LevelSaveData>();

    public List<LevelSaveData> LevelSaveDatas => levelSaveDatas;

    public void SaveLevelData()
    {

    }


    public class LevelSaveData : SaveableWithKey<LevelSaveData>
    {
        public string levelTitle;
        public int highScore;
    }
}
