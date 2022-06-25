using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    [SerializeField] private bool workOnStart = false;
    [SerializeField] private float delay = 0;

    public void Start()
    {
        if (workOnStart)
        {
            Invoke("Disable", delay);
        }
    }

    public void DisableAfter(float _delay)
    {
        Invoke("Disable", _delay);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        Destroy(this);
    }
}
