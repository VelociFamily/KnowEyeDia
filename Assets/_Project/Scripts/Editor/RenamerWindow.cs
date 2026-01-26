using UnityEngine;
using UnityEditor;
using System.Linq;

public class RenamerWindow : EditorWindow
{
    private string _baseName = "Water_TopLeft";
    private int _startIndex = 0;

    [MenuItem("Tools/Renamer Tool")]
    public static void ShowWindow()
    {
        GetWindow<RenamerWindow>("Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Renamer", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("1. Select the sprites in the Project Window (e.g. the 4 frames of animation for one corner).\n2. Enter the Base Name below.\n3. Click Rename.", MessageType.Info);

        _baseName = EditorGUILayout.TextField("Base Name", _baseName);
        _startIndex = EditorGUILayout.IntField("Start Index", _startIndex);

        if (GUILayout.Button("Rename Selected"))
        {
            RenameSelected();
        }
    }

    private void RenameSelected()
    {
        Object[] selectedObjects = Selection.objects;
        
        // Sort by name ensures that if they are currently "Sprite_0, Sprite_1" they keep that order
        // OR if the user selection order matters, we might need a different approach.
        // Usually sorting by name is safest IF the original slice order was logical.
        var sortedObjects = selectedObjects.OrderBy(x => x.name).ToArray();

        if (sortedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected!");
            return;
        }

        for (int i = 0; i < sortedObjects.Length; i++)
        {
            Object obj = sortedObjects[i];
            string path = AssetDatabase.GetAssetPath(obj);
            
            // Check if it's a sub-asset (Sprite inside Texture)
            if (AssetDatabase.IsSubAsset(obj))
            {
                obj.name = $"{_baseName}_{_startIndex + i}";
                Debug.Log($"Renamed to {obj.name}");
            }
            else
            {
                // Main asset
                string extension = System.IO.Path.GetExtension(path);
                string newName = $"{_baseName}_{_startIndex + i}";
                AssetDatabase.RenameAsset(path, newName);
            }
        }

        // Force re-import of the parent texture to save sub-asset name changes
        if (sortedObjects.Length > 0 && AssetDatabase.IsSubAsset(sortedObjects[0]))
        {
            string parentPath = AssetDatabase.GetAssetPath(sortedObjects[0]);
            AssetDatabase.ImportAsset(parentPath, ImportAssetOptions.ForceUpdate);
        }

        Debug.Log($"Batch Renamed {sortedObjects.Length} items.");
    }
}
