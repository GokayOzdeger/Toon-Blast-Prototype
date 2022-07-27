using UnityEngine;


[System.Serializable]
public class GridEntitySpawnerSceneReferences
{
    [BHeader("Grid Entity Spawner References")]
    [SerializeField] RectTransform gridParentTransform;
    public RectTransform GridParentTransform => gridParentTransform;
}

