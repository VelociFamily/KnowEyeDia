using VContainer;
using VContainer.Unity;
using KnowEyeDia.Domain.UseCases;
using KnowEyeDia.Presentation.Views;
using UnityEngine;

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
            // Settings can be moved to a configuration file/ScriptableObject later
            var data = _worldGenUseCase.GenerateWorld(
                width: 30,
                depth: 30,
                seed: Random.Range(0, 1000),
                scale: 5f,
                maxElevation: 3
            );
            
            _view.Render(data);
        }
    }
}
