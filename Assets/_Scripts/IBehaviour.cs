using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/")]
public interface IBehaviour
{
    public bool IsActive { get; }
    
    public void Setup(BehaviourController controller);

    public void StartBehaviour();

    public void StopBehaviour();
}
