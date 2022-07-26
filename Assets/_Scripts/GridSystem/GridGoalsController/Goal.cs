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
