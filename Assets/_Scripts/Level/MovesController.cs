using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesController
{
    public static readonly int MinGroupSizeForExplosion = 2;

    public int MovesLeft { get; private set; }

    private GridController _gridController;
    private TMPro.TMP_Text _movesLeftText;

    public MovesController(GridController gridController, MovesControllerSettings settings, MovesControllerSceneReferences references)
    {
        this._gridController = gridController;
        this._movesLeftText = references.MovesLeftText;
        MovesLeft = settings.MoveCount;
        UpdateMovesLeftUiText();
    }

    public bool TryMakeMatchMove(Block blockEntity)
    {
        if (MovesLeft == 0) return false;
        if (blockEntity.CurrentMatchGroup.Count < MinGroupSizeForExplosion) return false;
        DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent();
        destroyEvent.StartEvent(_gridController, blockEntity.CurrentMatchGroup);
        MovesLeft--;
        UpdateMovesLeftUiText();
        return true;
    }

    private void UpdateMovesLeftUiText()
    {
        _movesLeftText.text = MovesLeft.ToString();
    }
}
