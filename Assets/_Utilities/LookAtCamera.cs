using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 angleOffset;
    void Update()
    {
        transform.LookAt(_camera.transform);
        transform.Rotate(angleOffset);
    }
}
