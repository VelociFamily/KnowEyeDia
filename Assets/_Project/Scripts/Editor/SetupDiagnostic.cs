using UnityEngine;
using UnityEditor;
using KnowEyeDia.Installers;
using KnowEyeDia.Presentation.Views;

namespace KnowEyeDia.Editor
{
    public static class SetupDiagnostic
    {
        [MenuItem("Tools/Check Player Setup")]
        public static void CheckSetup()
        {
            Debug.Log("--- Starting Player Setup Diagnostic ---");
            
            // 1. Check LifetimeScope
            GameLifetimeScope scope = Object.FindObjectOfType<GameLifetimeScope>();
            if (scope == null)
            {
                Debug.LogError("FAIL: No GameLifetimeScope found in scene!");
                return;
            }
            Debug.Log($"PASS: Found GameLifetimeScope: {scope.name}");

            // 2. Check PlayerView
            PlayerView playerView = Object.FindObjectOfType<PlayerView>();
            if (playerView == null)
            {
                Debug.LogError("FAIL: No PlayerView found in scene!");
                return;
            }
            Debug.Log($"PASS: Found PlayerView on: {playerView.name}");

            // 3. Check Scope References
            SerializedObject serializedScope = new SerializedObject(scope);
            SerializedProperty playerViewProp = serializedScope.FindProperty("_playerView");
            if (playerViewProp.objectReferenceValue == null)
            {
                Debug.LogError("FAIL: PlayerView is NOT assigned in GameLifetimeScope!");
                Debug.Log("Attempting auto-fix...");
                playerViewProp.objectReferenceValue = playerView;
                serializedScope.ApplyModifiedProperties();
                Debug.Log("FIXED: Assigned PlayerView to GameLifetimeScope.");
            }
            else if (playerViewProp.objectReferenceValue != playerView)
            {
                Debug.LogWarning($"WARNING: GameLifetimeScope points to {playerViewProp.objectReferenceValue.name}, but found {playerView.name} in scene. Is it a prefab asset?");
                 // Fix if it's a prefab
                 if (AssetDatabase.Contains(playerViewProp.objectReferenceValue))
                 {
                     Debug.Log("It points to a Prefab Asset. Fixing to Scene Object...");
                     playerViewProp.objectReferenceValue = playerView;
                     serializedScope.ApplyModifiedProperties();
                 }
            }
            else
            {
                Debug.Log("PASS: GameLifetimeScope has correct PlayerView reference.");
            }

            // 4. Check Animator
            Animator animator = playerView.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("FAIL: Player does not have an Animator!");
            }
            else
            {
                if (animator.runtimeAnimatorController == null)
                    Debug.LogError("FAIL: Animator has no Controller assigned!");
                else
                    Debug.Log($"PASS: Animator has controller: {animator.runtimeAnimatorController.name}");
            }
            
            // 5. Check PlayerView Fields
            SerializedObject serializedView = new SerializedObject(playerView);
            SerializedProperty animProp = serializedView.FindProperty("_animator");
            if (animProp.objectReferenceValue == null)
            {
                Debug.LogError("FAIL: PlayerView does not have _animator assigned!");
                if (animator != null)
                {
                    animProp.objectReferenceValue = animator;
                    serializedView.ApplyModifiedProperties();
                    Debug.Log("FIXED: Assigned Animator to PlayerView.");
                }
            }
            else
            {
                Debug.Log("PASS: PlayerView has _animator assigned.");
            }

            Debug.Log("--- Diagnostic Code Finished ---");
        }
    }
}
