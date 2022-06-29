using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEntityTypeDefinition
{
    public string GridEntityTypeName { get; }
    public Sprite DefaultEntitySprite { get; }
}
