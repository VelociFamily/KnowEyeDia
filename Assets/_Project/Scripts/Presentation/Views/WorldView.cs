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
        [SerializeField] private Tilemap _islandMap;

        [Header("Rule Tiles")]
        [SerializeField] private TileBase _snowTile;
        [SerializeField] private TileBase _grassTile;
        [SerializeField] private TileBase _stoneTile;
        [SerializeField] private TileBase _desertTile;
        [SerializeField] private TileBase _dirtTile;
        [SerializeField] private TileBase _waterTile;
        [SerializeField] private TileBase _islandTile;

        public void Render(WorldData worldData)
        {
            // Clear all layers to start fresh
            ClearMap(_snowMap);
            ClearMap(_grassMap);
            ClearMap(_stoneMap);
            ClearMap(_desertMap);
            ClearMap(_dirtMap);
            ClearMap(_waterMap);
            ClearMap(_islandMap);

            // Explicit Z-Layering (Lower Z = Closer to Camera = On Top)
            // Water (Deepest) -> Island -> Dirt -> Desert -> Grass -> Snow -> Stone (Highest)
            SetMapZ(_waterMap, 5.0f);
            SetMapZ(_islandMap, 4.0f);
            SetMapZ(_dirtMap, 3.0f);
            SetMapZ(_desertMap, 2.5f);
            SetMapZ(_grassMap, 2.0f);
            SetMapZ(_snowMap, 1.5f);
            SetMapZ(_stoneMap, 1.0f);

            if (worldData == null)
            {
                Debug.LogWarning("[WorldView] WorldData is null.");
                return;
            }

            int tileCount = 0;
            for (int x = 0; x < worldData.Width; x++)
            {
                for (int z = 0; z < worldData.Depth; z++)
                {
                    TileType type = worldData.TileMap[x, z];
                    if (type == TileType.Empty) continue;

                    Vector3Int pos = new Vector3Int(x, z, 0);

                    if (type == TileType.Water)
                    {
                        if (_waterMap != null) _waterMap.SetTile(pos, _waterTile);
                    }
                    else
                    {
                        // Land Logic
                        
                        // 1. Base Layer: Always Island (to handle water edges)
                        if (_islandMap != null) _islandMap.SetTile(pos, _islandTile);

                        // 2. Neighbor Underlay Logic
                        // "Overlap by one more" -> Check neighbors up to 2 tiles away
                        // This pulls "lower" biomes (like Dirt) underneath "higher" biomes (like Desert)
                        // to hide the Island (green) foundation at the edges.
                        int range = 2;
                        for (int nx = x - range; nx <= x + range; nx++)
                        {
                            for (int nz = z - range; nz <= z + range; nz++)
                            {
                                if (nx == x && nz == z) continue;
                                TryRenderUnderlay(nx, nz, type, pos, worldData);
                            }
                        }

                        // 3. Main Layer
                        if (type != TileType.Island)
                        {
                            Tilemap targetMap = GetMapForType(type);
                            TileBase tileToPlace = GetTileBaseForType(type);
                            if (targetMap != null && tileToPlace != null)
                            {
                                targetMap.SetTile(pos, tileToPlace);
                            }
                        }
                    }
                    tileCount++;
                }
            }
            
            Debug.Log($"[WorldView] Rendered {tileCount} tiles for world size {worldData.Width}x{worldData.Depth}.");
        }

        // Checks if the neighbor at (nx, nz) is a biome that should be drawn UNDER the current tile
        private void TryRenderUnderlay(int nx, int nz, TileType currentType, Vector3Int currentPos, WorldData world)
        {
            if (!world.IsValid(nx, nz)) return;
            
            TileType neighborType = world.TileMap[nx, nz];
            
            // Only care about Land neighbors
            if (neighborType == TileType.Empty || neighborType == TileType.Water || neighborType == TileType.Island) return;
            if (neighborType == currentType) return;

            // Check Layer Hierarchy
            // If Neighbor is "Behind" Current (Higher Z value), we draw it as backing
            float currentZ = GetZForType(currentType);
            float neighborZ = GetZForType(neighborType);

            if (neighborZ > currentZ)
            {
                Tilemap neighborMap = GetMapForType(neighborType);
                TileBase neighborTile = GetTileBaseForType(neighborType);
                if (neighborMap != null && neighborTile != null)
                {
                    // Render the neighbor's tile at the CURRENT position
                    // Since neighbor's map Z is higher (further back), it will draw behind the current tile
                    neighborMap.SetTile(currentPos, neighborTile);
                }
            }
        }

        private float GetZForType(TileType type)
        {
            switch (type)
            {
                case TileType.Water: return 5.0f;
                case TileType.Island: return 4.0f;
                case TileType.Dirt: return 3.0f;
                case TileType.Desert: return 2.5f;
                case TileType.Grass: return 2.0f;
                case TileType.Snow: return 1.5f;
                case TileType.Stone: return 1.0f;
                default: return 0f;
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
                case TileType.Island: return _islandMap;
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
                case TileType.Island: return _islandTile;
                default:
                    // Debug.LogWarning($"[WorldView] No TileBase defined for TileType: {type}");
                    return null;
            }
        }

        private void SetMapZ(Tilemap map, float zPos)
        {
            if (map != null)
            {
                var pos = map.transform.position;
                map.transform.position = new Vector3(pos.x, pos.y, zPos);
            }
        }
    }
}
