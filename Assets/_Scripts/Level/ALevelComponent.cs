using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ALevelComponent : ILevelComponent
{
    public abstract void OnLevelEnd();

    public abstract void OnLevelPause();

    public abstract void OnLevelStart();
}
