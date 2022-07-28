using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridGoalsControllerSettings
{
    [SerializeField] private GameObject uiGoalPrefab;
    [SerializeField] private AudioClip goalCollectAudio;
    [SerializeField] private List<Goal> gridGoals;
    
    public GameObject UIGoalPrefab => uiGoalPrefab;
    public AudioClip GoalCollectAudio => goalCollectAudio;
    public List<Goal> GridGoals => gridGoals;
}
