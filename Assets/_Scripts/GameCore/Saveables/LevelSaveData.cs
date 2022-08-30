using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSaveData : SaveableWithKey<LevelSaveData>
{
    public bool IsCompleted => HighScore != 0;

    public string LevelTitle;
    public int HighScore;

    public static void SaveLevelData(string levelTitle, int highScore)
    {
        LevelSaveData data = Data(levelTitle);
        data.LevelTitle = levelTitle;
        data.HighScore = highScore;
        Save(levelTitle);
    }
}
