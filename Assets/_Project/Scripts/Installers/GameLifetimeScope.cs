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

        protected override void Configure(IContainerBuilder builder)
        {
            // Register Entities
            builder.Register<PlayerEntity>(Lifetime.Singleton);

            // UseCases
            builder.Register<PlayerSurvivalUseCase>(Lifetime.Singleton);

            // Gateways
            builder.Register<UnityInputService>(Lifetime.Singleton).As<IInputService>();

            // Presenters
            builder.Register<PlayerPresenter>(Lifetime.Singleton).AsImplementedInterfaces();

            // Views
            if (_playerView != null)
            {
                builder.RegisterComponent(_playerView);
            }
            else
            {
                Debug.LogWarning("PlayerView is not assigned in GameLifetimeScope inspector! Please drag the Player object into the slot.");
            }
        }
    }
}
