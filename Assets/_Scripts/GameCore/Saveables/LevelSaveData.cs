using SaveSystem;

public class LevelSaveData : SaveableWithKey<LevelSaveData>
{
    public int HighScore;

    public string LevelTitle;
    public bool IsCompleted => HighScore != 0;

    public static void SaveLevelData(string levelTitle, int highScore)
    {
        LevelSaveData data = Data(levelTitle);
        data.LevelTitle = levelTitle;
        data.HighScore = highScore;
        Save(levelTitle);
    }
}