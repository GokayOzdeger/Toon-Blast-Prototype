using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Basic Grid Entity Type Definition")]
public class BasicGridEntityTypeDefinition : ScriptableObject, IGridEntityTypeDefinition
{
    [BHeader("Base Grid Entity Settings")]
    [SerializeField] protected string gridEntityTypeName;
    [SerializeField] protected Sprite defaultSprite;

    public string GridEntityTypeName => throw new System.NotImplementedException();

    public Sprite DefaultEntitySprite => throw new System.NotImplementedException();
}
