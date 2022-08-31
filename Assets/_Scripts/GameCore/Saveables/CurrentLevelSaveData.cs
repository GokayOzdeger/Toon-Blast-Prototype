using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelSaveData : Saveable<CurrentLevelSaveData>
{
    public bool HasSavedLevel;

    // TileController Save Data
    public List<int> TilesLeft;

    // WordController Save Data
    public List<string> SubmittedWords;

    // ScoreController Save Data
    public int CurrentTotalScore;

    public void SaveLevelState(LevelController controller)
    {
        SaveTileController(controller.TileController);
        SaveWordController(controller.WordController);
        SaveScoreController(controller.ScoreController);
        HasSavedLevel = true;
        Save();
    }

    private void SaveTileController(TileController tileController)
    {
        TilesLeft = new List<int>();
        foreach(ITile tile in tileController.AllTiles)
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
        TilesLeft = null;
        SubmittedWords = null;
        CurrentTotalScore = 0;
        Save();
    }

}
