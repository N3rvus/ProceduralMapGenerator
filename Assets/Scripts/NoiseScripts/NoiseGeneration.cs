using System;
using System.Linq;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NoiseGeneration", menuName = "Algorithms/NoiseGeneration")]
    public class NoiseGeneration : AlgorithmBase
    {
        [Header("Noise settings")]
        public int Octaves;
        [Range(0, 1)]
        public float Persistance;
        public float Lacunarity;
        public float NoiseScale;
        public Vector2 Offset;
        public bool ApplyIslandGradient;

        [Serializable]
        class NoiseValues
        {
            [Range(0f, 1f)]
            public float Height;
            public GroundTileType GroundTile;
        }

        [SerializeField]
        private NoiseValues[] TileTypes;

        public override void Apply(TilemapStructure tilemap)
        {
            TileTypes = TileTypes.OrderBy(a => a.Height).ToArray();

            var noiseMap = Noise.GenerateNoiseMap(tilemap.Width, tilemap.Height, tilemap.Grid.Seed, NoiseScale, Octaves, Persistance, Lacunarity, Offset);

            if (ApplyIslandGradient)
            {
                var islandGradient = Noise.GenerateIslandGradientMap(tilemap.Width, tilemap.Height);
                for (int x = 0, y; x < tilemap.Width; x++)
                {
                    for (y = 0; y < tilemap.Height; y++)
                    {
                        float subtractedValue = noiseMap[y * tilemap.Width + x] - islandGradient[y * tilemap.Width + x];

                        noiseMap[y * tilemap.Width + x] = Mathf.Clamp01(subtractedValue);
                    }
                }
            }

            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    var height = noiseMap[y * tilemap.Width + x];

                    for (int i = 0; i < TileTypes.Length; i++)
                    {
                        if (height <= TileTypes[i].Height)
                        {
                            tilemap.SetTile(x, y, (int)TileTypes[i].GroundTile);
                            break;
                        }
                    }
                }
            }
        }
    }
}
