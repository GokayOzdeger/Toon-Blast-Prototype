using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/")]
public abstract class ALevelBehaviourSO : ScriptableObject, ILevelBehaviour
{
    public virtual bool IsActive { get; protected set; } = true;

    protected LevelConfig Config { get; set; }
    protected LevelController.LevelSceneReferences SceneReferences {get; set;}

    public virtual void Setup(LevelConfig config, LevelController.LevelSceneReferences sceneReferences)
    {
        Config = config;
        SceneReferences = sceneReferences;
        OnSetup();
    }
    
    public virtual void Start()
    {
        IsActive = true;
    }

    public virtual void Stop()
    {
        IsActive = false;
    }

    public virtual void Tick(float deltaTime)
    {
        if (!IsActive) return;
        OnTick(deltaTime);
    }
    
    public abstract void OnSetup();
    public abstract void OnTick(float deltaTime);
}
