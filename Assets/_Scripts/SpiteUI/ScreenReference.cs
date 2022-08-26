using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Screen Reference")]
public class ScreenReference : ScriptableObject
{
    public Vector2 ScreenResolution;
    [BHeader("Safe Area")]
    public int SafeAreaIntentTop;
    public int SafeAreaIntentRight;
    public int SafeAreaIntentLeft;
    public int SafeAreaIntentBottom;

    public Vector2 SafeArea 
    { 
        get
        {
            return new Vector2(ScreenResolution.x - SafeAreaIntentLeft - SafeAreaIntentRight, ScreenResolution.y - SafeAreaIntentBottom - SafeAreaIntentTop);
        } 
    }
}
