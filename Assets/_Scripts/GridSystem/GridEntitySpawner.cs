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
    private ArrayLayout _startLayout;

    public GridEntitySpawner(GridController gridController, GridEntitySpawnerSettings settings, GridEntitySpawnerSceneReferences references )
    {
        this._gridEntityTypes = settings.EntityTypes;
        this._gridParentTransform = references.GridParentTransform;
        this._gridController = gridController;
        this._collumnCount = gridController.CollumnCount;
        this._startLayout = settings.gridStartLayout;
        BlockSpawnRequests = new int[_collumnCount];
        CalculateSpawnPositionRow(settings.SpawnHeight);
    }

    public void FillAllGridWithStartLayout()
    {
        StartFillBoardRequest();
        SummonRequestedEntities(_startLayout);
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
    public void SummonRequestedEntities(ArrayLayout layout = null)
    {
        int[] summonRequestsCopy = (int[]) BlockSpawnRequests.Clone();
        ClearRequests();

        for (int i = 0; i < _collumnCount; i++)
        {
            for (int j = summonRequestsCopy[i]-1; j >=0 ; j--)
            {
                Vector2Int gridCoordinates = new Vector2Int(_gridController.RowCount - j - 1, i);

                // choose random entity type if startLayout is missing a configuration for it
                IGridEntityTypeDefinition randomEntityType = null;
                if (layout == null || layout.rows[j].row[i] == null) randomEntityType = _gridEntityTypes[Random.Range(0, _gridEntityTypes.Length)];
                else randomEntityType = layout.rows[j].row[i];
                
                GameObject newEntityGO = ObjectPooler.Instance.Spawn(randomEntityType.GridEntityPrefab.name, 
                    _spawnPositionRow[i]-j*new Vector2(0,_gridController.GridCellSpacing),
                    Quaternion.identity);
                newEntityGO.transform.SetParent(_gridParentTransform);
                newEntityGO.gameObject.name = $"{randomEntityType.GridEntityTypeName} {i}_{j}";

                newEntityGO.GetComponent<RectTransform>().sizeDelta = new Vector2(_gridController.GridCellSpacing, _gridController.GridCellSpacing);
                IGridEntity newEntity = newEntityGO.GetComponent<IGridEntity>();
                newEntity.SetupEntity(_gridController, randomEntityType);
                _gridController.RegisterGridEntityToPosition(newEntity, gridCoordinates.x, gridCoordinates.y);
                newEntity.OnMoveEntity(gridCoordinates, IGridEntity.MovementMode.Linear);
            }
        }
        _gridController.CallCachedChanges();
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
        [BHeader("Grid Start Layout")]
        public ArrayLayout gridStartLayout = new ArrayLayout(9, 9);

        [BHeader("Grid Entity Spawner Settings")]
        [SerializeField] private BasicGridEntityTypeDefinition[] entityTypes;
        [SerializeField] private int spawnHeight;

        public BasicGridEntityTypeDefinition[] EntityTypes => entityTypes;
        public int SpawnHeight => spawnHeight;
    }
}
