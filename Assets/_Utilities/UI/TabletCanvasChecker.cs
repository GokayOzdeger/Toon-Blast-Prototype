using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class TabletCanvasChecker : MonoBehaviour
{
    private const float scaleFactorPerInch = .05f;
    void Awake()
    {
        //if (Application.isEditor) return;
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        if (DeviceManager.Instance.IsTablet)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = Mathf.Clamp((DeviceManager.Instance.DeviceInches - 6.5f) * scaleFactorPerInch+1, 1, 2);
            Debug.Log($"Canvas mode set to Tablet, scalefactor: {canvasScaler.scaleFactor}");
        }
        else
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            Debug.Log($"Canvas mode set to Phone");
        }

    }
}
