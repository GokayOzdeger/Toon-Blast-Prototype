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

    public bool HasLegalMove()
    {
        foreach(IGridEntity entity in _gridController.EntityGrid)
        {
            if(entity is Block)
            {
                Block block = (Block)entity;
                if (block.CurrentBlockGroup.Count >= ExplosionGridEvent.MinGroupSizeForExplosion)
                {
                    return true;
                }
            }
        }
        DoShuffle();
        return false;
    }

    private void DoShuffle()
    {
        // collect entities to shuffle
        List<IGridEntity> entitiesToShuffle = new List<IGridEntity>();
        
        for (int i = 0; i < _gridController.RowCount; i++)
        {
            for (int j = 0; j < _gridController.CollumnCount; j++)
            {
                IGridEntity entity = _gridController.EntityGrid[i, j];
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

    [System.Serializable]
    public class ShuffleControllerSceneReferences
    {
        [BHeader("Shuffle Controller References")]
        [SerializeField] private Button forceShuffleButton;
        public Button ForceShuffleButton => forceShuffleButton;
    }
}
