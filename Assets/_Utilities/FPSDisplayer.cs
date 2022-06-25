using UnityEngine;
using System.Collections;
using TMPro;
public class FPSDisplayer : MonoBehaviour
{
    #region Fields

    float deltaTime = 0.0f;
    private TextMeshProUGUI textPro;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        textPro = GetComponent<TextMeshProUGUI>();

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        
        
    }


    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{1:0.} fps ({0:0.0} ms)", msec, fps);
        textPro.text = text;
    }
    #endregion
}
