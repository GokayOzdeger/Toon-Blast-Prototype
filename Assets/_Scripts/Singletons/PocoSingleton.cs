using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocoSingleton<T>
{
    public static T Instance { get; protected set; }
}
