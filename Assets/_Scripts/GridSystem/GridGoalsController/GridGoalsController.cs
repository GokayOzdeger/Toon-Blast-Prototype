using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class GridGoalsController
{
    private List<Goal> GridGoals { get; set; }
    private List<GridGoalUI> GridGoalUiElements { get; set; }

    private RectTransform _gridUiElementsParent;
    private GameObject _gridUiElementPrefab;
    
    public GridGoalsController(GridGoalsControllerSettings settings, GridGoalsControllerReferences references)
    {
        this._gridUiElementsParent = references.GoalObjectsParent;
        this._gridUiElementPrefab = settings.UIGoalPrefab;
        GridGoals = new List<Goal>(settings.GridGoals);
        GridGoalUiElements = new List<GridGoalUI>();

        StartAllGoals();
        SpawnUiElements();
    }

    public void OnEntityDestroyed(IGridEntity entity)
    {
        for (int i = 0; i < GridGoals.Count; i++)
        {
            if (GridGoals[i].entityType.GridEntityTypeName == entity.EntityType.GridEntityTypeName)
            {
                GridGoals[i].DecreaseGoal();
                GridGoalUiElements[i].UpdateGoalAmount(GridGoals[i].GoalLeft);
                if (GridGoals[i].IsCompleted) CheckAllGoalsCompleted();
            }
        }
    }

    private void StartAllGoals()
    {
        foreach (Goal goal in GridGoals) goal.StartGoal();
    }

    private void SpawnUiElements()
    {
        foreach (Goal goal in GridGoals)
        {
            GameObject newGo = ObjectPooler.Instance.Spawn(_gridUiElementPrefab.name, _gridUiElementsParent.position);
            newGo.transform.SetParent(_gridUiElementsParent);
            GridGoalUI goalUi = newGo.GetComponent<GridGoalUI>();
            goalUi.UpdateGoalAmountSprite(goal.entityType.DefaultEntitySprite);
            goalUi.UpdateGoalAmount(goal.GoalLeft);
            GridGoalUiElements.Add(goalUi);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_gridUiElementsParent);
    }

    private void CheckAllGoalsCompleted()
    {
        foreach (Goal goal in GridGoals) if (!goal.IsCompleted) return;
        Debug.Log("All Goals Completed !!!");
    }
}

[System.Serializable]
public class GridGoalsControllerReferences
{
    [SerializeField] private RectTransform goalObjectsParent;
    public RectTransform GoalObjectsParent => goalObjectsParent;
}

[System.Serializable]
public class GridGoalsControllerSettings
{
    [SerializeField] private GameObject uiGoalPrefab;
    [SerializeField] private List<Goal> gridGoals;
    
    public GameObject UIGoalPrefab => uiGoalPrefab;
    public List<Goal> GridGoals => gridGoals;
}


[System.Serializable]
public class Goal
{
    public BasicGridEntityTypeDefinition entityType;
    public int goalAmount;
    
    [System.NonSerialized]
    private int goalLeft;

    public bool IsCompleted => goalLeft <= 0;
    public int GoalLeft => goalLeft;

    public void StartGoal()
    {
        goalLeft = goalAmount;
    }

    public void DecreaseGoal()
    {
        goalLeft--;
        if (goalLeft < 0) goalLeft = 0;
    }
}
