using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ProjectSetup
{
   [MenuItem("Tools/Create Clean Architecture Folders")]
    public static void CreateCleanArchitectureFolders()
    {
        // Get the path to the Assets folder
        string assetsPath = Application.dataPath;

        // Define the "Simple Clean Architecture" folder structure
        // This follows the 'Screaming Architecture' pattern where the structure reveals intent
        List<string> folders = new List<string>()
        {
            // Core Project Root (Separates your code from 3rd party assets)
            "_Project",
            
            // 1. App Layer (Composition Root & Configuration)
            "_Project/App",
            "_Project/App/Installers",   // VContainer LifetimeScopes
            "_Project/App/Settings",     // ScriptableObject Configs
            
            // 2. Domain Layer (Pure C# Business Logic - No Unity Dependencies preferred)
            "_Project/Domain",
            "_Project/Domain/Entities",
            "_Project/Domain/ValueObjects",
            "_Project/Domain/UseCases",
            "_Project/Domain/UseCases/Interfaces",
            "_Project/Domain/UseCases/Implementations",
            
            // 3. Infrastructure Layer (Adapters & External Systems)
            "_Project/Infrastructure",
            "_Project/Infrastructure/Gateways", // Implementations of Domain Interfaces
            "_Project/Infrastructure/Services", // Audio, Analytics, etc.
            
            // 4. Presentation Layer (Unity View Logic)
            "_Project/Presentation",
            "_Project/Presentation/Common",
            "_Project/Presentation/Scenes",
            
            // Example Feature Slice: Gameplay
            "_Project/Presentation/Scenes/Gameplay",
            "_Project/Presentation/Scenes/Gameplay/Presenters", // Logic Glue
            "_Project/Presentation/Scenes/Gameplay/Views",      // Monobehaviours/UI
            "_Project/Presentation/Scenes/Gameplay/ViewModels", // MVVM State
            
            // Example Feature Slice: MainMenu
            "_Project/Presentation/Scenes/MainMenu",
            
            // Resources (Use sparingly, prefer Addressables)
            "_Project/Resources",

            // Test Hierarchy (Mirroring the structure)
            "Tests",
            "Tests/Editor",
            "Tests/PlayMode"
        };

        // Iterate and create folders
        foreach (string folder in folders)
        {
            string fullPath = Path.Combine(assetsPath, folder);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                Debug.Log($"Created folder: {folder}");
            }
        }

        // Create a placeholder Bootstrapper script if it doesn't exist
        string bootstrapperPath = Path.Combine(assetsPath, "_Project/App/Bootstrapper.cs");
        if (!File.Exists(bootstrapperPath))
        {
            string bootstrapperContent = 
@"using UnityEngine;
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
}";
            File.WriteAllText(bootstrapperPath, bootstrapperContent);
            Debug.Log("Created file: Bootstrapper.cs");
        }

        // Refresh the AssetDatabase so the folders appear immediately in the Editor
        AssetDatabase.Refresh();
        Debug.Log("<b></b> Clean Architecture structure initialized successfully.");
    }
}