using UnityEngine;

[System.Serializable]
public class MovesControllerSettings
{
    [SerializeField] private int moveCount;
    public int MoveCount => moveCount;
}
