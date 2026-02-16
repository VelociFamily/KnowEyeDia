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

        [Header("Decoration - Grass Sprites")]
        [SerializeField] private Transform _detailGrassParent;
        [SerializeField] private string _detailGrassPrefabFolder = "DetailGrass";
        [SerializeField] private float _detailGrassZ = -1f;
        [SerializeField] private int _detailGrassSortingOrder = 28;
        [SerializeField] private string _detailGrassSortingLayer = "Ground";

        private GameObject[] _cachedDetailGrassPrefabs;

        [Header("Decoration - Trees")]
        [SerializeField] private Transform _treeParent;
        [SerializeField] private BiomeTreeSet[] _treeSets;
        [SerializeField] private float _treeZ = -1f;
        [SerializeField] private int _treeSortingOrder = 40;
        [SerializeField] private string _treeSortingLayer = "Ground";

        [Header("Decoration - Grass")]
        [SerializeField, Range(0f, 1f)] private float _detailGrassChance = 0.2f;
        [SerializeField] private TileType[] _detailGrassAllowedBiomes = { TileType.Grass, TileType.Dirt };
        [SerializeField] private Vector2 _detailGrassScaleRange = new Vector2(0.9f, 1.1f);

        [System.Serializable]
        private class BiomeTreeSet
        {
            public TileType biome;
            public GameObject[] prefabs;
            [Range(0f, 1f)] public float spawnChance = 0.03f;
            public Vector2 scaleRange = new Vector2(3.5f, 4.5f);
        }

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

            ClearTreeObjects();
            ClearDetailGrassObjects();

            SetMapSorting(_waterMap, -1000000, "Default");
            SetMapSorting(_islandMap, -999995, "Default");
            SetMapSorting(_dirtMap, -999990, "Default");
            SetMapSorting(_desertMap, -999985, "Default");
            SetMapSorting(_grassMap, -999980, "Default");
            SetMapSorting(_snowMap, -999975, "Default");
            SetMapSorting(_stoneMap, -999970, "Default");

            // Explicit Z-Layering (Backup for parallax/physics)
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

                        TrySpawnDecorations(type, pos);
                    }
                    tileCount++;
                }
            }
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

        private void TrySpawnDecorations(TileType type, Vector3Int cellPos)
        {
            TrySpawnDetailGrass(type, cellPos);
            TrySpawnTree(type, cellPos);
        }

        private void TrySpawnDetailGrass(TileType type, Vector3Int cellPos)
        {
            if (_cachedDetailGrassPrefabs == null || _cachedDetailGrassPrefabs.Length == 0)
            {
                LoadDetailGrassPrefabs();
            }
            if (_cachedDetailGrassPrefabs == null || _cachedDetailGrassPrefabs.Length == 0) return;

            if (!IsAllowedBiome(type, _detailGrassAllowedBiomes)) return;
            if (Random.value > _detailGrassChance) return;

            GameObject prefab = _cachedDetailGrassPrefabs[Random.Range(0, _cachedDetailGrassPrefabs.Length)];
            if (prefab == null) return;

            Transform parent = GetOrCreateDetailGrassParent();
            Vector3 spawnPos = GetCellCenterWorld(cellPos);
            spawnPos.z = _detailGrassZ;

            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, parent);
            ApplyDetailGrassSorting(instance);

            float scale = Random.Range(_detailGrassScaleRange.x, _detailGrassScaleRange.y);
            instance.transform.localScale = new Vector3(scale, scale, scale);
        }

        private void TrySpawnTree(TileType type, Vector3Int cellPos)
        {
            BiomeTreeSet set = GetTreeSet(type);
            if (set == null || set.prefabs == null || set.prefabs.Length == 0) return;
            if (Random.value > set.spawnChance) return;

            GameObject prefab = set.prefabs[Random.Range(0, set.prefabs.Length)];
            if (prefab == null) return;

            Transform parent = GetOrCreateTreeParent();
            Vector3 spawnPos = GetCellCenterWorld(cellPos);
            spawnPos.z = _treeZ;

            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, parent);
            
            // Validate scale range from config (Inspector might have serialized it as 0,0)
            Vector2 range = set.scaleRange;
            if (range == Vector2.zero) range = new Vector2(3.5f, 4.5f); // Updated Default fallback

            // Calculate scale
            float scale = Random.Range(range.x, range.y);

            // Final safety check against NaN or Zero
            if (float.IsNaN(scale) || scale <= 0.001f) scale = 1.0f;

            instance.transform.localScale = new Vector3(scale, scale, scale);

            ApplyTreeSorting(instance);
        }

        private BiomeTreeSet GetTreeSet(TileType type)
        {
            if (_treeSets == null || _treeSets.Length == 0) return null;

            for (int i = 0; i < _treeSets.Length; i++)
            {
                if (_treeSets[i] != null && _treeSets[i].biome == type) return _treeSets[i];
            }

            return null;
        }

        private Vector3 GetCellCenterWorld(Vector3Int cellPos)
        {
            Tilemap map = _grassMap ?? _dirtMap ?? _islandMap ?? _snowMap ?? _stoneMap ?? _desertMap ?? _waterMap;
            return map != null ? map.GetCellCenterWorld(cellPos) : new Vector3(cellPos.x + 0.5f, cellPos.y + 0.5f, 0f);
        }

        private bool IsAllowedBiome(TileType type, TileType[] allowed)
        {
            if (allowed == null || allowed.Length == 0) return false;
            for (int i = 0; i < allowed.Length; i++)
            {
                if (allowed[i] == type) return true;
            }
            return false;
        }

        private void ClearMap(Tilemap map)
        {
            if (map != null) map.ClearAllTiles();
        }

        private void ClearTreeObjects()
        {
            if (_treeParent == null) return;

            for (int i = _treeParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_treeParent.GetChild(i).gameObject);
            }
        }

        private void ClearDetailGrassObjects()
        {
            if (_detailGrassParent == null) return;

            for (int i = _detailGrassParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_detailGrassParent.GetChild(i).gameObject);
            }
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

        private void SetMapSorting(Tilemap map, int order, string sortingLayerName)
        {
            if (map != null)
            {
                var renderer = map.GetComponent<TilemapRenderer>();
                if (renderer != null) 
                {
                    renderer.sortingOrder = order;
                    renderer.sortingLayerName = sortingLayerName;
                }
            }
        }

        private Transform GetOrCreateTreeParent()
        {
            if (_treeParent != null) return _treeParent;

            GameObject container = new GameObject("Trees");
            container.transform.SetParent(transform, false);
            _treeParent = container.transform;
            return _treeParent;
        }

        private Transform GetOrCreateDetailGrassParent()
        {
            if (_detailGrassParent != null) return _detailGrassParent;

            GameObject container = new GameObject("DetailGrass");
            container.transform.SetParent(transform, false);
            _detailGrassParent = container.transform;
            return _detailGrassParent;
        }

        private void ApplyTreeSorting(GameObject instance)
        {
            if (instance == null) return;

            // Sort trees by Y position (bottom of hitbox)
            // Lower Y (foreground) should have HIGHER sorting order to draw on top of Higher Y (background).
            float yPos = instance.transform.position.y;
            
            // Try to use collider bottom for more accurate sorting
            Collider2D col = instance.GetComponent<Collider2D>();
            if (col != null)
            {
                yPos = col.bounds.min.y;
            }

            int depthSortOrder = Mathf.RoundToInt(-yPos * 100);

            SpriteRenderer[] renderers = instance.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                // Add a small offset to tree sprites to ensure they don't Z-fight with things exactly at their base
                // or just keep them same plane.
                renderers[i].sortingOrder = _treeSortingOrder + depthSortOrder;
                renderers[i].sortingLayerName = _treeSortingLayer;
                renderers[i].enabled = true;
            }
        }


        private void ApplyDetailGrassSorting(GameObject instance)
        {
            if (instance == null) return;

            // Sort grass by Y position as well
            float yPos = instance.transform.position.y;
            
            // Try to use collider bottom (if grass has one, unlikely but safe)
            Collider2D col = instance.GetComponent<Collider2D>();
            if (col != null)
            {
                yPos = col.bounds.min.y;
            }

            int depthSortOrder = Mathf.RoundToInt(-yPos * 100);

            SpriteRenderer[] renderers = instance.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sortingOrder = _detailGrassSortingOrder + depthSortOrder;
                renderers[i].sortingLayerName = _detailGrassSortingLayer;
            }
        }

        private void LoadDetailGrassPrefabs()
        {
            _cachedDetailGrassPrefabs = Resources.LoadAll<GameObject>(_detailGrassPrefabFolder);
        }
     }
 }
