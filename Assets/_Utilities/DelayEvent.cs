using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private UnityEvent events; 
    void Start()
    {
        Invoke(nameof(InvokeEvents), delay);
    }

    private void InvokeEvents()
    {

        events.Invoke();
    }

}
