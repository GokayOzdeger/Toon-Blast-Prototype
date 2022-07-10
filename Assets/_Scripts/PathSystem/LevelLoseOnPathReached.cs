using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/ LevelLoseOnLastPathReached")]
public class LevelLoseOnPathReached : ALevelBehaviourSO
{
    public override void OnSetup()
    {
        RegisterLevelLoseToFinalNodes();
    }

    private void RegisterLevelLoseToFinalNodes()
    {
        foreach (PathNode finalNode in PathManager.Instance.FinalNodes)
        {
            finalNode.OnPathReached.RemoveAllListeners();
            finalNode.OnPathReached.AddListener(LoseLevel);
        }
    }

    public override void OnTick(float deltaTime)
    {
        //
    }

    private void LoseLevel(PathMovementBehaviour movementBehaviour)
    {
        GameManager.Instance.CurrentLevel.LostLevel();
    }
}
