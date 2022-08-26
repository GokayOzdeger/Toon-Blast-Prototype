using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraSizeScreenSize : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Camera>().orthographicSize = Mathf.Max(Screen.width, Screen.height)/2;       
    }
}
