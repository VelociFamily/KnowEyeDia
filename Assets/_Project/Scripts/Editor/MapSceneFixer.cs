using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace KnowEyeDia.Editor
{
    [InitializeOnLoad]
    public class MapSceneFixer
    {
        static MapSceneFixer()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (scene.name == "ProceduralScene")
            {
                FixHelper();
            }
        }

        [MenuItem("Tools/Fix Procedural Scene")]
        public static void FixSceneManual()
        {
            FixHelper();
        }

        private static void FixHelper()
        {
            GameObject grid = GameObject.Find("World Grid");
            if (grid == null) return;

            var view = grid.GetComponent<KnowEyeDia.Presentation.Views.WorldView>();
            if (view == null) return;

            SerializedObject so = new SerializedObject(view);
            bool changed = false;

            // Link Tilemaps
            changed |= LinkMap(so, "_snowMap", grid, "Snow");
            changed |= LinkMap(so, "_grassMap", grid, "Grass");
            changed |= LinkMap(so, "_stoneMap", grid, "Stone");
            changed |= LinkMap(so, "_desertMap", grid, "Desert");
            changed |= LinkMap(so, "_dirtMap", grid, "Dirt");
            changed |= LinkMap(so, "_waterMap", grid, "Water");

            // Link Assets
            changed |= LinkAsset(so, "_snowTile", "Snow Tile Rule");
            changed |= LinkAsset(so, "_grassTile", "Grass Rule Tile");
            changed |= LinkAsset(so, "_stoneTile", "Stone Tile Rule");
            changed |= LinkAsset(so, "_desertTile", "Desert Rule Tile");
            changed |= LinkAsset(so, "_dirtTile", "Dirt Rule Tile");
            changed |= LinkAsset(so, "_waterTile", "Water Tile");

            if (changed)
            {
                so.ApplyModifiedProperties();
                Debug.Log("[MapSceneFixer] Procedural Scene references updated.");
                EditorSceneManager.MarkSceneDirty(grid.scene);
            }
        }

        private static bool LinkMap(SerializedObject so, string propName, GameObject root, string childName)
        {
            var prop = so.FindProperty(propName);
            if (prop != null && prop.objectReferenceValue == null)
            {
                var child = root.transform.Find(childName);
                if (child != null)
                {
                    prop.objectReferenceValue = child.GetComponent<Tilemap>();
                    return true;
                }
            }
            return false;
        }

        private static bool LinkAsset(SerializedObject so, string propName, string assetName)
        {
            var prop = so.FindProperty(propName);
            if (prop != null && prop.objectReferenceValue == null)
            {
                string[] guids = AssetDatabase.FindAssets(assetName + " t:TileBase");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    var asset = AssetDatabase.LoadAssetAtPath<TileBase>(path);
                    if (asset != null)
                    {
                        prop.objectReferenceValue = asset;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
