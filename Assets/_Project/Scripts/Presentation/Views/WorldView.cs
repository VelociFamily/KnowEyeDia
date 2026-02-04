using KnowEyeDia.Domain.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace KnowEyeDia.Presentation.Views
{
    public class WorldView : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField] private Tilemap _snowMap;
        [SerializeField] private Tilemap _grassMap;
        [SerializeField] private Tilemap _stoneMap;
        [SerializeField] private Tilemap _desertMap;
        [SerializeField] private Tilemap _dirtMap;
        [SerializeField] private Tilemap _waterMap;

        [Header("Rule Tiles")]
        [SerializeField] private TileBase _snowTile;
        [SerializeField] private TileBase _grassTile;
        [SerializeField] private TileBase _stoneTile;
        [SerializeField] private TileBase _desertTile;
        [SerializeField] private TileBase _dirtTile;
        [SerializeField] private TileBase _waterTile;

        public void Render(WorldData worldData)
        {
            // Clear all layers
            ClearMap(_snowMap);
            ClearMap(_grassMap);
            ClearMap(_stoneMap);
            ClearMap(_desertMap);
            ClearMap(_dirtMap);
            ClearMap(_waterMap);

            if (worldData == null) return;

            for (int x = 0; x < worldData.Width; x++)
            {
                for (int z = 0; z < worldData.Depth; z++)
                {
                    TileType type = worldData.TileMap[x, z];
                    if (type == TileType.Empty) continue;

                    TileBase tileToPlace = GetTileBaseForType(type);
                    Tilemap targetMap = GetMapForType(type);

                    if (targetMap != null && tileToPlace != null)
                    {
                        Vector3Int pos = new Vector3Int(x, z, 0); 
                        targetMap.SetTile(pos, tileToPlace);
                        
                        // "Water is on bottom layer"
                        // If we want water explicitly underneath everything, we could do:
                        // if (type != TileType.Water) _waterMap.SetTile(pos, _waterTile);
                        // But user said "If there is a lake make sure nothing covers it up".
                        // This implies we rely on the UseCase to define where Water is.
                    }
                }
            }
        }

        private void ClearMap(Tilemap map)
        {
            if (map != null) map.ClearAllTiles();
        }

        private Tilemap GetMapForType(TileType type)
        {
            switch (type)
            {
                case TileType.Snow: return _snowMap;
                case TileType.Grass: return _grassMap;
                case TileType.Stone: return _stoneMap;
                case TileType.Desert: return _desertMap;
                case TileType.Dirt: return _dirtMap;
                case TileType.Water: return _waterMap;
                default: return null;
            }
        }

        private TileBase GetTileBaseForType(TileType type)
        {
            switch (type)
            {
                case TileType.Snow: return _snowTile;
                case TileType.Grass: return _grassTile;
                case TileType.Stone: return _stoneTile;
                case TileType.Desert: return _desertTile;
                case TileType.Dirt: return _dirtTile;
                case TileType.Water: return _waterTile;
                default: return null;
            }
        }
    }
}
