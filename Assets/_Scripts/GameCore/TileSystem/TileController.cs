using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class TileController
{
    private float _referencedDataRectLenght;

    private Rect _tileAreaRect;
    private Rect _tileDataRect;
    private WordController _wordController;

    public TileController(TileControllerReferences references, TileControllerSettings settings,
        TileControllerConfig config)
    {
        References = references;
        Settings = settings;
        Config = config;
    }

    public List<ITile> AllTiles { get; private set; } = new();
    private TileControllerReferences References { get; }
    private TileControllerSettings Settings { get; }
    private TileControllerConfig Config { get; }
    public float TileSize => TileDistanceMultiplier * Settings.tileSizeMultiplier;
    private float TileDistanceMultiplier { get; set; }

    public void SetupTileController(WordController wordController)
    {
        _wordController = wordController;

        CalculateTileArea();
        CalculateTileDistanceMultiplier();

        SpawnTiles(Config.TileDatas);
        LockChildrenTiles();
    }

    public void LoadTileController(WordController wordController, List<int> tilesLeftIds)
    {
        _wordController = wordController;

        CalculateTileArea();
        CalculateTileDistanceMultiplier();

        SpawnTiles(GetTileDatasFromId(tilesLeftIds));
        LockChildrenTiles();
    }

    private void SpawnTiles(TileData[] tilesToSpawn)
    {
        foreach (TileData tileData in tilesToSpawn)
        {
            // calculate and update new screen position for tile
            TileData copiedTileData = tileData.Clone();
            Vector2 tilePositionAsOffset = (Vector2)copiedTileData.Position - _tileDataRect.center;
            tilePositionAsOffset *= TileDistanceMultiplier;
            tilePositionAsOffset += Settings.tileAreaOffset;
            Vector3 tilePositionForCurrentScreen = (Vector3)_tileAreaRect.center +
                                                   new Vector3(tilePositionAsOffset.x, tilePositionAsOffset.y,
                                                       copiedTileData.Position.z);
            copiedTileData.SetPosition(tilePositionForCurrentScreen);

            // create tile GO and assiign generated lettertile
            GameObject tileGo = ObjectPooler.Instance.Spawn(References.tilePrefab.name, tilePositionForCurrentScreen);
            var monitor = tileGo.GetComponent<LetterMonitor>();
            var letter = new LetterTile(this, _wordController, monitor, copiedTileData);
            letter.SetPixelSize(TileSize);
            AllTiles.Add(letter);
        }
    }

    public ITile GetTileWithId(int id)
    {
        foreach (ITile tile in AllTiles)
        {
            if (tile.IsRemovedFromPlay) continue;
            if (tile.TileData.Id == id)
                return tile;
        }

        return null;
    }

    public void ClearTileController()
    {
        foreach (ITile tile in AllTiles)
        {
            if (tile.IsRemovedFromPlay) continue;
            tile.RemoveVisiuals();
        }

        AllTiles = null;
    }

    private TileData[] GetTileDatasFromId(List<int> tileIds)
    {
        var tileDatas = new TileData[tileIds.Count];
        var arrayIndexToAssign = 0;
        for (var i = 0; i < Config.TileDatas.Length; i++)
            if (tileIds.Contains(Config.TileDatas[i].Id))
            {
                tileDatas[arrayIndexToAssign] = Config.TileDatas[i];
                arrayIndexToAssign++;
            }

        return tileDatas;
    }

    private void CalculateTileDistanceMultiplier()
    {
        CalculateRectOfTileData();
        float longestDataRectSide = Mathf.Max(_tileDataRect.width, _tileDataRect.height);
        float heightDistanceMultiplier = _tileAreaRect.height / _tileDataRect.height;
        float widthDistanceMultiplier = _tileAreaRect.width / _tileDataRect.width;
        Debug.Log(_tileAreaRect.height + "/" + _tileDataRect.height);
        Debug.Log(_tileAreaRect.width + "/" + _tileDataRect.width);
        Debug.Log("H: " + heightDistanceMultiplier + " / W:" + widthDistanceMultiplier);
        TileDistanceMultiplier = Mathf.Min(heightDistanceMultiplier, widthDistanceMultiplier);
        float maxTileDistanceMultiplier =
            Screen.width / (_wordController.MaxWordLength + 1) /
            Settings.tileSizeMultiplier; // Maximum tiledistance multiplier allowed to make wordformer fit to screen
        if (TileDistanceMultiplier > maxTileDistanceMultiplier) TileDistanceMultiplier = maxTileDistanceMultiplier;
        Debug.Log("Distance Multiplier: " + TileDistanceMultiplier);
    }

    private void CalculateRectOfTileData()
    {
        _tileDataRect = new Rect();

        float highestX = Mathf.NegativeInfinity;
        float highestY = Mathf.NegativeInfinity;
        float smallestX = Mathf.Infinity;
        float smallestY = Mathf.Infinity;

        foreach (TileData tile in Config.TileDatas)
        {
            if (tile.Position.x < smallestX) smallestX = tile.Position.x;
            else if (tile.Position.x > highestX) highestX = tile.Position.x;
            if (tile.Position.y < smallestY) smallestY = tile.Position.y;
            else if (tile.Position.y > highestY) highestY = tile.Position.y;
        }

        _tileDataRect.size = new Vector2(highestX - smallestX, highestY - smallestY);
        _tileDataRect.center = new Vector2((highestX + smallestX) / 2, (highestY + smallestY) / 2);
    }

    private void CalculateTileArea()
    {
        _tileAreaRect = new Rect
        {
            size = ScreenHelper.GetScreenPercentage(Settings.percentageOfTileAreaOnScreen),
            center = References.tileAreaCenter.position
        };
    }

    private void LockChildrenTiles()
    {
        foreach (ITile tile in AllTiles) tile.LockChildren();
    }

    #region AUTO SOLVER METHODS

    public void SetupTileControllerAutoSolver(WordController wordController)
    {
        _wordController = wordController;
        SpawnTilesAutoSolver();
        LockChildrenTiles();
    }

    private void SpawnTilesAutoSolver()
    {
        foreach (TileData tileData in Config.TileDatas)
        {
            var letter = new LetterTile(this, _wordController, null, tileData);
            AllTiles.Add(letter);
        }
    }

    #endregion
}

