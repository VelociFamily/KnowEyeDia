using KnowEyeDia.Domain.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace KnowEyeDia.Presentation.Views
{
    public class WorldView : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Assign Tilemaps for each height level (0, 1, 2...). Ensure they are placed at correct Y height in scene.")]
        [SerializeField] private Tilemap[] _heightLayers;

        [Header("Rule Tiles")]
        [SerializeField] private TileBase _grassTile;
        [SerializeField] private TileBase _dirtTile;
        [SerializeField] private TileBase _stoneTile;
        [SerializeField] private TileBase _waterTile;

        public void Render(WorldData worldData)
        {
            // Clear all layers
            foreach (var tm in _heightLayers)
            {
                if(tm != null) tm.ClearAllTiles();
            }

            if (worldData == null || _heightLayers == null || _heightLayers.Length == 0) return;

            for (int x = 0; x < worldData.Width; x++)
            {
                for (int z = 0; z < worldData.Depth; z++)
                {
                    TileType type = worldData.TileMap[x, z];
                    int height = worldData.GetHeight(x, z);
                    
                    if (type == TileType.Empty) continue;

                    TileBase tileToPlace = GetTileBaseForType(type);
                    if (tileToPlace != null)
                    {
                        // Ensure we don't go out of bounds of our assigned layers
                        int layerIndex = Mathf.Clamp(height, 0, _heightLayers.Length - 1);
                        Tilemap targetMap = _heightLayers[layerIndex];
                        
                        if (targetMap != null)
                        {
                            // Map Z -> Y for the tilemap grid
                            Vector3Int pos = new Vector3Int(x, z, 0); 
                            targetMap.SetTile(pos, tileToPlace);

                            // OPTIONAL: Fill layers below this one to avoid floating tiles?
                            // For solid terrain, we usually just render the top surface or have a "wall" tile.
                            // For this MVP, we just render the top surface.
                        }
                    }
                }
            }
        }

        private TileBase GetTileBaseForType(TileType type)
        {
            switch (type)
            {
                case TileType.Grass: return _grassTile;
                case TileType.Dirt: return _dirtTile;
                case TileType.Stone: return _stoneTile;
                case TileType.Water: return _waterTile;
                default: return null;
            }
        }
    }
}
