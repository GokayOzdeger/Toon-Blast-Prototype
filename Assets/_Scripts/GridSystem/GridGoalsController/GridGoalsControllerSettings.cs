using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridGoalsControllerSettings
{
    [SerializeField] private GameObject uiGoalPrefab;
    [SerializeField] private List<Goal> gridGoals;
    
    public GameObject UIGoalPrefab => uiGoalPrefab;
    public List<Goal> GridGoals => gridGoals;
}
