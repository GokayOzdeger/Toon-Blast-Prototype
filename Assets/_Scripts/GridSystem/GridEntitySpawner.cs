using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GridEntitySpawner
{
    private int[] BlockSpawnRequests { get; set; }

    private Vector2[] _spawnPositionRow;
    private IGridEntityTypeDefinition[] _gridEntityTypes;
    private RectTransform _gridParentTransform;
    private int _collumnCount;

    private GridController _gridController;

    public GridEntitySpawner(GridController gridController, GridEntitySpawnerSettings settings, GridEntitySpawnerSceneReferences references )
    {
        this._gridEntityTypes = settings.EntityTypes;
        this._gridParentTransform = references.GridParentTransform;
        this._gridController = gridController;
        this._collumnCount = gridController.CollumnCount;
        BlockSpawnRequests = new int[_collumnCount];
        CalculateSpawnPositionRow(settings.SpawnHeight);
        StartFillBoardRequest();
        SummonRequestedEntities();
    }

    private void CalculateSpawnPositionRow(int spawnHeight)
    {
        _spawnPositionRow = new Vector2[_collumnCount];
        for (int i = 0; i < _collumnCount; i++)
        {
            _spawnPositionRow[i] = _gridController.GridPositions[_gridController.RowCount - 1, i] + new Vector2(0, spawnHeight);
        }
    }

    private void StartFillBoardRequest()
    {
        for (int i = 0; i < BlockSpawnRequests.Length; i++)
        {
            BlockSpawnRequests[i] = _gridController.RowCount;
        }
    }

    private void ClearRequests()
    {
        for (int i = 0; i < BlockSpawnRequests.Length; i++) BlockSpawnRequests[i] = 0;
    }

    // registers a spawn request to collumn index
    public void AddEntitySpawnReqeust(int collumnIndex)
    {
        BlockSpawnRequests[collumnIndex] ++;
    }

    // spawns grid entities at the requested collumns
    public void SummonRequestedEntities()
    {
        for (int i = 0; i < _collumnCount; i++)
        {
            for (int j = BlockSpawnRequests[i]-1; j >=0 ; j--)
            {
                Vector2Int gridCoordinates = new Vector2Int(_gridController.RowCount - j - 1, i);
                IGridEntityTypeDefinition randomBlockType = _gridEntityTypes[Random.Range(0, _gridEntityTypes.Length)];
                GameObject newEntityGO = ObjectPooler.Instance.Spawn(randomBlockType.GridEntityPrefab.name, 
                    _spawnPositionRow[i]-j*new Vector2(0,_gridController.RowSpacing),
                    Quaternion.identity);
                newEntityGO.transform.SetParent(_gridParentTransform);
                newEntityGO.gameObject.name = $"Block {i}_{j}";
                IGridEntity newEntity = newEntityGO.GetComponent<IGridEntity>();
                newEntity.SetupEntity(_gridController, randomBlockType);
                _gridController.RegisterGridEntityToPosition(newEntity, gridCoordinates.x, gridCoordinates.y);
                newEntity.OnMoveEntity(gridCoordinates);
            }
        }
        _gridController.CallCachedChanges();
        ClearRequests();
    }

    [System.Serializable]
    public class GridEntitySpawnerSceneReferences
    {
        [BHeader("Grid Entity Spawner References")]
        [SerializeField] RectTransform gridParentTransform;
        public RectTransform GridParentTransform => gridParentTransform;
    }

    [System.Serializable]
    public class GridEntitySpawnerSettings
    {
        [BHeader("Grid Entity Spawner Settings")]
        [SerializeField] private BasicGridEntityTypeDefinition[] entityTypes;
        [SerializeField] private int spawnHeight;
        public BasicGridEntityTypeDefinition[] EntityTypes => entityTypes;
        public int SpawnHeight => spawnHeight;
    }
}