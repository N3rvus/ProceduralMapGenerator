using Assets.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapStructure : MonoBehaviour
{
    [HideInInspector]
    public TileGrid Grid;

    [HideInInspector]
    public int Width, Height;
    private int[] _tiles;
    private Tilemap _graphicMap;

    [SerializeField]
    private AlgorithmBase[] _algorithms;

    [SerializeField]
    private TilemapType _type;
    public TilemapType Type { get { return _type; } }


    public void Initialize()
    {
        _graphicMap = GetComponent<Tilemap>();
        Grid = transform.parent.GetComponent<TileGrid>();
        Width = Grid.Width;
        Height = Grid.Height;
        _tiles = new int[Width * Height];

        foreach (var algorithm in _algorithms)
        {
            Generate(algorithm);
        }

        RenderAllTiles();

         void RenderAllTiles()
        {
            var positionsArray = new Vector3Int[Width * Height];
            var tilesArray = new Tile[Width * Height];

            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    positionsArray[x * Width + y] = new Vector3Int(x, y, 0);

                    var typeOfTile = GetTile(x, y);
                    
                    if (!Grid.Tiles.TryGetValue(typeOfTile, out Tile tile))
                    {
                        if (typeOfTile != 0)
                        {
                            Debug.LogError("Tile not defined for ID: " +  typeOfTile);
                        }
                        tilesArray[x * Width + y] = null;
                        continue;
                    }
                    tilesArray[y * Width + x] = tile;
                }
            }
            _graphicMap.SetTiles(positionsArray, tilesArray);
            _graphicMap.RefreshAllTiles();
        }

    }

    public List<KeyValuePair<Vector2Int, int>> GetNeighbors(int tileX, int tileY)
    {
        int startX = tileX - 1;
        int startY = tileY - 1;
        int endX = tileX + 1;
        int endY = tileY + 1;

        var neighbors = new List<KeyValuePair<Vector2Int, int>>();
        for (int x = startX; x < endX + 1; x++)
        {
            for (int y = startY; y < endY + 1; y++)
            {
                if (x == tileX && y == tileY) continue;
                if (InBounds(x, y))
                {
                    neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(x, y), GetTile(x, y)));
                }
            }
        }
        return neighbors;
    }

    public List<KeyValuePair<Vector2Int, int>> Get4Neighbors(int tileX, int tileY)
    {
        int startX = tileX - 1;
        int startY = tileY - 1;
        int endX = tileX + 1;
        int endY = tileY + 1;

        var neighbors = new List<KeyValuePair<Vector2Int, int>>();
        for (int x = startX; x < endX + 1; x++)
        {
            if (x == tileX || !InBounds(x, tileY)) continue;
            neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(x, tileY), GetTile(x, tileY)));
        }
        for (int y = startY; y < endY + 1; y++)
        {
            if (y == tileY || !InBounds(tileX, y)) continue;
            neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(tileX, y), GetTile(tileX, y)));
        }

        return neighbors;
    }



    public int GetTile(int x, int y)
    {
        return InBounds(x, y) ? _tiles[y * Width + x] : 0;
    }

    public void SetTile(int x, int y, int value)
    {
        if (InBounds(x, y))
        {
            _tiles[y * Width + x] = value;
        }
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public void Generate(AlgorithmBase algorithm)
    {
        algorithm.Apply(this);
    }
}