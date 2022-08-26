using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData 
{
    [SerializeField] private int id;
    [SerializeField] private Vector3 position;
    [SerializeField] private string character;
    [SerializeField] private int[] children;

    public int Id => id;
    public Vector3 Position => position;
    public string Character => character;
    public int[] Children => children;

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
    }
}
