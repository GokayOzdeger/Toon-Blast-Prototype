using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    /// <summary>
    ///     Called when the object goes to the pool
    /// </summary>
    void OnGoToPool();

    /// <summary>
    ///     Called when the object is called from the pool
    /// </summary>
    void OnPoolSpawn();
}