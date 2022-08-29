using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonGameStateListener<T> : MonoGameStateListener where T : class
{
    private static T _instance;
    public static T Instance => _instance;

    protected override void Awake()
    {
        _instance = this as T;
        base.Awake();
    }
}
