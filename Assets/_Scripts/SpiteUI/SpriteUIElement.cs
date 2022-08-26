using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUIElement : MonoBehaviour
{
    [SerializeField] private ScaleMode scaleMode;
    [SerializeField] private ScreenReference screenReference;
    [SerializeField] private bool inSafeArea;
    private Vector3 defaultScale;

    void Start()
    {
        defaultScale = transform.localScale;
        CalculatePosition();
        //CalculateScale();
        Debug.Log(Screen.safeArea.width);
        Debug.Log(Screen.safeArea.height);
    }

    private void CalculatePosition()
    {

    }   

    private void CalculateScale()
    {
        Vector2 screenResolution;
        
        if (inSafeArea) screenResolution = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
        else screenResolution = new Vector2(Screen.width, Screen.height);

        float scaleMultiplier = CalculateScaleMultiplier(screenResolution, scaleMode);
        transform.localScale = defaultScale * scaleMultiplier;
    }

    private float CalculateScaleMultiplier(Vector2 current, ScaleMode mode)
    {
        float multiplier;
        switch (mode)
        {
            case ScaleMode.ScaleForWidth:
                multiplier = GetWidthScaleMultiplier(current);
                break;
            case ScaleMode.ScaleForHeight:
                multiplier = GetHeightScaleMultiplier(current);
                break;
            case ScaleMode.ScaleForSmallest:
                float multiplierForWidth = GetWidthScaleMultiplier(current);
                float multiplierForHeight = GetHeightScaleMultiplier(current);
                multiplier = Mathf.Min(multiplierForWidth, multiplierForHeight);
                break;
            default:
                multiplier = 1;
                break;
        }
        return multiplier;
    }

    private float GetWidthScaleMultiplier(Vector2 current)
    {
        float multiplier;
        if (inSafeArea) multiplier = current.x / screenReference.SafeArea.x;
        else multiplier = current.x / screenReference.ScreenResolution.x;
        return multiplier;
    }

    private float GetHeightScaleMultiplier(Vector2 current)
    {
        float multiplier;
        if (inSafeArea) multiplier = current.y / screenReference.SafeArea.y;
        else multiplier = current.y / screenReference.ScreenResolution.y;
        return multiplier;
    }

    public enum ScaleMode
    {
        ScaleForWidth,
        ScaleForHeight,
        ScaleForSmallest
    }
}
