using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesController
{
    public static readonly int MinGroupSizeForExplosion = 2;

    public int MovesLeft { get; private set; }

    private GridController _gridController;
    private GridEntitySpawnController _gridEntitySpawner;
    private TMPro.TMP_Text _movesLeftText;

    public MovesController(GridController gridController, GridEntitySpawnController entitySpawner, MovesControllerSettings settings, MovesControllerSceneReferences references)
    {
        this._gridController = gridController;
        this._gridEntitySpawner = entitySpawner;
        this._movesLeftText = references.MovesLeftText;
        _gridController.OnGridInterractable.AddListener(OnGridReadyForNextMove);
        MovesLeft = settings.MoveCount;
        UpdateMovesLeftUiText();
    }

    public bool TryMakeMatchMove(Block blockEntity)
    {
        if (MovesLeft == 0) return false;
        if (blockEntity.CurrentMatchGroup.Count < MinGroupSizeForExplosion) return false;
        Vector2Int matchClickCoordinates = blockEntity.GridCoordinates;
        
        BlockMatchCondition? condition = blockEntity.ActiveBlockCondition();
        MatchGridEvent matchEvent = new MatchGridEvent(blockEntity.EntityTransform.position, blockEntity.GridCoordinates, condition);
        matchEvent.StartEvent(_gridController, blockEntity.CurrentMatchGroup);

        MovesLeft--;
        UpdateMovesLeftUiText();
        return true;
    }

    public void ClickedPowerUp()
    {
        MovesLeft--;
        UpdateMovesLeftUiText();
    }

    private void OnGridReadyForNextMove()
    {
        if (MovesLeft != 0) return;
        GameManager.Instance.CurrentLevel.LevelFailed();
    }
    private void UpdateMovesLeftUiText()
    {
        _movesLeftText.text = MovesLeft.ToString();
    }
}
