using KnowEyeDia.Domain.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace KnowEyeDia.Presentation.Views
{
    public class WorldView : MonoBehaviour
    {
        [SerializeField] private GameObject _grassTilePrefab;
        [SerializeField] private GameObject _dirtTilePrefab;
        [SerializeField] private GameObject _stoneTilePrefab;
        [SerializeField] private GameObject _waterTilePrefab;
        [SerializeField] private Transform _worldRoot;

        private List<GameObject> _activeTiles = new List<GameObject>();

        public void Render(WorldData worldData)
        {
            ClearWorld();

            if (worldData == null) return;

            for (int x = 0; x < worldData.Width; x++)
            {
                for (int z = 0; z < worldData.Depth; z++)
                {
                    TileType type = worldData.TileMap[x, z];
                    int height = worldData.GetHeight(x, z);
                    
                    if (type == TileType.Empty) continue;

                    GameObject prefab = GetPrefabForType(type);
                    if (prefab != null)
                    {
                        // Instantiate at x, height, z
                        GameObject instance = Instantiate(prefab, new Vector3(x, height, z), Quaternion.Euler(90, 0, 0), _worldRoot);
                        _activeTiles.Add(instance);
                        
                        // Optional: Create "sides" of the block if it's high up to avoid gaps, 
                        // or just stack blocks (expensive).
                        // For this iteration, we just place the top face.
                    }
                }
            }
        }

        private void ClearWorld()
        {
            foreach (var tile in _activeTiles)
            {
                Destroy(tile);
            }
            _activeTiles.Clear();
        }

        private GameObject GetPrefabForType(TileType type)
        {
            switch (type)
            {
                case TileType.Grass: return _grassTilePrefab;
                case TileType.Dirt: return _dirtTilePrefab;
                case TileType.Stone: return _stoneTilePrefab;
                case TileType.Water: return _waterTilePrefab;
                default: return null;
            }
        }
    }
}
