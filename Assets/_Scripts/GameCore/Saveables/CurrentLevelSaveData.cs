using System.Collections.Generic;
using SaveSystem;

public class CurrentLevelSaveData : Saveable<CurrentLevelSaveData>
{
    // ScoreController Save Data
    public int CurrentTotalScore;
    public bool HasSavedLevel;
    public string LevelTitle;

    // WordController Save Data
    public List<string> SubmittedWords;

    // TileController Save Data
    public List<int> TilesLeft;

    public void SaveLevelState(LevelController controller)
    {
        LevelTitle = controller.Config.LevelTitle;
        SaveTileController(controller.TileController);
        SaveWordController(controller.WordController);
        SaveScoreController(controller.ScoreController);
        HasSavedLevel = true;
        Save();
    }

    private void SaveTileController(TileController tileController)
    {
        TilesLeft = new List<int>();
        foreach (ITile tile in tileController.AllTiles)
        {
            if (tile.IsRemovedFromPlay) continue;
            TilesLeft.Add(tile.TileData.Id);
        }
    }

    private void SaveWordController(WordController wordController)
    {
        SubmittedWords = wordController.SubmittedWords;
    }

    private void SaveScoreController(ScoreController scoreController)
    {
        CurrentTotalScore = scoreController.CurrentTotalScore;
    }

    public void ClearSavedLevelStateData()
    {
        HasSavedLevel = false;
        LevelTitle = null;
        TilesLeft = null;
        SubmittedWords = null;
        CurrentTotalScore = 0;
        Save();
    }
}