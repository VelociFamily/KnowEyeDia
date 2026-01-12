using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App
{
    public class Bootstrapper : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Register your dependencies here
            // builder.Register<MyService>(Lifetime.Singleton);
        }
    }
}