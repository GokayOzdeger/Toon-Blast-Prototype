using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
public class ReplaceMeshReferences : EditorWindow
{

    public List<Mesh> meshesToReplace = new List<Mesh>();
    public List<Mesh> newMeshes = new List<Mesh>();

    private SerializedObject so;

    private SerializedProperty spMeshesToReplace;
    private SerializedProperty spNewMeshes;

    [MenuItem("Tools/Replace Meshes In All Scenes")]
    static void CreateReplaceWindow()
    {
        EditorWindow.GetWindow<ReplaceMeshReferences>();
    }

    private void OnEnable()
    {
        so = new SerializedObject( (ScriptableObject) this);

        spMeshesToReplace = so.FindProperty("meshesToReplace");
        spNewMeshes = so.FindProperty("newMeshes");
    }
    private void OnGUI()
    {
        EditorGUILayout.PropertyField(spMeshesToReplace, true); // True means show children
        EditorGUILayout.PropertyField(spNewMeshes, true); // True means show children
        so.ApplyModifiedProperties();
        so.Update();




        if (GUILayout.Button("Replace Meshes In All Scenes"))
        {
            string originalScene = EditorSceneManager.GetActiveScene().path;


            foreach (EditorBuildSettingsScene buildSettingsScene in EditorBuildSettings.scenes)
            {
                EditorSceneManager.OpenScene(buildSettingsScene.path);
                Scene scene = EditorSceneManager.GetActiveScene();
                int searches = 0;
                int changeCount = 0;

                List<MeshFilter> meshes = new List<MeshFilter>();

                foreach (MeshFilter mf in Resources.FindObjectsOfTypeAll(typeof(MeshFilter)) as MeshFilter[])
                {
                    if (EditorUtility.IsPersistent(mf.transform.root.gameObject) && !(mf.hideFlags == HideFlags.NotEditable || mf.hideFlags == HideFlags.HideAndDontSave))
                        meshes.Add(mf);
                }

                foreach (MeshFilter filter in meshes)
                {
                    for (int i = 0; i < meshesToReplace.Count; i++)
                    {
                        searches++;
                        if (filter.sharedMesh == meshesToReplace[i])
                        {
                            EditorUtility.SetDirty(filter.gameObject);
                            changeCount++;
                            filter.sharedMesh = newMeshes[i];
                        }
                        if(changeCount>0)Debug.Log($"{meshesToReplace[i].name}| Replaced {changeCount} meshes, Searched {searches} MeshFilters, in Scene: '{scene.name}'");
                    }
                }
                if (changeCount != 0) { EditorSceneManager.SaveScene(scene); }
            }
            if (originalScene != "") EditorSceneManager.OpenScene(originalScene);
        }

        if (GUILayout.Button("Replace Meshes"))
        {
            int searches = 0;
            int changeCount = 0;
            
            List<MeshFilter> meshes = new List<MeshFilter>();

            foreach (MeshFilter mf in Resources.FindObjectsOfTypeAll(typeof(MeshFilter)) as MeshFilter[])
            {
                if (EditorUtility.IsPersistent(mf.transform.root.gameObject) && !(mf.hideFlags == HideFlags.NotEditable || mf.hideFlags == HideFlags.HideAndDontSave))
                    meshes.Add(mf);
            }

            foreach (MeshFilter filter in meshes)
            {
                for (int i = 0; i < meshesToReplace.Count; i++)
                {
                    searches++;
                    if (filter.sharedMesh == meshesToReplace[i])
                    {
                        EditorUtility.SetDirty(filter.gameObject);
                        changeCount++;
                        filter.sharedMesh = newMeshes[i];
                    }
                    if (changeCount > 0) Debug.Log($"{meshesToReplace[i].name}| Replaced {changeCount} meshes, Searched {searches} MeshFilters");
                }
            }
        }
    }
}
    

#endif