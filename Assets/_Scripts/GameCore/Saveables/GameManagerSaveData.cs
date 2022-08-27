using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManagerSaveData : Saveable<GameManagerSaveData>
{
    [SerializeField] private int currentLevelIndex = 0;
    public int CurrentLevelIndex => currentLevelIndex;

    public void ProgressLevel()
    {
        currentLevelIndex++;
        Save();
    }

    public void ResetLevelIndex()
    {
        currentLevelIndex = 0;
        Save();
    }
}
