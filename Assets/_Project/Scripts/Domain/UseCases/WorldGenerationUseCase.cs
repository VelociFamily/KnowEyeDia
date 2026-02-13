using KnowEyeDia.Domain.Entities;
using KnowEyeDia.Domain.Interfaces;
using UnityEngine;
using System.Collections.Generic;

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
            Vector2 detailOffset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

            // Ocean border size - ensures islands don't touch edges
            int oceanBorder = 65;
            
            // Adjust scale for larger biomes
            float baseScale = scale * 0.5f; 

            // Initialize all as water
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    CurrentWorld.TileMap[x, z] = TileType.Water;
                }
            }

            // Playable area (inside the ocean border)
            int playableWidth = width - (oceanBorder * 2);
            int playableDepth = depth - (oceanBorder * 2);

            // Iterate in steps of 2 to ensure 2x2 tile blocks
            for (int x = oceanBorder; x < width - oceanBorder; x += 2)
            {
                for (int z = oceanBorder; z < depth - oceanBorder; z += 2)
                {
                    // Normalize coordinates relative to playable area for better generation
                    float normX = (x - oceanBorder) / (float)playableWidth;
                    float normZ = (z - oceanBorder) / (float)playableDepth;

                    float xCoord = (float)x + heightOffset.x;
                    float zCoord = (float)z + heightOffset.y;
                    
                    // Multi-octave noise for rugged coastlines
                    float heightSample = GetMultiOctaveNoise(xCoord, zCoord, baseScale);
                    
                    // Add detail noise for coastline raggedness
                    float detailNoise = Mathf.PerlinNoise(
                        ((float)x + detailOffset.x) * baseScale * 3f, 
                        ((float)z + detailOffset.y) * baseScale * 3f);
                    
                    // Apply detail to height for rougher edges
                    heightSample = heightSample * 0.85f + detailNoise * 0.15f;

                    // Temperature Noise (0 to 1)
                    float tempSample = Mathf.PerlinNoise(((float)x + tempOffset.x) * baseScale, ((float)z + tempOffset.y) * baseScale);

                    // Moisture Noise (0 to 1)
                    float moistureSample = Mathf.PerlinNoise(((float)x + moistureOffset.x) * baseScale, ((float)z + moistureOffset.y) * baseScale);

                    TileType tile = TileType.Water;
                    
                    // Generate large central island 
                    bool isCentralIsland = IsCentralIsland(normX, normZ, heightSample);
                    
                    // Generate smaller scattered islands
                    bool isSmallIsland = IsSmallIsland(normX, normZ, heightSample);

                    // Height threshold for Water vs Land
                    if (isCentralIsland || isSmallIsland)
                    {
                        if (isCentralIsland)
                        {
                            // Add river and lake generation to main island
                            float riverNoise = Mathf.PerlinNoise(((float)x + detailOffset.x) * baseScale * 2f, ((float)z + detailOffset.y) * baseScale * 2f);
                            float lakeNoise = Mathf.PerlinNoise(((float)x + detailOffset.x) * baseScale * 1.5f + 12.3f, ((float)z + detailOffset.y) * baseScale * 1.5f + 8.7f);
                            
                            // Create rivers (thin water channels)
                            if (riverNoise < 0.20f && heightSample > 0.35f && heightSample < 0.70f)
                            {
                                tile = TileType.Water; // River
                            }
                            // Create lakes (larger water bodies)
                            else if (lakeNoise < 0.15f && heightSample > 0.40f && heightSample < 0.65f)
                            {
                                tile = TileType.Water; // Lake
                            }
                            else if (heightSample < 0.38f)
                            {
                                tile = TileType.Stone; // High elevation -> Mountains
                            }
                            else if (heightSample < 0.43f) // Shore beach
                            {
                                tile = TileType.Island;
                            }
                            else if (tempSample < 0.3f)
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
                        else if (isSmallIsland)
                        {
                            // Small islands use stone/dirt/island primarily
                            if (heightSample < 0.4f)
                            {
                                tile = TileType.Island; // Beaches
                            }
                            else if (moistureSample < 0.5f)
                            {
                                tile = TileType.Stone; // Rocky
                            }
                            else
                            {
                                tile = TileType.Dirt; // Less lush biome
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

        private float GetMultiOctaveNoise(float x, float y, float baseScale)
        {
            float result = 0f;
            float amplitude = 1f;
            float frequency = 1f;
            float maxValue = 0f;

            for (int i = 0; i < 3; i++)
            {
                result += Mathf.PerlinNoise(x * baseScale * frequency, y * baseScale * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= 0.5f;
                frequency *= 2f;
            }

            return result / maxValue;
        }

        private bool IsCentralIsland(float normX, float normZ, float heightSample)
        {
            // Create an irregular, non-circular continent by stacking multiple shape patterns
            float centerX = 0.5f;
            float centerZ = 0.5f;
            float distFromCenter = Mathf.Sqrt(
                (normX - centerX) * (normX - centerX) + 
                (normZ - centerZ) * (normZ - centerZ)
            );

            // Smaller base radius to give islands more room
            float baseRadius = 0.30f;

            // Use multiple independent noise patterns as separate "landmasses" stacked together
            // Increased amplitudes for bigger peninsulas
            float mainShape = Mathf.PerlinNoise(normX * 1.5f, normZ * 1.5f);
            float shapeVariation1 = Mathf.PerlinNoise(normX * 2.2f + 5.5f, normZ * 2.2f + 3.3f);
            float shapeVariation2 = Mathf.PerlinNoise(normX * 1.8f + 9.2f, normZ * 1.8f + 7.1f);
            
            // Combine shapes to create less circular landmass - increased weights for more impact
            float combinedShape = (mainShape * 0.4f + shapeVariation1 * 0.4f + shapeVariation2 * 0.2f);

            // Highly detailed coastline with many octaves for ruggediness
            float coarse = Mathf.PerlinNoise(normX * 3f, normZ * 3f);
            float medium = Mathf.PerlinNoise(normX * 6f, normZ * 6f);
            float fine = Mathf.PerlinNoise(normX * 12f, normZ * 12f);
            float detail = Mathf.PerlinNoise(normX * 24f, normZ * 24f);
            float microDetail = Mathf.PerlinNoise(normX * 48f, normZ * 48f);
            
            // Weighted combination for very complex coastline - increased amplitude
            float coastlineComplexity = (coarse * 0.4f + medium * 0.3f + fine * 0.15f + detail * 0.1f + microDetail * 0.05f);
            
            // Create much more extreme variation for rugged edges - increased peninsula amplitude
            float variableRadius = baseRadius + (combinedShape - 0.5f) * 0.28f + (coastlineComplexity - 0.5f) * 0.22f;
            
            // Add angular variation for organic randomness
            float angleFromCenter = Mathf.Atan2(normZ - centerZ, normX - centerX);
            float angularNoise1 = Mathf.PerlinNoise(angleFromCenter * 3f, distFromCenter * 4f);
            float angularNoise2 = Mathf.PerlinNoise(angleFromCenter * 5f + 2.7f, distFromCenter * 6f);
            
            float angularVariation = (angularNoise1 * 0.6f + angularNoise2 * 0.4f) - 0.5f;
            variableRadius += angularVariation * 0.15f;

            // Clamp to reasonable bounds
            variableRadius = Mathf.Clamp(variableRadius, baseRadius * 0.35f, baseRadius * 1.8f);

            // Add subtle erosion patterns - reduced aggressiveness for bigger peninsulas
            float erosionNoise1 = Mathf.PerlinNoise(normX * 4.5f, normZ * 4.5f);
            float erosionNoise2 = Mathf.PerlinNoise(normX * 9f + 11.3f, normZ * 9f + 8.9f);
            float erosionPattern = (erosionNoise1 * 0.6f + erosionNoise2 * 0.4f);
            
            // Create subtle "bite marks" - much less aggressive than before
            if (erosionPattern > 0.72f && distFromCenter < variableRadius * 0.90f)
            {
                variableRadius *= 0.85f; // Reduced from 0.7f
            }
            
            // Create internal holes/bays that cut deeper - less aggressive
            if (erosionNoise1 > 0.76f && distFromCenter < baseRadius * 0.75f)
            {
                variableRadius *= 0.80f; // Reduced from 0.65f
            }

            return distFromCenter < variableRadius;
        }

        private bool IsSmallIsland(float normX, float normZ, float heightSample)
        {
            // Small islands appear in outer regions (avoid center)
            float centerDistance = Mathf.Sqrt((normX - 0.5f) * (normX - 0.5f) + (normZ - 0.5f) * (normZ - 0.5f));
            
            // Keep area around main continent clear - slightly reduced buffer
            if (centerDistance < 0.36f) return false;
            
            // Don't generate at the very edges to avoid cut-off islands - reduced buffer
            float edgeDistance = Mathf.Min(
                Mathf.Min(normX, 1f - normX),
                Mathf.Min(normZ, 1f - normZ)
            );
            if (edgeDistance < 0.05f) return false;

            // Use multi-scale noise for archipelago patterns with more variation
            float islandNoise1 = Mathf.PerlinNoise(normX * 2.5f, normZ * 2.5f);
            float islandNoise2 = Mathf.PerlinNoise(normX * 5f, normZ * 5f);
            float islandNoise3 = Mathf.PerlinNoise(normX * 10f, normZ * 10f);
            float detailNoise = Mathf.PerlinNoise(normX * 20f, normZ * 20f);

            // Weighted combination for organic clustering with finer detail
            float combinedNoise = (islandNoise1 * 0.3f + islandNoise2 * 0.3f + islandNoise3 * 0.3f + detailNoise * 0.1f);

            // Lowered thresholds significantly for more islands on larger map
            bool smallCluster = combinedNoise > 0.55f && heightSample > 0.48f;
            bool tinyCluster = combinedNoise > 0.62f && heightSample > 0.50f;
            bool microCluster = combinedNoise > 0.68f && heightSample > 0.51f;

            return smallCluster || tinyCluster || microCluster;
        }

        private void ApplyShorelineBuffer(int width, int depth, WorldData world)
        {
            // Apply shoreline buffering with multiple passes for better raggedness
            List<Vector2Int> tilesToErode = new List<Vector2Int>();

            for (int pass = 0; pass < 2; pass++)
            {
                tilesToErode.Clear();

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

                // Apply erosion for this pass (creates rougher coastlines)
                foreach (var pos in tilesToErode)
                {
                    world.TileMap[pos.x, pos.y] = TileType.Island;
                }

                if (tilesToErode.Count == 0) break; // No more tiles to erode
            }
        }

        private bool HasWaterNeighbor(int x, int z, WorldData world)
        {
            if (IsWater(x + 1, z, world)) return true;
            if (IsWater(x - 1, z, world)) return true;
            if (IsWater(x, z + 1, world)) return true;
            if (IsWater(x, z - 1, world)) return true;
            // Diagonal neighbors for rougher coastlines
            if (IsWater(x + 1, z + 1, world)) return true;
            if (IsWater(x - 1, z + 1, world)) return true;
            if (IsWater(x + 1, z - 1, world)) return true;
            if (IsWater(x - 1, z - 1, world)) return true;
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

        public bool IsWalkable(float x, float z)
        {
            if (CurrentWorld == null) return false;

            const float sampleRadius = 0.45f;

            return IsSampleWalkable(x - sampleRadius, z - sampleRadius)
                && IsSampleWalkable(x + sampleRadius, z - sampleRadius)
                && IsSampleWalkable(x - sampleRadius, z + sampleRadius)
                && IsSampleWalkable(x + sampleRadius, z + sampleRadius);
        }

        public Vector2 GetDefaultSpawnPosition()
        {
            if (CurrentWorld == null) return Vector2.zero;

            int startX = Mathf.RoundToInt(CurrentWorld.Width * 0.5f);
            int startZ = Mathf.RoundToInt(CurrentWorld.Depth * 0.5f);

            return FindNearestWalkable(startX, startZ);
        }

        private Vector2 FindNearestWalkable(int startX, int startZ)
        {
            int maxRadius = Mathf.Max(CurrentWorld.Width, CurrentWorld.Depth);

            for (int radius = 0; radius <= maxRadius; radius++)
            {
                int minX = startX - radius;
                int maxX = startX + radius;
                int minZ = startZ - radius;
                int maxZ = startZ + radius;

                // Check perimeter of the square ring at this radius.
                for (int x = minX; x <= maxX; x++)
                {
                    if (TryGetWalkableAt(x, minZ, out var pos)) return pos;
                    if (TryGetWalkableAt(x, maxZ, out pos)) return pos;
                }

                for (int z = minZ + 1; z <= maxZ - 1; z++)
                {
                    if (TryGetWalkableAt(minX, z, out var pos)) return pos;
                    if (TryGetWalkableAt(maxX, z, out pos)) return pos;
                }
            }

            return Vector2.zero;
        }

        private bool TryGetWalkableAt(int x, int z, out Vector2 position)
        {
            position = Vector2.zero;
            if (!IsTileWalkable(x, z)) return false;

            position = new Vector2(x, z);
            return true;
        }

        private bool IsSampleWalkable(float x, float z)
        {
            int gridX = Mathf.FloorToInt(x);
            int gridZ = Mathf.FloorToInt(z);
            return IsTileWalkable(gridX, gridZ);
        }

        private bool IsTileWalkable(int x, int z)
        {
            if (!CurrentWorld.IsValid(x, z)) return false;
            if (!IsWithinWalkableBounds(x, z)) return false;

            TileType tile = CurrentWorld.TileMap[x, z];
            return tile != TileType.Water && tile != TileType.Empty;
        }

        private bool IsWithinWalkableBounds(int x, int z)
        {
            // Keep a 1-tile buffer around the entire map so edges are not steppable.
            return x > 0 && x < CurrentWorld.Width - 1 && z > 0 && z < CurrentWorld.Depth - 1;
        }
    }
}
