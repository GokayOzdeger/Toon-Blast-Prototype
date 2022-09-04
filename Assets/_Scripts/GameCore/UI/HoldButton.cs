using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : Button
{
    private readonly UnityEvent _onHold = new();
    private Tween _activeTween;

    private bool _canBeHold;
    private Graphic[] _graphicsInChildren;
    private bool _registeredHold;
    private float _timeSinceStartedHolding;

    public UnityEvent OnHold
    {
        get
        {
            _canBeHold = true;
            return _onHold;
        }
    }

    public float HoldRegisterDuration { get; set; } = .3f;
    private bool HoveringButton { get; set; }
    private bool HoldingButton { get; set; }

    protected override void Awake()
    {
        _graphicsInChildren = GetComponentsInChildren<Graphic>(true);
        base.Awake();
    }

    public void Update()
    {
        if (!HoldingButton || !HoveringButton || _registeredHold || !interactable || !_canBeHold) return;
        _timeSinceStartedHolding += Time.deltaTime;
        if (_timeSinceStartedHolding > HoldRegisterDuration)
        {
            AnimateClick();
            _registeredHold = true;
            _onHold.Invoke();
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        Color tintColor = GetStateTrasitionColor(state);

        foreach (Graphic graphic in _graphicsInChildren) StartColorTween(graphic, tintColor, true);
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

    private void CompleteLastTween()
    {
        if (_activeTween == null) return;
        _activeTween.Complete(true);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // remove default behaviour of button
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _registeredHold = false;
        _timeSinceStartedHolding = 0;
        HoldingButton = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        HoldingButton = false;
        if (!interactable) return;
        if (!HoveringButton) return;
        if (_registeredHold) return;
        AnimateClick();
        onClick.Invoke();
    }

    private void AnimateClick()
    {
        CompleteLastTween();
        _activeTween = TweenHelper.PunchScale(transform);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        HoveringButton = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        HoveringButton = false;
    }
}