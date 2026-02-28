using VContainer;
using VContainer.Unity;
using KnowEyeDia.Domain.UseCases;
using KnowEyeDia.Presentation.Views;
using UnityEngine;
using System;

namespace KnowEyeDia.Presentation.Presenters
{
    public class WorldPresenter : IStartable
    {
        private readonly WorldGenerationUseCase _worldGenUseCase;
        private readonly WorldView _view;

        [Inject]
        public WorldPresenter(WorldGenerationUseCase worldGenUseCase, WorldView view)
        {
            _worldGenUseCase = worldGenUseCase;
            _view = view;
        }

        public void Start()
        {
            Debug.Log("Generating World...");
            
            // Generate seed from current system time including milliseconds (no symbols)
            // Format: HHmmssSSS (e.g., 1:32:56.123 becomes 13256123)
            string timeString = System.DateTime.Now.ToString("HHmmssfff");
            int seed = int.Parse(timeString);
            
            Debug.Log($"World Seed: {seed} (from time: {System.DateTime.Now:HH:mm:ss.fff})");
            
            // Settings can be moved to a configuration file/ScriptableObject later
            var data = _worldGenUseCase.GenerateWorld(
                width: 700,
                depth: 700,
                seed: seed,
                scale: 0.02f,
                maxElevation: 1 // elevation not strictly used for visuals anymore, but kept for logic
            );
            
            _view.Render(data);
        }
    }
}
