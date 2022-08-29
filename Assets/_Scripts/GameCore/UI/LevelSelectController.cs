using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private RectTransform scrollRectContentRect;
    [SerializeField] private GameObject listElementPrefab;

    private void Start()
    {
        foreach(LevelConfig config in LevelManager.Instance.LevelList)
        {
            LevelSelectElement element = ObjectPooler.Instance.Spawn(listElementPrefab.name, Vector3.zero).GetComponent<LevelSelectElement>();
            element.transform.SetParent(scrollRectContentRect, false);
            element.SetupElement(config);
        }
    }
}
