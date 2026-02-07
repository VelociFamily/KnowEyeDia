using KnowEyeDia.Domain.Entities;
using KnowEyeDia.Domain.Interfaces;
using UnityEngine;

namespace KnowEyeDia.Domain.UseCases
{
    public class WorldGenerationUseCase : IWorldService
    {
        public WorldData CurrentWorld { get; private set; }

        public WorldData GenerateWorld(int width, int depth, int seed, float scale, int maxElevation)
        {
            CurrentWorld = new WorldData(width, depth);
            Random.InitState(seed);
            
            // Random offsets for noise maps to ensure variety
            Vector2 heightOffset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));
            Vector2 tempOffset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));
            Vector2 moistureOffset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

            // Adjust scale for larger biomes. 
            // If user passes ~0.05-0.1, we might need even smaller for 32x32 blocks.
            // Let's use a fixed base scale modifier to ensure large features.
            float baseScale = scale * 0.5f; 

            // Iterate in steps of 2 to ensure 2x2 tile blocks
            for (int x = 0; x < width; x += 2)
            {
                for (int z = 0; z < depth; z += 2)
                {
                    float xCoord = (float)x + heightOffset.x;
                    float zCoord = (float)z + heightOffset.y;
                    
                    // Height Noise (0 to 1)
                    float heightSample = Mathf.PerlinNoise(xCoord * baseScale, zCoord * baseScale);
                    
                    // Temperature Noise (0 to 1)
                    float tempSample = Mathf.PerlinNoise(((float)x + tempOffset.x) * baseScale, ((float)z + tempOffset.y) * baseScale);

                    // Moisture Noise (0 to 1)
                    float moistureSample = Mathf.PerlinNoise(((float)x + moistureOffset.x) * baseScale, ((float)z + moistureOffset.y) * baseScale);

                    TileType tile = TileType.Water;
                    
                    // Height threshold for Water vs Land
                    if (heightSample < 0.35f)
                    {
                        tile = TileType.Water;
                    }
                    else if (heightSample > 0.85f)
                    {
                        tile = TileType.Stone; // High elevation -> Mountains
                    }
                    else
                    {
                        // Land Biome Selection
                        // Shoreline check: Close to water
                        if (heightSample < 0.40f) // 0.35 to 0.40
                        {
                            tile = TileType.Island; // Beach/Shore uses Island tiles
                        }
                        else
                        {
                            // Biome based on Temp & Moisture
                            if (tempSample < 0.3f)
                            {
                                tile = TileType.Snow; // Cold
                            }
                            else if (tempSample > 0.7f)
                            {
                                tile = TileType.Desert; // Hot
                            }
                            else
                            {
                                // Moderate Temperature
                                if (moistureSample < 0.4f)
                                {
                                    tile = TileType.Dirt; // Dry-ish land
                                }
                                else
                                {
                                    tile = TileType.Grass; // Standard
                                }
                            }
                        }
                    }

            // Apply to 2x2 block
                    SetBlock(x, z, tile, CurrentWorld);
                    
                    // Set height map (for visual elevation if needed later)
                    int elevation = Mathf.FloorToInt(heightSample * maxElevation);
                    SetHeightBlock(x, z, elevation, CurrentWorld);
                }
            }

            // Shoreline Enforcement Pass
            // Ensure any land tile touching Water becomes an Island tile (Buffer Zone)
            // This prevents "Hard" biome edges against water.
            ApplyShorelineBuffer(width, depth, CurrentWorld);

            return CurrentWorld;
        }

        private void ApplyShorelineBuffer(int width, int depth, WorldData world)
        {
            // We need a temporary buffer or we process in strict order. 
            // Since we are only turning Land -> Island (downgrading), 
            // doing it in-place is mostly fine, but let's be safe to avoid cascading erosion.
            // Actually, for a single 1-tile buffer, cascading is key if we wanted N tiles, 
            // but for just 1 tile, in-place check against original "Water" is best.
            // But we don't have a copy. 
            // Simple check: If I am Land AND I have a Water neighbor, I become Island.
            
            System.Collections.Generic.List<Vector2Int> tilesToErode = new System.Collections.Generic.List<Vector2Int>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    TileType current = world.TileMap[x, z];
                    
                    // Only check Land tiles that aren't already Island
                    if (current != TileType.Water && current != TileType.Island && current != TileType.Empty)
                    {
                        if (HasWaterNeighbor(x, z, world))
                        {
                            tilesToErode.Add(new Vector2Int(x, z));
                        }
                    }
                }
            }

            foreach (var pos in tilesToErode)
            {
                world.TileMap[pos.x, pos.y] = TileType.Island;
            }
        }

        private bool HasWaterNeighbor(int x, int z, WorldData world)
        {
            if (IsWater(x + 1, z, world)) return true;
            if (IsWater(x - 1, z, world)) return true;
            if (IsWater(x, z + 1, world)) return true;
            if (IsWater(x, z - 1, world)) return true;
            return false;
        }

        private bool IsWater(int x, int z, WorldData world)
        {
            if (world.IsValid(x, z))
            {
                return world.TileMap[x, z] == TileType.Water;
            }
            return false;
        }

        private void SetBlock(int x, int z, TileType type, WorldData world)
        {
            if (world.IsValid(x, z)) world.TileMap[x, z] = type;
            if (world.IsValid(x + 1, z)) world.TileMap[x + 1, z] = type;
            if (world.IsValid(x, z + 1)) world.TileMap[x, z + 1] = type;
            if (world.IsValid(x + 1, z + 1)) world.TileMap[x + 1, z + 1] = type;
        }

        private void SetHeightBlock(int x, int z, int height, WorldData world)
        {
            if (world.IsValid(x, z)) world.HeightMap[x, z] = height;
            if (world.IsValid(x + 1, z)) world.HeightMap[x + 1, z] = height;
            if (world.IsValid(x, z + 1)) world.HeightMap[x, z + 1] = height;
            if (world.IsValid(x + 1, z + 1)) world.HeightMap[x + 1, z + 1] = height;
        }

        public float GetHeightAt(float x, float z)
        {
            if (CurrentWorld == null) return 0f;

            // Simple snapping to grid for now. 
            int gridX = Mathf.RoundToInt(x);
            int gridZ = Mathf.RoundToInt(z);

            return CurrentWorld.GetHeight(gridX, gridZ); 
        }
    }
}