[Serializable]
public class TileControllerSettings
{
    [BHeader("Tile Area Settings")] public Vector2 percentageOfTileAreaOnScreen;

    public float tileSizeMultiplier;
    public Vector2 tileAreaOffset;
}

[Serializable]
public class TileControllerReferences
{
    public GameObject tilePrefab;
    public RectTransform tileAreaCenter;
}

[Serializable]
public class TileControllerConfig
{
    [SerializeField] private TileData[] tileDatas;

    public TileData[] TileDatas => tileDatas;

    #region EDITOR

#if UNITY_EDITOR
    public void SaveTileDataEditor(TileData[] datas)
    {
        tileDatas = datas;
        FixTileZPositions();
    }

    private void FixTileZPositions()
    {
        Debug.Log("Fixing " + tileDatas.Length + " Z Positions...");
        foreach (TileData t in tileDatas)
        {
            int childLevel = ChildLevel(t.Id);
            t.SetPosition(t.Position - new Vector3(0, 0, 10 * childLevel));
        }
    }

    private int ChildLevel(int id)
    {
        TileData tile = GetTileWithId(id);
        var childLevels = new int[tile.Children.Length];
        for (var i = 0; i < tile.Children.Length; i++) childLevels[i] = ChildLevel(tile.Children[i]) + 1;

        var highestChildLevel = 0;
        foreach (int childLevel in childLevels)
            if (childLevel > highestChildLevel)
                highestChildLevel = childLevel;
        return highestChildLevel;
    }

    private TileData GetTileWithId(int id)
    {
        foreach (TileData tile in tileDatas)
            if (tile.Id == id)
                return tile;
        return null;
    }
#endif

    #endregion
}