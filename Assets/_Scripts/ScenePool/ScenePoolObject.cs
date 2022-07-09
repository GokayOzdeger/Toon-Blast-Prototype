using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenePoolObject
{
    public GameObject Prefab;
    public List<Quaternion> Rotations = new List<Quaternion>();
    public List<Vector3> Positions = new List<Vector3>();
    public List<Vector3> Scales = new List<Vector3>();

    public ScenePoolObject(GameObject prefab)
    {
        Prefab = prefab;
    }

    public void WriteSceneObject(GameObject obj)
    {
        Rotations.Add(obj.transform.rotation);
        Positions.Add(obj.transform.position);
        Scales.Add(obj.transform.localScale);
    }
}

