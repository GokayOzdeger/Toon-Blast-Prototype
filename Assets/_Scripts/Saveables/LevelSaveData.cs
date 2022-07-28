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
        CollectGridControllerDatas(controller);

        CollectMovesControllerDatas(controller);

        CollectGridGoalControllerDatas(controller);

        foreach (var name in SavedGrid) Debug.Log(name);
        Save();
    }

    public void ClearSavedLevelState()
    {
        SavedGrid = null;
        MovesLeft = 0;
        GoalAmountsLeft = null;
        Save();
    }

    private void CollectGridGoalControllerDatas(LevelController controller)
    {
        // cache grid entity type names
        List<Goal> goals = controller.GridGoalController.GridGoals;
        GoalAmountsLeft = new int[goals.Count];
        for (int i = 0; i < goals.Count; i++)
        {
            Debug.Log("Save : " + i);
            GoalAmountsLeft[i] = goals[i].GoalLeft;
        }
    }

    private void CollectMovesControllerDatas(LevelController controller)
    {
        // cache moves left
        MovesLeft = controller.MovesController.MovesLeft;
    }

    private void CollectGridControllerDatas(LevelController controller)
    {
        // cache goal amounts left
        IGridEntity[,] grid = controller.GridController.EntityGrid;
        SavedGrid = new string[grid.Length];
        
        int currentIndex = 0;
        foreach (IGridEntity entity in grid)
        {
            if (entity != null)
            {
                SavedGrid[currentIndex] = entity.EntityType.GridEntityTypeName;
            }
            currentIndex++;
        }

    }
}
