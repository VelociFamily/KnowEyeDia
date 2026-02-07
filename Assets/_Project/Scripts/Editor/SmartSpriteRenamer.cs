using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using System.Collections.Generic;
using System.Linq;

public class SmartSpriteRenamer : EditorWindow
{
    private Texture2D _texture;
    private int _tileWidth = 16;
    private int _tileHeight = 16;
    
    // Animation Settings
    private int _framesPerGroup = 4;
    private int _totalRowWidth = 20; // Used to calc stride
    private bool _horizontalFrames = true;

    // Navigation
    private int _cursorX = 0; 
    private int _cursorY = 0; 
    
    // Naming
    private string _currentName = "Block_Name";

    [MenuItem("Tools/Smart Sprite Renamer")]
    public static void ShowWindow()
    {
        GetWindow<SmartSpriteRenamer>("Smart Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Smart Animated Renamer", EditorStyles.boldLabel);

        // 1. Setup
        GUILayout.BeginVertical("box");
        ConfirmTexture(); // Helper to ensure we have texture data
        _texture = (Texture2D)EditorGUILayout.ObjectField("Texture", _texture, typeof(Texture2D), false);
        
        GUILayout.BeginHorizontal();
        _tileWidth = EditorGUILayout.IntField("Tile W", _tileWidth);
        _tileHeight = EditorGUILayout.IntField("Tile H", _tileHeight);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        _framesPerGroup = Mathf.Max(1, EditorGUILayout.IntField("Frames", _framesPerGroup));
        _totalRowWidth = Mathf.Max(_framesPerGroup, EditorGUILayout.IntField("Total Row Width (Tiles)", _totalRowWidth));
        GUILayout.EndHorizontal();
        _horizontalFrames = EditorGUILayout.Toggle("Horizontal?", _horizontalFrames);
        GUILayout.EndVertical();

        if (_texture == null) return;

        int frameStride = _totalRowWidth / _framesPerGroup;
        GUILayout.Label($"Calculated Stride: Every {frameStride} tiles", EditorStyles.miniLabel);

        // 2. Navigation Control
        GUILayout.Space(10);
        GUILayout.Label($"Current Cursor: ({_cursorX}, {_cursorY})", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<< Prev Tile")) MoveCursor(-1);
        if (GUILayout.Button("Next Tile >>")) MoveCursor(1);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        // FLIPPED LOGIC: Y=0 is TOP. So "Row Down" means INCREASING Y.
        if (GUILayout.Button("Row Up")) { _cursorY--; _cursorX = 0; SanitizeCursor(); }
        if (GUILayout.Button("Row Down")) { _cursorY++; _cursorX = 0; SanitizeCursor(); }
        GUILayout.EndHorizontal();

        // 3. Visualization
        DrawPreview(frameStride);

        // 4. Action
        GUILayout.Space(10);
        
        // Find current name for display
        string currentSpriteName = GetCurrentSpriteName(frameStride);
        GUILayout.Label($"Current Tile Name: {currentSpriteName}", EditorStyles.helpBox);

        GUI.SetNextControlName("NameField");
        _currentName = EditorGUILayout.TextField("Name for this Sequence", _currentName);

        // Handle ENTER key
        Event e = Event.current;
        bool pressedEnter = e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter);
        
        if (GUILayout.Button("Rename & Next Tile", GUILayout.Height(40)) || (pressedEnter && GUI.GetNameOfFocusedControl() == "NameField"))
        {
            RenameSequence(frameStride);
            MoveCursor(1);
            
            // Keep focus
            GUI.FocusControl("NameField");
            
            // Consume event to prevent weirdness
            if (pressedEnter) e.Use(); 
        }
    }

    private void MoveCursor(int direction)
    {
        _cursorX += direction;
        SanitizeCursor();
    }
    
    private void SanitizeCursor()
    {
        if (_cursorX < 0) _cursorX = 0;
        if (_cursorY < 0) _cursorY = 0;
        // Could prevent going > max rows, but loose bounds are okay for now
    }

    private void ConfirmTexture()
    {
        // Occasional safety check
    }

