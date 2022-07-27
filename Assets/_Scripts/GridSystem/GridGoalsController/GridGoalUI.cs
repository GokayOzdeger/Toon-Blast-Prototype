using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGoalUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text goalAmountLeftText;
    [SerializeField] private Image goalImage;
    [SerializeField] private Image goalCompletedImage;

    public Goal Goal { get; private set; }

    public void SetupGoalUI(Goal goal)
    {
        Goal = goal;
        goalImage.sprite = goal.entityType.DefaultEntitySprite;
        UpdateUIElements();
    }
    
    public void UpdateUIElements()
    {
        Debug.Log("UpdateUIElements");
        if (Goal.GoalLeft == 0)
        {
            goalCompletedImage.enabled = true;
            goalAmountLeftText.text = "";
        }
        else
        {
            goalCompletedImage.enabled = false;
            goalAmountLeftText.text = Goal.GoalLeft.ToString();
        }
    }
}
