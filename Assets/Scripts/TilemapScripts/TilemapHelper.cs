using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TilemapHelper
{
    public static List<Vector2Int> GetTilesByType(TilemapStructure tilemap, IEnumerable<int> enumerable)
    {
        var tileTypes = enumerable.ToList();
        var validTilePositions = new List<Vector2Int>();
        for (int x = 0; x < tilemap.Width; x++)
        {
            for (int y = 0; y < tilemap.Height; y++)
            {

                var tileType = tilemap.GetTile(x, y);
                if (tileTypes.Any(a => a == tileType))
                {
                    validTilePositions.Add(new Vector2Int(x, y));
                }
            }
        }
        return validTilePositions;
    }

    public static Vector2Int? FindClosestTileByType(TilemapStructure tilemap, Vector2Int startPos, int tileType)
    {
        float smallestDistance = float.MaxValue;
        Vector2Int? smallestDistancePosition = null;
        for (int x = 0;x < tilemap.Width; x++)
        {
            for (int y = 0;y < tilemap.Height;y++)
            {
                if (tilemap.GetTile(x, y) == tileType)
                {
                    float distance = ((startPos.x - x) * (startPos.x - x) + (startPos.y - y) * (startPos.y - y));
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        smallestDistancePosition = new Vector2Int(x, y);
                    }
                }
            }
        }
        return smallestDistancePosition;
    }
}