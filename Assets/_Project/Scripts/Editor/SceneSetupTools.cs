using UnityEngine;
using UnityEditor;

namespace KnowEyeDia.Editor
{
    public class SceneSetupTools
    {
        [MenuItem("Tools/Setup Ground")]
        public static void SetupGround()
        {
            // 1. Load the Texture
            string texturePath = "Assets/_Project/Art/Sprites/grass_tile_01.png";
            Texture2D grassTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

            if (grassTexture == null)
            {
                Debug.LogError($"Could not find texture at {texturePath}. Please check your files.");
                return;
            }

            // 2. Ensure Material Exists
            string matPath = "Assets/_Project/Art/Materials/Mat_Grass.mat";
            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Art/Materials"))
            {
                AssetDatabase.CreateFolder("Assets/_Project/Art", "Materials");
            }

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                // Try URP Lit first, fall back to Standard if URP not found (safety)
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Simple Lit");
                if (shader == null) shader = Shader.Find("Standard");
                
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, matPath);
            }

            // 3. Configure Material
            mat.mainTexture = grassTexture;
            mat.mainTextureScale = new Vector2(100, 100); // Tiling
            // Ensure Albedo Color is White so it doesn't tint grey
            mat.color = Color.white;
            EditorUtility.SetDirty(mat);

            // 4. Create or Find Ground Plane
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
            }

            // 5. Apply Material and Scale
            ground.transform.localScale = new Vector3(100, 1, 100);
            Renderer renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = mat;
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Ground Setup Complete! You should see green grass now.");
        }
    }
}
