using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelComponent
{
    void OnLevelStart();
    void OnLevelEnd();
    void OnLevelPause();
}
