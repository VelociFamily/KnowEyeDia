
using UnityEngine;

namespace KnowEyeDia.Domain.Interfaces
{
    public interface IWorldService
    {
        float GetHeightAt(float x, float z);
        bool IsWalkable(float x, float z);
        Vector2 GetDefaultSpawnPosition();
    }
}
