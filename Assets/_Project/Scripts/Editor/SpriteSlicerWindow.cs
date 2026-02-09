using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D.Sprites;
using System.Linq;

namespace KnowEyeDia.Editor.Tools
{
    public class SpriteSlicerWindow : EditorWindow
    {
        private Texture2D _selectedTexture;
        private int _sliceWidth = 16;
        private int _sliceHeight = 16;
        private Vector2 _pivot = new Vector2(0.5f, 0.5f);
        private bool _keepEmpty = false;
        private float _alphaTolerance = 0.01f;

        [MenuItem("Tools/Sprite Slicer")]
        public static void ShowWindow()
        {
            GetWindow<SpriteSlicerWindow>("Sprite Slicer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Sprite Slicer Settings", EditorStyles.boldLabel);

            _selectedTexture = (Texture2D)EditorGUILayout.ObjectField("Texture", _selectedTexture, typeof(Texture2D), false);
            _sliceWidth = EditorGUILayout.IntField("Tile Width", _sliceWidth);
            _sliceHeight = EditorGUILayout.IntField("Tile Height", _sliceHeight);
            _pivot = EditorGUILayout.Vector2Field("Pivot", _pivot);
            _keepEmpty = EditorGUILayout.Toggle("Keep Empty Tiles", _keepEmpty);
            _alphaTolerance = EditorGUILayout.Slider("Alpha Tolerance", _alphaTolerance, 0f, 1f);

            if (GUILayout.Button("Slice Sprite"))
            {
                if (_selectedTexture == null)
                {
                    Debug.LogError("No texture selected!");
                    return;
                }
                SliceSprite(_selectedTexture);
            }
        }

        private void SliceSprite(Texture2D texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
            {
                Debug.LogError("Could not get TextureImporter for " + path);
                return;
            }

            importer.isReadable = true;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            
            // Allow alpha transparency and no compression to ensure we read pixels correctly
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteExtrude = 0;
            settings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(settings);
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            // Need to reload the texture to read pixels effectively
            Texture2D readableTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            
            // Use ISpriteEditorDataProvider
            var factory = new UnityEditor.U2D.Sprites.SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            dataProvider.InitSpriteEditorDataProvider();
            
            List<SpriteRect> neMetas = new List<SpriteRect>();
            
            int cols = readableTex.width / _sliceWidth;
            int rows = readableTex.height / _sliceHeight;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    // Remember texture coords start bottom-left
                    // We want to process top-to-bottom usually, but the loop order matters less than rect calculation
                    // row 0 is usually bottom
                    
                    int xPos = x * _sliceWidth;
                    int yPos = y * _sliceHeight; // Bottom-up

                    // Optional: Check for empty pixels
                    if (!_keepEmpty && IsTileEmpty(readableTex, xPos, yPos, _sliceWidth, _sliceHeight))
                    {
                        continue;
                    }

                    SpriteRect meta = new SpriteRect();
                    meta.rect = new Rect(xPos, yPos, _sliceWidth, _sliceHeight);
                    meta.alignment = SpriteAlignment.Custom; // Custom
                    meta.pivot = _pivot;
                    meta.name = $"{texture.name}_{x}_{y}";
                    meta.spriteID = GUID.Generate();
                    neMetas.Add(meta);
                }
            }

            dataProvider.SetSpriteRects(neMetas.ToArray());
            dataProvider.Apply();
            importer.SaveAndReimport();
            Debug.Log($"Sliced {texture.name} into {neMetas.Count} sprites.");
        }

        private bool IsTileEmpty(Texture2D tex, int x, int y, int w, int h)
        {
            Color[] pixels = tex.GetPixels(x, y, w, h);
            foreach (Color p in pixels)
            {
                if (p.a > _alphaTolerance) return false;
            }
            return true;
        }
    }
}
