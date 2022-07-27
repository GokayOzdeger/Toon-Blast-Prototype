using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Rocket Type Definition")]
public class RocketTypeDefinition : BasicGridEntityTypeDefinition
{
    [SerializeField] private GameObject rocketExplodeAnimPrefab;

    public GameObject RocketExplodeAnimPrefab => rocketExplodeAnimPrefab;
}
