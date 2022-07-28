using UnityEngine;

[System.Serializable]
public class MovesControllerSceneReferences
{
    [SerializeField] private TMPro.TMP_Text movesLeftText;
    public TMPro.TMP_Text MovesLeftText => movesLeftText;
}
