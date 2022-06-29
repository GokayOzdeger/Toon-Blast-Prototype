using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Group]
    [SerializeField] private GridController.GridControllerSettings gridControllerSettings;

    public GridController.GridControllerSettings GridControllerSettings => gridControllerSettings;
}
