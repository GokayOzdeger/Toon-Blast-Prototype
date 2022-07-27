using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftWithSpeed : MonoBehaviour
{
    [SerializeField] private float speed;

    void Update()
    {
        transform.position += -transform.right * speed * Time.deltaTime;
    }
}
