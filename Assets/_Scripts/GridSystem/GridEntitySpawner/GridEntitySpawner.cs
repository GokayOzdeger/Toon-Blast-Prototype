using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GridEntitySpawner
{
    private int[] BlockSpawnRequests { get; set; }

    private Vector2[] _spawnPositionRow;
    private IGridEntityTypeDefinition[] _gridEntityTypesToSpawnFrom;
    private RectTransform _gridParentTransform;
    private int _collumnCount;

    private GridController _gridController;
    private GridStartLayout _startLayout;

    public GridEntitySpawner(GridController gridController, GridEntitySpawnerSettings settings, GridEntitySpawnerSceneReferences references )
    {
        this._gridEntityTypesToSpawnFrom = settings.EntityTypes;
        this._gridParentTransform = references.GridParentTransform;
        this._gridController = gridController;
        this._collumnCount = gridController.ColumnCount;
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

    // unregisters a spawn request to collumn index
    public void RemoveEntitySpawnReqeust(int collumnIndex)
    {
        BlockSpawnRequests[collumnIndex]--;
    }

    // spawns grid entities at the requested collumns
    public void SummonRequestedEntities(GridStartLayout layout = null)
    {
        int[] summonRequestsCopy = (int[]) BlockSpawnRequests.Clone();
        int blocksSummoned = 0;
        ClearRequests();

        for (int i = 0; i < _collumnCount; i++)
        {
            for (int j = summonRequestsCopy[i]-1; j >=0 ; j--)
            {
                blocksSummoned++;
                Vector2Int gridCoordinates = new Vector2Int(_gridController.RowCount - j - 1, i);

                // choose random entity type if startLayout is missing a configuration for it
                IGridEntityTypeDefinition randomEntityType = null;
                if (layout == null || layout.rows[j].row[i] == null) randomEntityType = _gridEntityTypesToSpawnFrom[Random.Range(0, _gridEntityTypesToSpawnFrom.Length)];
                else randomEntityType = layout.rows[j].row[i];

                Vector2 spawnPos = _spawnPositionRow[i] - j * new Vector2(0, _gridController.GridCellSpacing);
                GameObject newEntityGO = ObjectPooler.Instance.Spawn(randomEntityType.GridEntityPrefab.name, spawnPos);
                newEntityGO.transform.SetParent(_gridParentTransform);
                newEntityGO.gameObject.name = $"{randomEntityType.GridEntityTypeName} {i}_{j}";

                newEntityGO.GetComponent<RectTransform>().sizeDelta = new Vector2(_gridController.GridCellSpacing, _gridController.GridCellSpacing);
                IGridEntity newEntity = newEntityGO.GetComponent<IGridEntity>();
                newEntity.SetupEntity(_gridController, randomEntityType);
                _gridController.RegisterGridEntityToPosition(newEntity, gridCoordinates.x, gridCoordinates.y);
                newEntity.OnMoveEntity(gridCoordinates, MovementMode.Linear);
            }
        }
        if(blocksSummoned > 0) _gridController.CallCachedChanges();
    }

    public void SpawnEntity(IGridEntityTypeDefinition entityType, Vector2Int gridCoordinates)
    {
        Vector2 spawnPos = _gridController.GridPositions[gridCoordinates.x, gridCoordinates.y];
        GameObject newEntityGO = ObjectPooler.Instance.Spawn(entityType.GridEntityPrefab.name, spawnPos);
        newEntityGO.transform.SetParent(_gridParentTransform);
        newEntityGO.gameObject.name = $"{entityType.GridEntityTypeName} {gridCoordinates.x}_{gridCoordinates.y}";

        newEntityGO.GetComponent<RectTransform>().sizeDelta = new Vector2(_gridController.GridCellSpacing, _gridController.GridCellSpacing);
        IGridEntity newEntity = newEntityGO.GetComponent<IGridEntity>();
        newEntity.SetupEntity(_gridController, entityType);
        _gridController.RegisterGridEntityToPosition(newEntity, gridCoordinates.x, gridCoordinates.y);
        newEntity.OnMoveEntity(gridCoordinates, MovementMode.Linear);
    }
}
