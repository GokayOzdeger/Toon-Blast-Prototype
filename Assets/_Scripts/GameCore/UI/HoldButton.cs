using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : Button
{
    public UnityEvent onHold = new UnityEvent();
    public float HoldRegisterDuration { get; set; } = .3f;
    private bool HoveringButton { get; set; }
    private bool HoldingButton { get; set; }

    private float timeSinceStartedHolding;
    private bool registeredHold;

    public override void OnPointerDown(PointerEventData eventData)
    {
        registeredHold = false;
        timeSinceStartedHolding = 0;
        HoldingButton = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        HoldingButton = false;
        if (!HoveringButton) return;
        if (registeredHold) return;    
        onClick.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        HoveringButton = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        HoveringButton = false;
    }

    public void Update()
    {
        if (!HoldingButton || !HoveringButton) return;
        timeSinceStartedHolding += Time.deltaTime;
        if(timeSinceStartedHolding> HoldRegisterDuration)
        {
            registeredHold = true;
            onHold.Invoke();
        }
    }

}
