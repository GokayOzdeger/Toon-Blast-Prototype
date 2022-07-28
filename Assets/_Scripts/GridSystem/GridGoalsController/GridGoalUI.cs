using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class GridGoalUI : MonoBehaviour
{
    [SerializeField] private PoolObject poolObject;
    [SerializeField] private GameObject particleEffectPrefab;
    [SerializeField] private TMPro.TMP_Text goalAmountLeftText;
    [SerializeField] private Image goalImage;
    [SerializeField] private Image goalCompletedImage;

    public Goal Goal { get; private set; }

    public void SetupGoalUI(Goal goal)
    {
        Goal = goal;
        goalImage.sprite = goal.entityType.DefaultEntitySprite;
        SetGoalAmount(goal.GoalLeft, false);
    }
    
    public void SetGoalAmount(int goalAmount, bool playParticles = true)
    {
        if (playParticles) ObjectPooler.Instance.Spawn(particleEffectPrefab.name, transform.position);
        if (goalAmount == 0)
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

    public void GoToPool()
    {
        poolObject.GoToPool();
    }
}
