using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEvent 
{
    public void StartEvent<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity;

    public void OnEventEnd();
}
