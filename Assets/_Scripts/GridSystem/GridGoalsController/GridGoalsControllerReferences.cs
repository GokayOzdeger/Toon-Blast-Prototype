using UnityEngine;

[System.Serializable]
public class GridGoalsControllerReferences
{
    [SerializeField] private RectTransform goalObjectsParent;
    public RectTransform GoalObjectsParent => goalObjectsParent;
}
