using Assets.Tilemaps;
using System.Linq;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CellularAutomata", menuName = "Algorithms/CellularAutomata")]
    public class CellularAutomata : AlgorithmBase
    {
        public int MinAlive, Repetitions;

        [Tooltip("If this is checked, ReplacedBy will have no effect.")]
        public bool ReplaceByDominantTile;

        public ObjectTileType TargetTile, ReplacedBy;

        public override void Apply(TilemapStructure tilemap)
        {
            int targetTileId = (int)TargetTile;
            int replaceTileId = (int)ReplacedBy;
            for (int i = 0; i < Repetitions; i++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    for (int y = 0; y < tilemap.Height; y++)
                    {
                        var tile = tilemap.GetTile(x, y);
                        if (tile == targetTileId)
                        {
                            var neighbors = tilemap.GetNeighbors(x, y);
                            int targetTilesCount = neighbors.Count(a => a.Value == targetTileId);

                            if (targetTilesCount < MinAlive)
                            {
                                if (ReplaceByDominantTile)
                                {
                                    var dominantTile = neighbors
                                        .GroupBy(a => a.Value)
                                        .OrderByDescending(a => a.Count())
                                        .Select(a => a.Key)
                                        .First();

                                    tilemap.SetTile(x, y, dominantTile);
                                }
                                else
                                {
                                    tilemap.SetTile(x, y, replaceTileId);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
