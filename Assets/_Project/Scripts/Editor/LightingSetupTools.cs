using UnityEngine;
using UnityEditor;

namespace KnowEyeDia.Editor
{
    public class LightingSetupTools
    {
        [MenuItem("Tools/Setup Lighting")]
        public static void SetupLighting()
        {
            // Find or Create Directional Light
            Light light = Object.FindFirstObjectByType<Light>();
            if (light == null || light.type != LightType.Directional)
            {
                GameObject lightObj = new GameObject("Directional Light");
                light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                
                // Typical Day Setup
                lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
                light.color = Color.white;
                light.intensity = 1.0f;
                
                Debug.Log("Created Directional Light.");
            }
            else
            {
                Debug.Log("Directional Light already exists.");
            }
        }
    }
}
