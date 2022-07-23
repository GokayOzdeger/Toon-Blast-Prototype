using UnityEngine;
#if UNITY_EDITOR
#endif

public partial class GridController
{
    [System.Serializable]
    public class GridControllerSettings
    {
        [BHeader("Grid Controller Settings")]
        [SerializeField] private uint rowCount;
        [SerializeField] private uint collumnCount;
        [SerializeField] private int maxEntitiesPerRow;
        [SerializeField] private int maxEnttiesPerCollumn;
        [BHeader("Grid Frame Settings")]
        [SerializeField] private float gridFrameWidthAdd;
        [SerializeField] private float gridFrameHeightAdd;
        [Group]
        [SerializeField] private GridEntitySpawner.GridEntitySpawnerSettings gridEntitySpawnerSettings;

        public uint RowCount => rowCount;
        public uint CollumnCount => collumnCount;
        public int MaxEntitiesPerSide => maxEntitiesPerRow;
        public float GridFrameWidthAdd => gridFrameWidthAdd;
        public float GridFrameHeightAdd => gridFrameHeightAdd;

        public GridEntitySpawner.GridEntitySpawnerSettings GridEntitySpawnerSettings => gridEntitySpawnerSettings;
    }
}
