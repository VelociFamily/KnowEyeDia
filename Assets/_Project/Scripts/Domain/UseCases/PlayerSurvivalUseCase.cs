using KnowEyeDia.Domain.Entities;
using UnityEngine; // For Mathf, if strict pure C# is needed we use System.Math but Unity Mathf is often acceptable for logic if not Monobehaviour. 
// Rules say: "Usecase ... Pure C# Class. NO MonoBehaviour."
// It doesn't explicitly ban UnityEngine.Mathf, but for purity, System.Math is safer. I'll use System.Math.

using System;

namespace KnowEyeDia.Domain.UseCases
{
    public class PlayerSurvivalUseCase
    {
        private readonly PlayerEntity _player;

        public PlayerSurvivalUseCase(PlayerEntity player)
        {
            _player = player;
        }

        public void Tick(float deltaTime)
        {
            // Decrease hunger
            // Example rate: 1 hunger per 10 seconds ?? Let's say 0.1 per second for now.
            float hungerLoss = 0.5f * deltaTime;
            _player.Hunger = Math.Max(0, _player.Hunger - hungerLoss);

            // Health damage if starving
            if (_player.Hunger <= 0)
            {
                float damage = 2f * deltaTime; // 2 health per second
                _player.CurrentHealth = Math.Max(0, _player.CurrentHealth - damage);
            }

            // Temperature logic (Simplified for start)
            // Just clamp for now or drift towards ambient.
        }
    }
}
