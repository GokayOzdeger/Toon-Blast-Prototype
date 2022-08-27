using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSaveData : Saveable<LevelSaveData>
{
    public bool HasLevelSaved => SavedGrid != null;

    // GridController Save Data
    public string[] SavedGrid;

    // MovesController Save Data
    public int MovesLeft;

    // GridGoalController Save Data
    public int[] GoalAmountsLeft;

    public void SaveLevelState(LevelController controller)
    {

        Save();
    }

    public void ClearSavedLevelState()
    {
        SavedGrid = null;
        MovesLeft = 0;
        GoalAmountsLeft = null;
        Save();
    }

}
