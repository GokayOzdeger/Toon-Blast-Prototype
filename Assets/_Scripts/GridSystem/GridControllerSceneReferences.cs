using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GridControllerSceneReferences
{
    [BHeader("Grid Controller References")] 
    [SerializeField] private RectTransform gridRect;
    [SerializeField] private RectTransform gridOverlay;
    [SerializeField] RectTransform gridFrame;
    [SerializeField] private CanvasScaler canvasScaler;

    public RectTransform GridRect => gridRect;
    public RectTransform GridOverlay => gridOverlay;
    public RectTransform GridFrame => gridFrame;
    public CanvasScaler CanvasScaler => canvasScaler;
}

