using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundTransform : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private float _step;
    private void Awake()
    {
        _step = Time.fixedDeltaTime * rotationSpeed;
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward*_step, Space.Self);
    }
}
