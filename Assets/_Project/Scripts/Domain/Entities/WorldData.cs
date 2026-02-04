using UnityEngine;

namespace KnowEyeDia.Domain.Entities
{
    public class WorldData
    {
        public int Width { get; private set; }
        public int Depth { get; private set; }
        public int[,] HeightMap { get; private set; }
        public TileType[,] TileMap { get; private set; }

        public WorldData(int width, int depth)
        {
            Width = width;
            Depth = depth;
            HeightMap = new int[width, depth];
            TileMap = new TileType[width, depth];
        }

        public void SetHeight(int x, int z, int height)
        {
            if (IsValid(x, z)) HeightMap[x, z] = height;
        }

        public int GetHeight(int x, int z)
        {
            return IsValid(x, z) ? HeightMap[x, z] : 0;
        }

        public bool IsValid(int x, int z)
        {
            return x >= 0 && x < Width && z >= 0 && z < Depth;
        }
    }

    public enum TileType
    {
        Empty = 0,
        Grass = 1,
        Dirt = 2,
        Stone = 3,
        Water = 4,
        Desert = 5,
        Snow = 6
    }
}
