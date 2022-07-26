using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGoalUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text goalAmountLeftText;
    [SerializeField] private Image goalImage;
    [SerializeField] private Image goalCompletedImage;

    public void UpdateGoalAmountSprite(Sprite sprite)
    {
        goalImage.sprite = sprite;
    }
    public void UpdateGoalAmount(int goalAmount)
    {
        if(goalAmount == 0)
        {
            goalCompletedImage.enabled = true;
            goalAmountLeftText.text = "";
        }
        else
        {
            goalCompletedImage.enabled = false;
            goalAmountLeftText.text = goalAmount.ToString();
        }
    }
}
