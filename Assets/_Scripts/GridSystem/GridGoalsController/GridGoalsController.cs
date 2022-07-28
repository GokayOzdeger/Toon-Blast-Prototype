using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class GridGoalsController : PocoSingleton<GridGoalsController>
{
    public List<Goal> GridGoals { get; private set; }
    private List<GridGoalUI> GridGoalUiElements { get; set; }

    private RectTransform _gridUiElementsParent;
    private GameObject _gridUiElementPrefab;
    
    public GridGoalsController(GridGoalsControllerSettings settings, GridGoalsControllerReferences references)
    {
        Instance = this;
        this._gridUiElementsParent = references.GoalObjectsParent;
        this._gridUiElementPrefab = settings.UIGoalPrefab;
        GridGoals = new List<Goal>(settings.GridGoals);
        GridGoalUiElements = new List<GridGoalUI>();

        StartAllGoals();
        TryLoadGoalSaveData();
        SpawnUiElements();
    }

    public void OnEntityDestroyed(IGridEntity entity)
    {
        for (int i = 0; i < GridGoals.Count; i++)
        {
            Goal goal = GridGoals[i];
            GridGoalUI goalUI = GridGoalUiElements[i];

            if (goal.IsCompleted) continue;
            if (goal.entityType.GridEntityTypeName == entity.EntityType.GridEntityTypeName)
            {
                goal.DecreaseGoal();
                CreateFlyingSpriteToGoal(entity, goalUI);
                if (goal.IsCompleted) CheckAllGoalsCompleted();
            }
        }
    }

    public void CreateFlyingSpriteToGoal(IGridEntity entity, GridGoalUI goalUI)
    {
        UIEffectsManager.Instance.CreateCurvyFlyingSprite(
            entity.EntityType.DefaultEntitySprite,
            entity.EntityTransform.GetComponent<RectTransform>().sizeDelta * 1.25f, // create bigger flying image for better visual representation
            entity.EntityTransform.position, 
            goalUI.transform.position, 
            UIEffectsManager.CanvasLayer.OverEverything, 
            ()=> goalUI.UpdateUIElements());
    }

    public void ClearUIElementsOnLevelEnd()
    {
        for (int i = 0; i < GridGoalUiElements.Count; i++)
        {
            GridGoalUiElements[i].GoToPool();
        }
    }

    private void StartAllGoals()
    {
        foreach (Goal goal in GridGoals) goal.StartGoal();
    }

    private void TryLoadGoalSaveData()
    {
        if (!LevelSaveData.Data.HasLevelSaved) return;
        for (int i = 0; i < GridGoals.Count; i++)
        {
            Goal goal = GridGoals[i];
            goal.LoadGoalAmountLeft(LevelSaveData.Data.GoalAmountsLeft[i]);
        }
    }

    private void SpawnUiElements()
    {
        foreach (Goal goal in GridGoals)
        {
            GameObject newGo = ObjectPooler.Instance.Spawn(_gridUiElementPrefab.name, _gridUiElementsParent.position);
            newGo.transform.SetParent(_gridUiElementsParent);
            GridGoalUI goalUi = newGo.GetComponent<GridGoalUI>();
            goalUi.transform.localScale = Vector3.one;
            goalUi.SetupGoalUI(goal);
            GridGoalUiElements.Add(goalUi);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_gridUiElementsParent);
    }

    private void CheckAllGoalsCompleted()
    {
        foreach (Goal goal in GridGoals) if (!goal.IsCompleted) return;
        Debug.Log("All Goals Completed !!!");
        GameManager.Instance.CurrentLevel.LevelCleared();
    }
}
