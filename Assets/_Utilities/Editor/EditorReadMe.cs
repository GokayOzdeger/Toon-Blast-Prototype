using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.EditorApplication;

[InitializeOnLoad]
public static class EditorReadMe
{
    static EditorReadMe()
    {
        EditorApplication.update += OpenEditorReadMeWindowOnEditorLaunch;
    }
    
    public static void OpenEditorReadMeWindowOnEditorLaunch()
    {
        EditorApplication.update -= OpenEditorReadMeWindowOnEditorLaunch;
        if (!SessionState.GetBool("FirstInitDone", false))
        {
            EditorWindow.GetWindow<EditorReadMeWindow>();
            SessionState.SetBool("FirstInitDone", true);
        }
    }
}

public class EditorReadMeWindow : EditorWindow
{
    private void OnEnable()
    {
        position = new Rect(300, 200, 600, 400);
    }

    private void OnDisable()
    {
        EditorApplication.Beep();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 600, 150), 
           "Most Settings Can be configured from the 'Level Config' scriptable object in Resources.\n" +
           "The Level Config SO should be assigned to the GameManager in the scene." +
           "\n" +
           "\nMore configurations can be found in the following scriptable objects:" +
           "\n-> GridEntityDefinitions" +
           "\n-> Conditions");
    }
}
