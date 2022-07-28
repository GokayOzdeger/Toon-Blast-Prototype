using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShuffleController
{
    private GridController _gridController;

    public ShuffleController(GridController grid, ShuffleControllerSceneReferences references)
    {
        this._gridController = grid;
        if (references.ForceShuffleButton) 
            references.ForceShuffleButton.onClick.AddListener(OnClickForceShuffleButton);
    }

    public void CheckShuffleRequired()
    {
        foreach(IGridEntity entity in _gridController.EntityGrid)
        {
            if(entity is Block)
            {
                Block block = (Block)entity;
                if (block.CurrentMatchGroup.Count >= MovesController.MinGroupSizeForExplosion) return;
            }
        }
        DoShuffle();
    }

    private void DoShuffle()
    {
        CallShufflingFlyingText();
        
        // collect entities to shuffle
        List<IGridEntity> entitiesToShuffle = new List<IGridEntity>();
        
        for (int i = 0; i < _gridController.RowCount; i++)
        {
            for (int j = 0; j < _gridController.ColumnCount; j++)
            {
                IGridEntity entity = _gridController.EntityGrid[i, j];
                if (entity == null) continue;
                if (entity.EntityType.EntityIncludedInShuffle) entitiesToShuffle.Add(entity);
            }
        }

        // shuffle entities in pairs
        int shufflePairsCount = (entitiesToShuffle.Count / 2)-1;
        for (int i = 0; i < shufflePairsCount; i++)
        {
            Vector2Int entityACoordinates = PopRandomFromList(ref entitiesToShuffle).GridCoordinates;
            Vector2Int entityBCoordinates = PopRandomFromList(ref entitiesToShuffle).GridCoordinates;
            _gridController.SwapEntities(entityACoordinates, entityBCoordinates);
        }
        _gridController.UpdateAllEntities();
    }

    private void CallShufflingFlyingText()
    {
        Vector2 flyingTextStartPos = UIEffectsManager.Instance.GetReferencePointByName("LeftCenterOutside");
        Vector2 flyingTextWaitingPos = UIEffectsManager.Instance.GetReferencePointByName("ScreenCenter");
        Vector2 flyingTextEndPos = UIEffectsManager.Instance.GetReferencePointByName("RightCenterOutside");
        UIEffectsManager.Instance.CreatePassingByFlyingText("Shuffling", 130, flyingTextStartPos, flyingTextWaitingPos, flyingTextEndPos, UIEffectsManager.CanvasLayer.OverGridUnderUI, 1f, 0);
    }
    
    private T PopRandomFromList<T>(ref List<T> list)
    {
        if (list.Count == 0) return default(T);
        int randomIndex = Random.Range(0, list.Count);
        T randomElement = list[randomIndex];
        list.RemoveAt(randomIndex);
        return randomElement;
    }

    private void OnClickForceShuffleButton()
    {
        DoShuffle();
    }
}
