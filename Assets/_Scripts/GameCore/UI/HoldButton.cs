using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
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
    private Graphic[] graphicsInChildren;

    protected override void Awake()
    {
        graphicsInChildren = GetComponentsInChildren<Graphic>(true);
        base.Awake();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        Color tintColor = GetStateTrasitionColor(state);

        foreach (Graphic graphic in graphicsInChildren)
        {
            StartColorTween(graphic, tintColor, true);
        }
    }

    private Color GetStateTrasitionColor(SelectionState state)
    {
        switch (state)
        {
            case SelectionState.Normal:
                return colors.normalColor;
            case SelectionState.Highlighted:
                return colors.highlightedColor;
            case SelectionState.Pressed:
                return colors.pressedColor;
            case SelectionState.Selected:
                return colors.selectedColor;
            case SelectionState.Disabled:
                return colors.disabledColor;
            default:
                return Color.black;
        }
    }

    private void StartColorTween(Graphic graphic, Color targetColor, bool instant)
    {
        if (graphic == null)
            return;

        graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
    }

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