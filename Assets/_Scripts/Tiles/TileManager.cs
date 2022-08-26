using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class TileManager 
{
    private List<ITile> AllTiles = new List<ITile>();
    private TileManagerReferences References { get; set; }
    private TileManagerSettings Settings { get; set; }
    private TileManagerConfig Config { get; set; }

    public TileManager(TileManagerReferences references, TileManagerSettings settings, TileManagerConfig config)
    {
        References = references;
        Settings = settings;
        Config = config;
    }

    public void SetupTileManager()
    {
        foreach(TileData tileData in Config.TileDatas)
        {
            GameObject tileGO = ObjectPooler.Instance.Spawn(Settings.tilePrefab.name, tileData.Position);
            LetterMonitor monitor = tileGO.GetComponent<LetterMonitor>();
            LetterTile letter = new LetterTile(monitor, tileData);
            AllTiles.Add(letter);
        }
        LockChildrenTiles();
    }

    public ITile GetTileWithId(int id)
    {
        foreach (ITile tile in AllTiles) if (tile.LetterData.Id == id) return tile;
        return null;
    }

    private void LockChildrenTiles()
    {
        foreach(ITile tile in AllTiles)
        {
            foreach(int childrenId in tile.LetterData.Children)
            {
                GetTileWithId(childrenId).LockTile();
            }
        }
    }
}

[System.Serializable]
public class TileManagerSettings
{
    public GameObject tilePrefab;
    public float extraDistanceBetweenTiles;
}

[System.Serializable]
public class TileManagerReferences
{

}

[System.Serializable]
public class TileManagerConfig
{
    [SerializeField] private TileData[] tileDatas;

    public TileData[] TileDatas => tileDatas;

#if UNITY_EDITOR
    public void SaveTileDataEditor(TileData[] datas)
    {
        tileDatas = datas;
        FixTileZPositions();
    }

    private void FixTileZPositions()
    {
        for (int i = 0; i < tileDatas.Length; i++)
        {
            tileDatas[i].SetPositionEditor(tileDatas[i].Position - new Vector3(0, 0, 5 * i));
        }
    }
#endif
}