    private string GetCurrentSpriteName(int stride)
    {
        string path = AssetDatabase.GetAssetPath(_texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return "-";

        // Try using DataProvider first for accuracy, fallback to importer.spritesheet
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        if (dataProvider != null)
        {
            dataProvider.InitSpriteEditorDataProvider();
            var rects = dataProvider.GetSpriteRects();
            
            int totalRows = _texture.height / _tileHeight;
            int actualY = (totalRows - 1) - _cursorY;
            Rect targetRect = new Rect(_cursorX * _tileWidth, actualY * _tileHeight, _tileWidth, _tileHeight);
            
            foreach (var r in rects)
            {
                if (r.rect.Contains(targetRect.center)) return r.name;
            }
        }
        
        return "(No Slice)";
    }

    private void DrawPreview(int stride)
    {
        if (_texture == null) return;

        // Aspect Ratio Fix
        float cellHeight = 64f;
        float ratio = (float)_tileWidth / _tileHeight;
        float cellWidth = cellHeight * ratio;

        GUILayout.BeginHorizontal("box");
        
        for (int i = 0; i < _framesPerGroup; i++)
        {
            // Calculate Frame Position
            int tX = _cursorX;
            int tY = _cursorY;

            if (_horizontalFrames) tX += (i * stride);
            else tY += (i * stride);

            // FLIP Y logic
            int totalRows = _texture.height / _tileHeight;
            int actualY = (totalRows - 1) - tY;

            // Coords in pixels
            Rect texCoords = new Rect(
                (float)(tX * _tileWidth) / _texture.width,
                (float)(actualY * _tileHeight) / _texture.height,
                (float)_tileWidth / _texture.width,
                (float)_tileHeight / _texture.height
            );

            // Draw Box
            Rect guiRect = GUILayoutUtility.GetRect(cellWidth, cellHeight, GUILayout.Width(cellWidth), GUILayout.Height(cellHeight));
            GUI.DrawTextureWithTexCoords(guiRect, _texture, texCoords);
        }
        GUILayout.EndHorizontal();
        GUILayout.Label($"Previewing {_framesPerGroup} frames (Stride {stride})");
    }

    private void RenameSequence(int stride)
    {
        string path = AssetDatabase.GetAssetPath(_texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
        if (importer == null) 
        {
            Debug.LogError($"Could not find TextureImporter at {path}");
            return;
        }

        // Use ISpriteEditorDataProvider for robust saving that mimics the Sprite Editor Window
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        
        if (dataProvider != null)
        {
            dataProvider.InitSpriteEditorDataProvider();
            var spriteRects = dataProvider.GetSpriteRects().ToList();
            bool changed = false;
            int totalRows = _texture.height / _tileHeight;


            for (int i = 0; i < _framesPerGroup; i++)
            {
                int tX = _cursorX;
                int tY = _cursorY;

                if (_horizontalFrames) tX += (i * stride);
                else tY += (i * stride);

                int actualY = (totalRows - 1) - tY;
                Rect targetRect = new Rect(tX * _tileWidth, actualY * _tileHeight, _tileWidth, _tileHeight);
                Vector2 targetCenter = targetRect.center;

                // Find match in existing rects
                for (int k = 0; k < spriteRects.Count; k++)
                {
                    if (spriteRects[k].rect.Contains(targetCenter))
                    {
                        var rect = spriteRects[k];
                        string oldName = rect.name;
                        rect.name = $"{_currentName}_{i}"; // e.g., Water_0, Water_1
                        spriteRects[k] = rect; // Update struct in list
                        
                        changed = true;

                        Debug.Log($"[SmartRenamer] Renaming slice at ({tX},{tY}): '{oldName}' -> '{rect.name}'");
                        break;
                    }
                }
            }

            if (changed)
            {
                // Apply changes via DataProvider
                dataProvider.SetSpriteRects(spriteRects.ToArray());
                dataProvider.Apply(); // Commits to ITextureDataProvider

                // Save to AssetDatabase
                var assetImporter = dataProvider.targetObject as AssetImporter;
                if (assetImporter != null)
                {
                    assetImporter.SaveAndReimport();
                    Debug.Log($"[SmartRenamer] Successfully SAVED {path} via SpriteDataProvider.");
                }
            }
        }
        else
        {
            Debug.LogError("[SmartRenamer] Could not access sprite data provider. Make sure the texture is properly imported as a sprite.");
        }
    }
}
