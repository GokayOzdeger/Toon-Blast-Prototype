using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridControllerSettings
{
    [BHeader("Grid Controller Settings")]
    [SerializeField] private uint rowCount;
    [SerializeField] private uint collumnCount;
    [SerializeField] private int maxEntitiesPerRow;
    [SerializeField] private int maxEntitiesPerCollumn;
        
    [BHeader("Grid Frame Settings")]
    [SerializeField] private float gridFrameWidthAdd;
    [SerializeField] private float gridFrameBottomAdd;
    [SerializeField] private float gridFrameTopAdd;

    public uint RowCount => rowCount;
    public uint CollumnCount => collumnCount;
    public int MaxEntitiesPerSide => maxEntitiesPerRow;
    public float GridFrameWidthAdd => gridFrameWidthAdd/100;
    public float GridFrameBottomAdd => gridFrameBottomAdd/100;
    public float GridFrameTopAdd => gridFrameTopAdd/100;
}

