using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletUIAdjuster : MonoBehaviour
{
    [SerializeField] private Vector2 scaleMultiplier = Vector2.one;

    [SerializeField] private Vector2 offset = Vector2.zero;
    // Start is called before the first frame update
    private void Start()
    {
        if(!DeviceManager.Instance.IsTablet) return;
        
        transform.position = offset + (Vector2)transform.position ;
        transform.localScale *= scaleMultiplier;
    }
}
