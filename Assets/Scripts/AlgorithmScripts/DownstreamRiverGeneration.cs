using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DownstreamRiverGeneration", menuName = "Algorithms/DownstreamRiverGeneration")]
    public class DownstreamRiverGeneration : AlgorithmBase
    {
        public int MinRiverQuota;
        public int MaxRiverQuota;
        public int MinDistanceBetweenRiverStartPoints;
        public GroundTileType[] StartingTileTypes;
        public NoiseGeneration GroundHeightmap;

        private System.Random _random;

        public override void Apply(TilemapStructure tilemap)
        {
            _random = new System.Random(tilemap.Grid.Seed);

            var heightmap = Noise.GenerateNoiseMap(tilemap.Width, tilemap.Height, tilemap.Grid.Seed, GroundHeightmap.NoiseScale, GroundHeightmap.Octaves, GroundHeightmap.Persistance, GroundHeightmap.Lacunarity, GroundHeightmap.Offset);
            if (GroundHeightmap.ApplyIslandGradient)
            {
                var islandGradient = Noise.GenerateIslandGradientMap(tilemap.Width, tilemap.Height);
                for (int x = 0, y; x < tilemap.Width; x++)
                {
                    for (y = 0; y < tilemap.Height; y++)
                    {
                        float subtractedValue = heightmap[y * tilemap.Width + x] - islandGradient[y * tilemap.Width + x];

                        heightmap[y * tilemap.Width + x] = Mathf.Clamp01(subtractedValue);
                    }
                }
            }

            var validStartPositions = TilemapHelper.GetTilesByType(tilemap, StartingTileTypes.Select(a => (int)a));
            var amountOfRivers = _random.Next(MinRiverQuota, MaxRiverQuota + 1);

            var rivers = new List<DownstreamRiver>();
            for (int i = 0; i < amountOfRivers; i++)
            {
                var startPoint = GetValidStartPosition(validStartPositions, rivers);
                if (!startPoint.HasValue) break;

                var river = new DownstreamRiver(startPoint.Value);
                if (river.Build(tilemap, heightmap))
                {
                    rivers.Add(river);
                }
            }

            int riverTileId = (int)GroundTileType.River;
            foreach (var riverPosition in rivers.SelectMany(a => a.RiverPositions))
            {
                tilemap.SetTile(riverPosition.x, riverPosition.y, riverTileId);
            }
        }

        private Vector2Int? GetValidStartPosition(List<Vector2Int> startPositions, List<DownstreamRiver> rivers)
        {
            if (!startPositions.Any()) return null;

            var startPoint = startPositions[_random.Next(0, startPositions.Count)];
            startPositions.Remove(startPoint);

            const int maxAttempts = 500;
            int attempt = 0;

            while (rivers.Any(river => !river.CheckDistance(startPoint, MinDistanceBetweenRiverStartPoints)))
            {
                if (attempt >= maxAttempts)
                {
                    return null;
                }
                attempt++;

                if (!startPositions.Any()) return null;
                startPoint = startPositions[_random.Next(0, startPositions.Count)];
                startPositions.Remove(startPoint);
            }

            return startPoint;
        }

        class DownstreamRiver
        {
            public Vector2Int StartPos;
            public HashSet<Vector2Int> RiverPositions;

            private const int _maxAttempts = 1000;

            public DownstreamRiver(Vector2Int startPos)
            {
                StartPos = startPos;
                RiverPositions = new HashSet<Vector2Int> { StartPos };
            }

            public bool CheckDistance(Vector2Int startPos, int minDistance)
            {
                float distance = ((startPos.x - StartPos.x) * (startPos.x - StartPos.x) + (startPos.y - StartPos.y) * (startPos.y - StartPos.y));
                return distance > minDistance * minDistance;
            }

            public bool Build(TilemapStructure tilemap, float[] heightmap)
            {
                Vector2Int currentPos = RiverPositions.First();

                int waterTileId = (int)GroundTileType.DeepWater;

                bool done = false;
                int attempt = 0;
                while (!done)
                {
                    if (attempt >= _maxAttempts)
                    {
                        break;
                    }
                    attempt++;

                    var height = heightmap[currentPos.y * tilemap.Width + currentPos.x];


                    var lowestHeightNeighbor = tilemap.Get4Neighbors(currentPos.x, currentPos.y)
                        .Select(a => new KeyValuePair<Vector2Int, float>(a.Key, heightmap[a.Key.y * tilemap.Width + a.Key.x]))
                        .OrderBy(a => a.Value)
                        .Select(a => new KeyValuePair<Vector2Int, float>?(a))
                        .FirstOrDefault(a => !RiverPositions.Contains(a.Value.Key));

                    if (lowestHeightNeighbor == null)
                    {
                        done = true;
                        break;
                    }

                    currentPos = lowestHeightNeighbor.Value.Key;
                    RiverPositions.Add(lowestHeightNeighbor.Value.Key);

                    done = tilemap.GetTile(lowestHeightNeighbor.Value.Key.x, lowestHeightNeighbor.Value.Key.y) == waterTileId;
                }

                return done;
            }
        }
    }
}
