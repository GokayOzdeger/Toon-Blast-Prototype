using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ShuffleControllerSceneReferences
{
    [BHeader("Shuffle Controller References")]
    [SerializeField] private Button forceShuffleButton;
    public Button ForceShuffleButton => forceShuffleButton;
}

