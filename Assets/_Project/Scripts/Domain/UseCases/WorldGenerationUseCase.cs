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

            // Iterate in steps of 2 to ensure 2x2 tile blocks
            for (int x = 0; x < width; x += 2)
            {
                for (int z = 0; z < depth; z += 2)
                {
                    // Generate noise values for the block
                    // Use scale/width implies scale is total repeats? Or scale is frequency?
                    // User param 'scale' usually means zoom. Larger scale = larger features (lower frequency).
                    // Or standard Unity: coord * scale.
                    // Let's assume passed 'scale' is frequency multiplier. For 32x32 blobs, we need low frequency.
                    // If scale is passed as e.g. 0.05.
                    
                    float xCoord = (float)x + heightOffset.x;
                    float zCoord = (float)z + heightOffset.y;
                    
                    // Height Noise
                    float heightSample = Mathf.PerlinNoise(xCoord * scale, zCoord * scale);
                    
                    // Temperature Noise (offset to be different)
                    float tempSample = Mathf.PerlinNoise(((float)x + tempOffset.x) * scale, ((float)z + tempOffset.y) * scale);

                    TileType tile = TileType.Water;
                    
                    if (heightSample < 0.3f)
                    {
                        tile = TileType.Water;
                    }
                    else if (heightSample > 0.85f)
                    {
                        tile = TileType.Stone; // Mountain tops
                    }
                    else
                    {
                        // Land Biomes based on Temperature
                        if (tempSample > 0.6f)
                        {
                            tile = TileType.Desert;
                        }
                        else if (tempSample < 0.3f)
                        {
                            tile = TileType.Snow;
                        }
                        else
                        {
                            // Grasslands
                             // Dirt patches in grass or just Grass?
                             // Let's mix in Dirt based on another noise or simple rule
                             // For now, pure Grass to look clean.
                             tile = TileType.Grass;
                             
                             // Optional: Dirt transition or patches
                             if (heightSample > 0.3f && heightSample < 0.35f)
                             {
                                 tile = TileType.Dirt; // Shoreline
                             }
                        }
                    }

                    // Apply to 2x2 block
                    SetBlock(x, z, tile, CurrentWorld);
                    
                    // Set height map (for visual elevation if needed later)
                    int elevation = Mathf.FloorToInt(heightSample * maxElevation);
                    CurrentWorld.SetHeight(x, z, elevation);
                    if (x + 1 < width) CurrentWorld.SetHeight(x + 1, z, elevation);
                    if (z + 1 < depth) CurrentWorld.SetHeight(x, z + 1, elevation);
                    if (x + 1 < width && z + 1 < depth) CurrentWorld.SetHeight(x + 1, z + 1, elevation);
                }
            }

            return CurrentWorld;
        }

        private void SetBlock(int x, int z, TileType type, WorldData world)
        {
            if (world.IsValid(x, z)) world.TileMap[x, z] = type;
            if (world.IsValid(x + 1, z)) world.TileMap[x + 1, z] = type;
            if (world.IsValid(x, z + 1)) world.TileMap[x, z + 1] = type;
            if (world.IsValid(x + 1, z + 1)) world.TileMap[x + 1, z + 1] = type;
        }

        public float GetHeightAt(float x, float z)
        {
            if (CurrentWorld == null) return 0f;

            // Simple snapping to grid for now. 
            // In a more complex game, we might interpolate between 4 neighbors.
            int gridX = Mathf.RoundToInt(x);
            int gridZ = Mathf.RoundToInt(z);

            return CurrentWorld.GetHeight(gridX, gridZ); // * VerticalScale if we have one (which is 1 unit for now)
        }
    }
}
