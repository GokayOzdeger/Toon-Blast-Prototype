using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class LevelSelectController : MonoGameStateListener
{
    [SerializeField] private RectTransform scrollRectContentRect;
    [SerializeField] private GameObject listElementPrefab;

    private readonly List<LevelSelectElement> levelSelectElements = new();

    private void Start()
    {
        CreateElements();
    }

    private void CreateElements()
    {
        var previousLevelCompleted = true;
        foreach (LevelConfig config in LevelManager.Instance.LevelList)
        {
            var element = ObjectPooler.Instance.Spawn(listElementPrefab.name, Vector3.zero)
                .GetComponent<LevelSelectElement>();
            levelSelectElements.Add(element);
            element.transform.SetParent(scrollRectContentRect, false);
            LevelSaveData saveData = LevelSaveData.Data(config.LevelTitle);
            element.SetupElement(config, saveData, previousLevelCompleted);
            previousLevelCompleted = saveData.IsCompleted;
        }
    }

    public void RefreshElements()
    {
        var previousLevelCompleted = true;
        for (var i = 0; i < levelSelectElements.Count; i++)
        {
            LevelConfig config = LevelManager.Instance.LevelList[i];
            LevelSaveData saveData = LevelSaveData.Data(config.LevelTitle);
            levelSelectElements[i].UpdateElement(saveData, previousLevelCompleted);
            previousLevelCompleted = saveData.IsCompleted;
        }
    }

    protected override void OnEnterState()
    {
        RefreshElements();
    }

    protected override void OnExitState()
    {
        //
    }
}