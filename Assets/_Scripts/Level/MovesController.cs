using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesController
{
    public static readonly int MinGroupSizeForExplosion = 2;

    public int MovesLeft { get; private set; }

    private GridController _gridController;
    private GridEntitySpawner _gridEntitySpawner;
    private TMPro.TMP_Text _movesLeftText;

    public MovesController(GridController gridController, GridEntitySpawner entitySpawner, MovesControllerSettings settings, MovesControllerSceneReferences references)
    {
        this._gridController = gridController;
        this._gridEntitySpawner = entitySpawner;
        this._movesLeftText = references.MovesLeftText;
        MovesLeft = settings.MoveCount;
        UpdateMovesLeftUiText();
    }

    public bool TryMakeMatchMove(Block blockEntity)
    {
        if (MovesLeft == 0) return false;
        if (blockEntity.CurrentMatchGroup.Count < MinGroupSizeForExplosion) return false;
        Vector2Int matchClickCoordinates = blockEntity.GridCoordinates;
        BlockMatchCondition? condition = blockEntity.ActiveBlockCondition();
        DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent(EntityDestroyTypes.DestroyedByMatch);
        destroyEvent.StartEvent(_gridController, blockEntity.CurrentMatchGroup);
        if (condition != null)
        {
            _gridEntitySpawner.SpawnEntity(condition.Value.GetRandomEntityToSpawn(), matchClickCoordinates);
            _gridEntitySpawner.RemoveEntitySpawnReqeust(matchClickCoordinates.y);
        }
        MovesLeft--;
        UpdateMovesLeftUiText();
        return true;
    }

    private void UpdateMovesLeftUiText()
    {
        _movesLeftText.text = MovesLeft.ToString();
    }
}
