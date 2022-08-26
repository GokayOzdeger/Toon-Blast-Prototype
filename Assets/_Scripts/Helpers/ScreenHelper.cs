using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenHelper
{
    public static Vector2 GetScreenPercentage(Vector2 percentage)
    {
        return new Vector2(Screen.width * percentage.x/100, Screen.height * percentage.y/100);
    }

    public static float GetWidthPercentage(float percentage)
    {
        return Screen.width * percentage/100;
    }

    public static float GetHeightPercentage(float percentage)
    {
        return Screen.height * percentage/100;
    }
}
