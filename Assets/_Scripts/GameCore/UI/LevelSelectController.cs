using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class LevelSelectController : MonoGameStateListener
{
    [SerializeField] private RectTransform scrollRectContentRect;
    [SerializeField] private GameObject listElementPrefab;

    private List<LevelSelectElement> levelSelectElements = new List<LevelSelectElement>();

    private void Start()
    {
        CreateElements();
    }

    private void CreateElements()
    {
        bool previousLevelCompleted = true;
        foreach (LevelConfig config in LevelManager.Instance.LevelList)
        {
            LevelSelectElement element = ObjectPooler.Instance.Spawn(listElementPrefab.name, Vector3.zero).GetComponent<LevelSelectElement>();
            levelSelectElements.Add(element);
            element.transform.SetParent(scrollRectContentRect, false);
            LevelSaveData saveData = LevelSaveData.Data(config.LevelTitle);
            element.SetupElement(config, saveData, previousLevelCompleted);
            previousLevelCompleted = saveData.IsCompleted;
        }
    }

    public void RefreshElements()
    {
        Debug.Log("Refresh elements");
        bool previousLevelCompleted = true;
        for (int i = 0; i < levelSelectElements.Count; i++)
        {
            LevelConfig config = LevelManager.Instance.LevelList[i];
            LevelSaveData saveData = LevelSaveData.Data(config.LevelTitle);
            Debug.Log($"{saveData.LevelTitle}-{saveData.HighScore}");
            levelSelectElements[i].UpdateElement(saveData, previousLevelCompleted);
            previousLevelCompleted = saveData.IsCompleted;
        } 
    }

    public override void OnEnterState()
    {
        RefreshElements();
    }

    public override void OnExitState()
    {
        //
    }
}
