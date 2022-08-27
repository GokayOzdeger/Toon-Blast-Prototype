using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBackground : MonoBehaviour
{
    [SerializeField] private ScaleMode scaleMode;
    [SerializeField] private bool inSafeArea;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CalculateScale();
    }

    private void CalculateScale()
    {
        Vector2 screenResolution;
        
        if (inSafeArea) screenResolution = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
        else screenResolution = new Vector2(Screen.width, Screen.height);

        float scaleMultiplier = CalculateScaleMultiplier(screenResolution, scaleMode);
        transform.localScale = transform.localScale * scaleMultiplier;
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
                multiplier = Mathf.Min(GetWidthScaleMultiplier(current), GetHeightScaleMultiplier(current));
                break;
            case ScaleMode.ScaleForBiggest:
                multiplier = Mathf.Max(GetWidthScaleMultiplier(current), GetHeightScaleMultiplier(current));
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
        if (inSafeArea) multiplier = current.x / spriteRenderer.bounds.size.x;
        else multiplier = current.x / spriteRenderer.bounds.size.x;
        return multiplier;
    }

    private float GetHeightScaleMultiplier(Vector2 current)
    {
        float multiplier;
        if (inSafeArea) multiplier = current.y / spriteRenderer.bounds.size.y;
        else multiplier = current.y / spriteRenderer.bounds.size.y;
        return multiplier;
    }

    public enum ScaleMode
    {
        ScaleForWidth,
        ScaleForHeight,
        ScaleForSmallest,
        ScaleForBiggest,
    }
}
