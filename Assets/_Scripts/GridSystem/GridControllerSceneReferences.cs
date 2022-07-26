using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
#endif

public partial class GridController
{
    [System.Serializable]
    public class GridControllerSceneReferences
    {
        [BHeader("Grid Controller References")] 
        [SerializeField] private RectTransform gridRect;
        [SerializeField] private RectTransform gridOverlay;
        [SerializeField] RectTransform gridFrame;
        [SerializeField] private CanvasScaler canvasScaler;
        [Group]
        [SerializeField] private GridEntitySpawner.GridEntitySpawnerSceneReferences gridEntitySpawnerSceneReferences;
        [Group]
        [SerializeField] private ShuffleController.ShuffleControllerSceneReferences shuffleControllerSceneReferences;
        public RectTransform GridRect => gridRect;
        public RectTransform GridOverlay => gridOverlay;
        public RectTransform GridFrame => gridFrame;
        public CanvasScaler CanvasScaler => canvasScaler;
        public GridEntitySpawner.GridEntitySpawnerSceneReferences GridEntitySpawnerSceneReferences => gridEntitySpawnerSceneReferences;
        public ShuffleController.ShuffleControllerSceneReferences ShuffleControllerSceneReferences => shuffleControllerSceneReferences;
    }
}
