using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Tilemaps
{
    public class TileGrid : MonoBehaviour
    {
        public int Width, Height;
        public int TileSize, Seed;
        public Dictionary<int, Tile> Tiles { get; private set; }

        [Serializable]
        class GroundTiles : TileData
        {
            public GroundTileType TileType;
        }

        [Serializable]
        class ObjectTiles : TileData
        {
            public ObjectTileType TileType;
        }

        class TileData
        {
            public Sprite Sprite;
            public Color Color;
            public Tile Tile;
            public Tile.ColliderType ColliderType;
        }

        [SerializeField]
        private GroundTiles[] GroundTileTypes;
        [SerializeField]
        private ObjectTiles[] ObjectTileTypes;

        public Dictionary<TilemapType, TilemapStructure> Tilemaps;

        private void Awake()
        {
            Seed = UnityEngine.Random.Range(0, 100000);

            Tiles = InitializeTiles();

            Tilemaps = new Dictionary<TilemapType, TilemapStructure>();


            foreach (Transform child in transform)
            {
                var tilemap = child.GetComponent<TilemapStructure>();
                if (tilemap == null) continue;
                if (Tilemaps.ContainsKey(tilemap.Type))
                {
                    throw new Exception("Duplicate tilemap type: " + tilemap.Type);
                }
                Tilemaps.Add(tilemap.Type, tilemap);
            }

            foreach (var tilemap in Tilemaps.Values)
            {
                tilemap.Initialize();
            }

        }

        private Dictionary<int, Tile> InitializeTiles()
        {
            var dictionary = new Dictionary<int, Tile>();

 
            foreach (var tiletype in GroundTileTypes)
            {

                if (tiletype.TileType == 0) continue;

                var tile = tiletype.Tile == null ?
                    CreateTile(tiletype.Color, tiletype.Sprite) :
                    tiletype.Tile;
                tile.colliderType = tiletype.ColliderType;

                dictionary.Add((int)tiletype.TileType, tile);
            }

            foreach (var tiletype in ObjectTileTypes)
            {

                if (tiletype.TileType == 0) continue;

                var tile = tiletype.Tile == null ?
                    CreateTile(tiletype.Color, tiletype.Sprite) :
                    tiletype.Tile;
                tile.colliderType = tiletype.ColliderType;

                dictionary.Add((int)tiletype.TileType, tile);
            }

            return dictionary;
        }

        private Tile CreateTile(Color color, Sprite sprite)
        {
            bool setColor = false;
            Texture2D texture = sprite == null ? null : sprite.texture;
            if (texture == null)
            {
                setColor = true;
                texture = new Texture2D(TileSize, TileSize)
                {
                    filterMode = FilterMode.Point
                };
                sprite = Sprite.Create(texture, new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);

            }

            var tile = ScriptableObject.CreateInstance<Tile>();

            if (setColor)
            {
                color.a = 1;
                tile.color = color;
            }

            tile.sprite = sprite;

            return tile;
        }
    }
}
