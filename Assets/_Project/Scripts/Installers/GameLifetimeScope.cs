using VContainer;
using VContainer.Unity;
using UnityEngine;
using KnowEyeDia.Domain.Entities;
using KnowEyeDia.Domain.UseCases;
using KnowEyeDia.Domain.Interfaces;
using KnowEyeDia.Infrastructure.Gateways;
using KnowEyeDia.Presentation.Presenters;
using KnowEyeDia.Presentation.Views;

namespace KnowEyeDia.Installers
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Scene References")]
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private WorldView _worldView;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register Entities
            builder.Register<PlayerEntity>(Lifetime.Singleton);

            // UseCases
            builder.Register<PlayerSurvivalUseCase>(Lifetime.Singleton);
            builder.Register<WorldGenerationUseCase>(Lifetime.Singleton).As<IWorldService>().AsSelf();

            // Gateways
            builder.Register<UnityInputService>(Lifetime.Singleton).As<IInputService>();

            // Presenters
            builder.Register<PlayerPresenter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<WorldPresenter>(Lifetime.Singleton).AsImplementedInterfaces();

            // Views
            if (_playerView != null)
                builder.RegisterComponent(_playerView);
            else
                Debug.LogWarning("PlayerView missing in GameLifetimeScope.");

            if (_worldView != null)
                builder.RegisterComponent(_worldView);
            else
                Debug.LogWarning("WorldView missing in GameLifetimeScope.");
        }
    }
}
