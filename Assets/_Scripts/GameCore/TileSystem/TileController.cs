using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class TileController 
{
    public List<ITile> AllTiles { get; private set; } = new List<ITile>();
    private TileControllerReferences References { get; set; }
    private TileControllerSettings Settings { get; set; }
    private TileControllerConfig Config { get; set; }
    public float TileSize => TileDistanceMultiplier * Settings.tileSizeMultiplier * TileDataRect.width;
    public float TileDistanceMultiplier { get; private set; }

    private Rect TileAreaRect;
    private Rect TileDataRect;
    private WordController _wordController;

    public TileController(TileControllerReferences references, TileControllerSettings settings, TileControllerConfig config)
    {
        References = references;
        Settings = settings;
        Config = config;
    }

    public void SetupTileManager( WordController wordController)
    {
        _wordController = wordController;
        CalculateTileArea();
        CalculateTileDistanceMultiplier();
        SpawnTiles();
        LockChildrenTiles();
    }

    public void SetupTileManagerAutoSolver()
    {
        SpawnTilesAutoSolver();
        LockChildrenTiles();
    }

    private void SpawnTiles()
    {
        foreach (TileData tileData in Config.TileDatas)
        {
            // calculate and update new screen position for tile
            Vector2 tilePositionAsOffset = (Vector2)tileData.Position - TileDataRect.center;
            tilePositionAsOffset *= TileDistanceMultiplier;
            tilePositionAsOffset += Settings.tileAreaOffset;
            Vector3 tilePositionForCurrentScreen = (Vector3)TileAreaRect.center + new Vector3(tilePositionAsOffset.x, tilePositionAsOffset.y, tileData.Position.z);
            tileData.SetPosition(tilePositionForCurrentScreen);

            // create tile GO and assiign generated lettertile
            GameObject tileGO = ObjectPooler.Instance.Spawn(References.tilePrefab.name, tilePositionForCurrentScreen);
            LetterMonitor monitor = tileGO.GetComponent<LetterMonitor>();
            LetterTile letter = new LetterTile(this, _wordController, monitor, tileData);
            letter.SetPixelSize(TileSize);
            AllTiles.Add(letter);
        }
    }

    private void SpawnTilesAutoSolver()
    {
        foreach (TileData tileData in Config.TileDatas)
        {
            LetterTile letter = new LetterTile(this, _wordController,null, tileData);
            AllTiles.Add(letter);
        }
    }

    public ITile GetTileWithId(int id)
    {
        foreach (ITile tile in AllTiles) if (tile.TileData.Id == id) return tile;
        return null;
    }

    private void CalculateTileDistanceMultiplier()
    {
        CalculateRectOfTileData();
        float heightDistanceMultiplier = TileAreaRect.height / TileDataRect.height;
        float widthDistanceMultiplier = TileAreaRect.width / TileDataRect.width;
        TileDistanceMultiplier = Mathf.Min(heightDistanceMultiplier, widthDistanceMultiplier);
    }

    private void CalculateRectOfTileData()
    {
        TileDataRect = new Rect();
        
        float highestX = Mathf.NegativeInfinity;
        float highestY = Mathf.NegativeInfinity;
        float smallestX = Mathf.Infinity;
        float smallestY = Mathf.Infinity;
        
        foreach(TileData tile in Config.TileDatas)
        {
            if (tile.Position.x < smallestX) smallestX = tile.Position.x;
            else if (tile.Position.x > highestX) highestX = tile.Position.x;
            if (tile.Position.y < smallestY) smallestY = tile.Position.y;
            else if (tile.Position.y > highestY) highestY = tile.Position.y;
        }
        TileDataRect.size = new Vector2(highestX - smallestX, highestY - smallestY);
        TileDataRect.center = new Vector2((highestX+smallestX)/2, (highestY + smallestY) / 2);
    }

    private void CalculateTileArea()
    {
        TileAreaRect = new Rect();
        TileAreaRect.size = ScreenHelper.GetScreenPercentage(Settings.percentageOfTileAreaOnScreen);
        TileAreaRect.center = References.tileAreaCenter.position;
    }

    private void LockChildrenTiles()
    {
        foreach(ITile tile in AllTiles) tile.LockChildren();
    }
}

[System.Serializable]
public class TileControllerSettings
{
    [BHeader("Tile Area Settings")]
    public Vector2 percentageOfTileAreaOnScreen;
    public float tileSizeMultiplier;
    public Vector2 tileAreaOffset;
}

[System.Serializable]
public class TileControllerReferences
{
    public GameObject tilePrefab;
    public RectTransform tileAreaCenter;
}

[System.Serializable]
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
        Debug.Log($"Fixing {tileDatas.Length} Z Positions...");
        for (int i = 0; i < tileDatas.Length; i++)
        {
            int childLevel = ChildLevel(tileDatas[i].Id);
            tileDatas[i].SetPosition(tileDatas[i].Position - new Vector3(0, 0, 10 * childLevel));
        }
    }

    private int ChildLevel(int id)
    {
        TileData tile = GetTileWithId(id);
        int[] childLevels = new int[tile.Children.Length];
        for (int i = 0; i < tile.Children.Length; i++)
        {
            childLevels[i] = ChildLevel(tile.Children[i]) + 1;
        }

        int highestChildLevel = 0;
        foreach(int childLevel in childLevels)
        {
            if(childLevel > highestChildLevel) highestChildLevel = childLevel;
        }
        return highestChildLevel;
    }

    private TileData GetTileWithId(int id)
    {
        foreach (TileData tile in tileDatas) if (tile.Id == id) return tile;
        return null;
    }
#endif
    #endregion
}
