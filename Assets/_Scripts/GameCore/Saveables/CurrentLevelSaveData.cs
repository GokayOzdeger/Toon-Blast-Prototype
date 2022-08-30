using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelSaveData : Saveable<CurrentLevelSaveData>
{
    public bool HasLevelSaved => TilesLeft != null;

    // TileController Save Data
    public List<TileData> TilesLeft;

    // WordController Save Data
    private List<string> SubmittedWords;

    // ScoreController Save Data
    public int CurrentTotalScore;

    public void SaveLevelState(LevelController controller)
    {
        SaveTileController(controller.TileController);
        SaveWordController(controller.WordController);
        SaveScoreController(controller.ScoreController);
        Save();
    }

    private void SaveTileController(TileController tileController)
    {
        TilesLeft = new List<TileData>();
        foreach(ITile tile in tileController.AllTiles)
        {
            if (tile.IsRemovedFromPlay) continue;
            TilesLeft.Add(tile.TileData);
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

    public void ClearSavedLevelState()
    {
        TilesLeft = null;
        SubmittedWords = null;
        CurrentTotalScore = 0;
        Save();
    }

}
