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
        private readonly PlayerView _view;

        [Inject]
        public PlayerPresenter(
            PlayerEntity playerEntity,
            PlayerSurvivalUseCase survivalUseCase,
            IInputService inputService,
            PlayerView view)
        {
            _playerEntity = playerEntity;
            _survivalUseCase = survivalUseCase;
            _inputService = inputService;
            _view = view;
        }

        public void Start()
        {
            // Initialization
            Debug.Log("PlayerPresenter Started");
        }

        public void Tick()
        {
            float dt = Time.deltaTime;

            // 1. Logic Update
            _survivalUseCase.Tick(dt);

            // 2. Input & Movement
            Vector2 input = _inputService.GetMovementInput();
            
            // Simple movement logic here (or move to a MovementUseCase if complex)
            // For now, doing it here to drive the view.
            if (input != Vector2.zero)
            {
                // Move logic
                Vector3 currentPos = _view.transform.position;
                Vector3 move = new Vector3(input.x, 0, input.y) * 5f * dt; // Speed 5
                _view.SetPosition(currentPos + move);
            }

            // Sync other view states (e.g. Health UI) if we had a view for it.
        }
    }
}
