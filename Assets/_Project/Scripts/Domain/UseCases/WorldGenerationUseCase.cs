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
            Vector2 offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Perlin Noise for height
                    float xCoord = (float)x / width * scale + offset.x;
                    float zCoord = (float)z / depth * scale + offset.y;
                    float sample = Mathf.PerlinNoise(xCoord, zCoord);

                    // Map sample 0-1 to elevation steps
                    int elevation = Mathf.FloorToInt(sample * maxElevation);
                    
                    CurrentWorld.SetHeight(x, z, elevation);
                    
                    // Simple tile type logic based on height
                    if (sample < 0.2f)
                        CurrentWorld.TileMap[x, z] = TileType.Water;
                    else if (sample < 0.4f)
                        CurrentWorld.TileMap[x, z] = TileType.Dirt;
                    else if (sample < 0.7f)
                        CurrentWorld.TileMap[x, z] = TileType.Grass;
                    else
                        CurrentWorld.TileMap[x, z] = TileType.Stone;
                }
            }

            return CurrentWorld;
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
