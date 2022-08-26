using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraSizeScreenSize : MonoBehaviour
{
    void Start()
    {
        GetComponent<Camera>().orthographicSize = Mathf.Max(Screen.width, Screen.height)/2;       
    }
}
