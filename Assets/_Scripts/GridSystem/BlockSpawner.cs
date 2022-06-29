using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BlockSpawner
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private BlockTypeDefinition[] blockTypes;
    [SerializeField] private RectTransform gridParentTransform;

    private uint[] BlockSpawnRequests { get; set; }

    private uint _collumnCount;

    private GridController _gridController;

    public BlockSpawner(GridController controller, uint collumnCount)
    {
        _collumnCount = collumnCount;
        BlockSpawnRequests = new uint[_collumnCount];

        StartFillBoardRequest();
        SummonRequestedBlocks();
    }

    private void StartFillBoardRequest()
    {
        for (int i = 0; i < BlockSpawnRequests.Length; i++)
        {
            BlockSpawnRequests[i] = (uint) _gridController.RowCount;
        }
    }

    private void ClearRequests()
    {
        for (int i = 0; i < BlockSpawnRequests.Length; i++) BlockSpawnRequests[i] = (uint)_gridController.RowCount;
    }

    public void AddBlockSpawnReqeust(int collumnIndex)
    {
        BlockSpawnRequests[collumnIndex] += 1;
    }

    public void SummonRequestedBlocks()
    {
        for (int i = 0; i < _collumnCount; i++)
        {
            for (int j = 0; j < BlockSpawnRequests[i]; j++)
            {
                GameObject newEntityGO = ObjectPooler.Instance.Spawn(blockPrefab.name, _gridController.GridPositions[_gridController.RowCount-1, i], Quaternion.identity, gridParentTransform);
                newEntityGO.gameObject.name = $"Block {i}_{j}";
                IGridEntity newEntity = newEntityGO.GetComponent<IGridEntity>();
                BlockTypeDefinition randomBlockType = blockTypes[UnityEngine.Random.Range(0, blockTypes.Length)];
                newEntity.SetupEntity(_gridController, randomBlockType);
                _gridController.RegisterGridEntityToPosition(newEntity, _gridController.RowCount-j, i);
            }
        }
        ClearRequests();
    }
}
