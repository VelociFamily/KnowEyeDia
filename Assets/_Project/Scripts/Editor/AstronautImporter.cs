using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace KnowEyeDia.Editor
{
    public class AstronautImporter : EditorWindow
    {
        [MenuItem("Tools/Import Astronaut Sprites")]
        public static void Import()
        {
            string texturePath = "Assets/_Project/Art/Sprites/astronaut_farm_free/astronaut_farm_free/player/character.png";
            string animFolder = "Assets/Art/Animations/Characters/Astronaut";

            if (!Directory.Exists(animFolder)) Directory.CreateDirectory(animFolder);

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(texturePath);
            List<Sprite> sprites = new List<Sprite>();
            foreach (Object asset in assets)
            {
                if (asset is Sprite s) sprites.Add(s);
            }

            if (sprites.Count == 0)
            {
                Debug.LogError($"No sprites found at {texturePath}. Ensure texture is 'Multiple' and Sliced.");
                return;
            }

            // Group by Row (Y position), Top to Bottom
            var rows = sprites.GroupBy(s => Mathf.RoundToInt(s.rect.y))
                              .OrderByDescending(g => g.Key)
                              .ToList();

            Debug.Log($"Found {rows.Count} rows of sprites.");

            List<AnimationClip> clips = new List<AnimationClip>();

            // Helper to create clip with optional FlipX
            void CreateClip(string name, List<Sprite> frameSprites, bool flipX, float sampleRate = 8)
            {
                if (frameSprites == null || frameSprites.Count == 0) return;

                // Sort by X
                frameSprites.Sort((a, b) => a.rect.x.CompareTo(b.rect.x));

                AnimationClip clip = new AnimationClip();
                clip.frameRate = sampleRate;

                // 1. Sprite Animation
                EditorCurveBinding spriteBinding = new EditorCurveBinding();
                spriteBinding.type = typeof(SpriteRenderer);
                spriteBinding.path = "";
                spriteBinding.propertyName = "m_Sprite";

                ObjectReferenceKeyframe[] spriteKeyframes = new ObjectReferenceKeyframe[frameSprites.Count];
                for (int i = 0; i < frameSprites.Count; i++)
                {
                    spriteKeyframes[i] = new ObjectReferenceKeyframe();
                    spriteKeyframes[i].time = i / clip.frameRate;
                    spriteKeyframes[i].value = frameSprites[i];
                }
                AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyframes);

                // 2. FlipX Animation (Critical for mirroring)
                EditorCurveBinding flipBinding = new EditorCurveBinding();
                flipBinding.type = typeof(SpriteRenderer);
                flipBinding.path = "";
                flipBinding.propertyName = "m_FlipX";

                Keyframe[] flipKeyframes = new Keyframe[2]; // Start and End to ensure constant value
                float flipValue = flipX ? 1f : 0f;
                // Constant curve
                flipKeyframes[0] = new Keyframe(0, flipValue);
                flipKeyframes[1] = new Keyframe(frameSprites.Count / clip.frameRate, flipValue);
                
                AnimationCurve flipCurve = new AnimationCurve(flipKeyframes);
                // Make it constant steps (optional, but good for boolean)
                for(int i=0; i<flipCurve.keys.Length; i++) { AnimationUtility.SetKeyLeftTangentMode(flipCurve, i, AnimationUtility.TangentMode.Constant); }
                
                clip.SetCurve("", typeof(SpriteRenderer), "m_FlipX", flipCurve);

                // Loop Settings
                var settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, settings);

                string clipPath = $"{animFolder}/Astronaut_{name}.anim";
                AssetDatabase.CreateAsset(clip, clipPath);
                clips.Add(clip);
                Debug.Log($"Created Clip: {name} (FlipX: {flipX})");
            }

            // --- ANIMATION MAPPING ---
            // Based on observed layout:
            // Row 0 (Top): Idles/Special
            // Row 1: Walk Down
            // Row 2: Walk Up
            // Row 3: Walk Side (Right)
            
            if (rows.Count >= 4)
            {
                var rowIdle = rows[0].ToList();
                var rowDown = rows[1].ToList();
                var rowUp   = rows[2].ToList();
                var rowSide = rows[3].ToList(); 
                
                // Sort them specifically by X to correct any loading order issues
                frameSpritesSort(rowIdle);
                frameSpritesSort(rowDown);
                frameSpritesSort(rowUp);
                frameSpritesSort(rowSide);

                // Create Walks
                CreateClip("Walk_S", rowDown, false);
                CreateClip("Walk_N", rowUp, false);
                CreateClip("Walk_E", rowSide, false); // Right
                CreateClip("Walk_W", rowSide, true);  // Left (Mirrored Right)

                // Create Idles
                // Use first frame of respective walk rows for "Stat" look, 
                // OR use Row 0 if we can identify them. 
                // Let's stick to using the first frame of the walk cycle for perfect matching 
                // UNLESS user specifically wanted the top row.
                // Reverting to previous logic: Use Row 0 for Idles if possible.
                // Row 0 has 6 sprites. Let's assume indices 0-3 map to Down, Up, Right, Left? 
                // Actually, often it's Down, Up, Side.
                
                // Let's safely grab idles from the Walk rows to match the walks exactly.
                Sprite idleS = rowDown[0];
                Sprite idleN = rowUp[0];
                Sprite idleE = rowSide[0];
                Sprite idleW = rowSide[0]; // Mirror this

                CreateClip("Idle_S", new List<Sprite> { idleS }, false);
                CreateClip("Idle_N", new List<Sprite> { idleN }, false);
                CreateClip("Idle_E", new List<Sprite> { idleE }, false);
                CreateClip("Idle_W", new List<Sprite> { idleW }, true); // Mirror
            }
            else
            {
                Debug.LogError("Sprite Sheet did not have expected rows. Found " + rows.Count);
            }

            AssetDatabase.SaveAssets();

            // 4. Create/Recreate Animator Controller
            CreateController(animFolder, clips);
        }

        private static void frameSpritesSort(List<Sprite> list)
        {
             list.Sort((a, b) => a.rect.x.CompareTo(b.rect.x));
        }

        private static void CreateController(string animFolder, List<AnimationClip> clips)
        {
            string controllerPath = $"{animFolder}/AstronautController.controller";
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            controller.AddParameter("InputX", AnimatorControllerParameterType.Float);
            controller.AddParameter("InputY", AnimatorControllerParameterType.Float);
            controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);

            var rootStateMachine = controller.layers[0].stateMachine;

            // STATE 1: IDLE
            var idleState = rootStateMachine.AddState("Idle");
            var idleTree = new UnityEditor.Animations.BlendTree();
            idleTree.blendType = UnityEditor.Animations.BlendTreeType.SimpleDirectional2D;
            idleTree.blendParameter = "InputX"; 
            idleTree.blendParameterY = "InputY";
            idleTree.name = "IdleTree";
            idleState.motion = idleTree;

            // Helper
            AnimationClip FindClip(string suffix) => clips.Find(c => c.name.EndsWith($"_{suffix}"));
            void Add(UnityEditor.Animations.BlendTree t, AnimationClip c, Vector2 p) { if(c!=null) t.AddChild(c, p); }

            Add(idleTree, FindClip("Idle_N"), new Vector2(0, 1));
            Add(idleTree, FindClip("Idle_S"), new Vector2(0, -1));
            Add(idleTree, FindClip("Idle_E"), new Vector2(1, 0));
            Add(idleTree, FindClip("Idle_W"), new Vector2(-1, 0));
            // Diagonals
            Add(idleTree, FindClip("Idle_N"), new Vector2(1, 1).normalized); 
            Add(idleTree, FindClip("Idle_N"), new Vector2(-1, 1).normalized);
            Add(idleTree, FindClip("Idle_S"), new Vector2(1, -1).normalized);
            Add(idleTree, FindClip("Idle_S"), new Vector2(-1, -1).normalized);
            
            // STATE 2: WALK
            var walkState = rootStateMachine.AddState("Walk");
            var walkTree = new UnityEditor.Animations.BlendTree();
            walkTree.blendType = UnityEditor.Animations.BlendTreeType.SimpleDirectional2D;
            walkTree.blendParameter = "InputX";
            walkTree.blendParameterY = "InputY";
            walkTree.name = "WalkTree";
            walkState.motion = walkTree;

            Add(walkTree, FindClip("Walk_N"), new Vector2(0, 1));
            Add(walkTree, FindClip("Walk_S"), new Vector2(0, -1));
            Add(walkTree, FindClip("Walk_E"), new Vector2(1, 0));
            Add(walkTree, FindClip("Walk_W"), new Vector2(-1, 0));
            // Diagonals
            Add(walkTree, FindClip("Walk_N"), new Vector2(1, 1).normalized); 
            Add(walkTree, FindClip("Walk_N"), new Vector2(-1, 1).normalized); 
            Add(walkTree, FindClip("Walk_S"), new Vector2(1, -1).normalized); 
            Add(walkTree, FindClip("Walk_S"), new Vector2(-1, -1).normalized);

            // TRANSITIONS
            var t1 = idleState.AddTransition(walkState);
            t1.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "IsMoving");
            t1.duration = 0;

            var t2 = walkState.AddTransition(idleState);
            t2.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "IsMoving");
            t2.duration = 0;

            AssetDatabase.AddObjectToAsset(idleTree, controller);
            AssetDatabase.AddObjectToAsset(walkTree, controller);
            AssetDatabase.SaveAssets();

            Debug.Log("Updated Controller with Mirrored West Animations.");
        }

        [MenuItem("Tools/Setup Player Animator")]
        public static void SetupPlayer()
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                var view = GameObject.FindObjectOfType<KnowEyeDia.Presentation.Views.PlayerView>();
                if (view != null) player = view.gameObject;
                else { Debug.LogError("Player not found"); return; }
            }

            Animator animator = player.GetComponent<Animator>();
            if (animator == null) animator = player.AddComponent<Animator>();

            string controllerPath = "Assets/Art/Animations/Characters/Astronaut/AstronautController.controller";
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
            if (controller != null) animator.runtimeAnimatorController = controller;

             var playerView = player.GetComponent<KnowEyeDia.Presentation.Views.PlayerView>();
            if (playerView != null)
            {
                SerializedObject serializedView = new SerializedObject(playerView);
                SerializedProperty animProp = serializedView.FindProperty("_animator");
                if (animProp != null)
                {
                    animProp.objectReferenceValue = animator;
                    serializedView.ApplyModifiedProperties();
                }
            }
            Debug.Log("Player Setup Complete.");
        }
    }
}
