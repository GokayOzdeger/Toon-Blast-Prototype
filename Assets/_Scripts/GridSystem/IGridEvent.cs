using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEvent 
{
    public bool ProceedGridAfterEventEnds { get; }
    public bool TryEventStart<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity;

    public void OnEventEnd();
}
