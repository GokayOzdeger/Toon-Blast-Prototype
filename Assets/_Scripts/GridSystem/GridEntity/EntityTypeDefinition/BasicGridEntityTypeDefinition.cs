using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Basic Grid Entity Type Definition")]
public class BasicGridEntityTypeDefinition : ScriptableObject, IGridEntityTypeDefinition
{
    [BHeader("Base Grid Entity Settings")]
    [SerializeField] protected GameObject gridEntityPrefab;
    [SerializeField] protected string gridEntityTypeName;
    [SerializeField] protected Sprite defaultSprite;

    public string GridEntityTypeName => gridEntityTypeName;

    public Sprite DefaultEntitySprite => defaultSprite;

    public GameObject GridEntityPrefab => gridEntityPrefab;
}
