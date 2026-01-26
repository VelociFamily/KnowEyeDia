using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using System.Collections.Generic;

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
    private string _currentName = "";

    [MenuItem("Tools/Smart Sprite Renamer")]
    public static void ShowWindow()
    {
        GetWindow<SmartSpriteRenamer>("Smart Sprite Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Smart Sprite Renamer", EditorStyles.boldLabel);
        
        _texture = EditorGUILayout.ObjectField("Sprite Sheet", _texture, typeof(Texture2D), false) as Texture2D;
        
        if (_texture == null)
        {
            EditorGUILayout.HelpBox("Please select a sprite sheet texture", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        
        _tileWidth = EditorGUILayout.IntField("Tile Width", _tileWidth);
        _tileHeight = EditorGUILayout.IntField("Tile Height", _tileHeight);
        
        EditorGUILayout.Space();
        GUILayout.Label("Animation Settings", EditorStyles.boldLabel);
        
        _framesPerGroup = EditorGUILayout.IntField("Frames Per Group", _framesPerGroup);
        _totalRowWidth = EditorGUILayout.IntField("Total Row Width", _totalRowWidth);
        _horizontalFrames = EditorGUILayout.Toggle("Horizontal Frames", _horizontalFrames);
        
        int frameStride = _totalRowWidth / _framesPerGroup;
        
        EditorGUILayout.Space();
        GUILayout.Label("Navigation", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        _cursorX = EditorGUILayout.IntField("Cursor X", _cursorX);
        _cursorY = EditorGUILayout.IntField("Cursor Y", _cursorY);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("←")) MoveCursor(-1);
        if (GUILayout.Button("→")) MoveCursor(1);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        GUILayout.Label($"Current Sprite: {GetCurrentSpriteName(frameStride)}", EditorStyles.helpBox);
        
        EditorGUILayout.Space();
        DrawPreview(frameStride);
        
        EditorGUILayout.Space();
        GUILayout.Label("Rename Settings", EditorStyles.boldLabel);
        
        GUI.SetNextControlName("NameField");
        _currentName = EditorGUILayout.TextField("Animation Name", _currentName);
        
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
        
        try
        {
            var factory = new SpriteDataProviderFactories();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            if (dataProvider != null)
            {
                dataProvider.InitSpriteEditorDataProvider();
                var rects = dataProvider.GetSpriteRects();
                
                int totalRows = _texture.height / _tileHeight;
                int actualY = (totalRows - 1) - _cursorY;
                
                Rect targetRect = new Rect(_cursorX * _tileWidth, actualY * _tileHeight, _tileWidth, _tileHeight);
                Vector2 center = targetRect.center;
                
                foreach (var rect in rects)
                {
                    if (rect.rect.Contains(center)) return rect.name;
                }
            }
        }
        catch { }
        
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

        // Prefer Sprite Editor Data Provider API to preserve sprite IDs
        bool usedDataProvider = false;
        try
        {
            var factory = new SpriteDataProviderFactories();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            if (dataProvider != null)
            {
                dataProvider.InitSpriteEditorDataProvider();
                var rects = new List<SpriteRect>(dataProvider.GetSpriteRects());

                int totalRows = _texture.height / _tileHeight;
                bool changed = false;

                for (int i = 0; i < _framesPerGroup; i++)
                {
                    int tX = _cursorX;
                    int tY = _cursorY;
                    if (_horizontalFrames) tX += (i * stride); else tY += (i * stride);
                    int actualY = (totalRows - 1) - tY;

                    Rect targetRect = new Rect(tX * _tileWidth, actualY * _tileHeight, _tileWidth, _tileHeight);
                    Vector2 targetCenter = targetRect.center;

                    for (int k = 0; k < rects.Count; k++)
                    {
                        if (rects[k].rect.Contains(targetCenter))
                        {
                            string oldName = rects[k].name;
                            var rect = rects[k];
                            rect.name = $"{_currentName}_{i}";
                            rects[k] = rect;
                            changed = true;
                            Debug.Log($"[SmartRenamer] Renaming slice at ({tX},{tY}): '{oldName}' -> '{rect.name}'");
                            break;
                        }
                    }
                }

                if (changed)
                {
                    AssetDatabase.StartAssetEditing();
                    dataProvider.SetSpriteRects(rects.ToArray());
                    dataProvider.Apply();
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    Debug.Log($"[SmartRenamer] Successfully saved {path}");
                }
                else
                {
                    Debug.LogWarning("[SmartRenamer] No matching slices were found to rename. Ensure the texture is sliced first!");
                }

                usedDataProvider = true;
            }
        }
        catch { /* fallback below */ }
        finally
        {
            try { AssetDatabase.StopAssetEditing(); } catch { }
        }

        if (!usedDataProvider)
        {
            Debug.LogError("[SmartRenamer] Could not access sprite data provider. Make sure the texture is properly imported as a sprite.");
        }
    }
}
