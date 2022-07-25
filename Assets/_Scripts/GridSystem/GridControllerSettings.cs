using System.Collections.Generic;
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
        [SerializeField] /*[SO2DArray]*/private int maxEntitiesPerCollumn;
        
        [BHeader("Grid Frame Settings")]
        [SerializeField] private float gridFrameWidthAdd;
        [SerializeField] private float gridFrameBottomAdd;
        [SerializeField] private float gridFrameTopAdd;

        [Group]
        [SerializeField] private GridEntitySpawner.GridEntitySpawnerSettings gridEntitySpawnerSettings;

        public uint RowCount => rowCount;
        public uint CollumnCount => collumnCount;
        public int MaxEntitiesPerSide => maxEntitiesPerRow;
        public float GridFrameWidthAdd => gridFrameWidthAdd/100;
        public float GridFrameBottomAdd => gridFrameBottomAdd/100;
        public float GridFrameTopAdd => gridFrameTopAdd/100;

        public GridEntitySpawner.GridEntitySpawnerSettings GridEntitySpawnerSettings => gridEntitySpawnerSettings;
    }
}
