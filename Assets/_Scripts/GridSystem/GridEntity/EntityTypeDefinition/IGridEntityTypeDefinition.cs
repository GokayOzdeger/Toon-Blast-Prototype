using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEntityTypeDefinition
{
    public bool EntityIncludedInShuffle { get; }
    public GameObject GridEntityPrefab { get; }
    public string GridEntityTypeName { get; }
    public Sprite DefaultEntitySprite { get; }
}
