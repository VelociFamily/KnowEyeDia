using VContainer;
using VContainer.Unity;
using KnowEyeDia.Domain.Entities;
using KnowEyeDia.Domain.UseCases;
using KnowEyeDia.Domain.Interfaces;
using KnowEyeDia.Presentation.Views;
using UnityEngine;

namespace KnowEyeDia.Presentation.Presenters
{
    public class PlayerPresenter : IStartable, ITickable
    {
        private readonly PlayerEntity _playerEntity;
        private readonly PlayerSurvivalUseCase _survivalUseCase;
        private readonly IInputService _inputService;
        private readonly IWorldService _worldService;
        private readonly PlayerView _view;

        [Inject]
        public PlayerPresenter(
            PlayerEntity playerEntity,
            PlayerSurvivalUseCase survivalUseCase,
            IInputService inputService,
            IWorldService worldService,
            PlayerView view)
        {
            _playerEntity = playerEntity;
            _survivalUseCase = survivalUseCase;
            _inputService = inputService;
            _worldService = worldService;
            _view = view;
        }

        public void Start()
        {
            // Initialization
            Debug.Log("PlayerPresenter Started");
            
            // Set initial position on terrain
             Vector3 currentPos = _view.transform.position;
             float height = _worldService.GetHeightAt(currentPos.x, currentPos.z);
             _view.SetPosition(new Vector3(currentPos.x, height, currentPos.z));
        }

        public void Tick()
        {
            float dt = Time.deltaTime;

            // 1. Logic Update
            _survivalUseCase.Tick(dt);

            // 2. Input & Movement
            Vector2 input = _inputService.GetMovementInput();
            
            if (input != Vector2.zero)
            {
                // Move logic
                Vector3 currentPos = _view.transform.position;
                Vector3 move = new Vector3(input.x, 0, input.y) * 5f * dt; // Speed 5
                
                Vector3 targetPos = currentPos + move;
                
                // Adjust Y based on terrain
                float targetHeight = _worldService.GetHeightAt(targetPos.x, targetPos.z);
                
                // Simple snap to height (walk up/down stairs instantly)
                // or we could Lerp for smoothness. Snapping is safer for 2D/2.5D grids.
                targetPos.y = targetHeight;

                _view.SetPosition(targetPos);
            }
        }
    }
}
