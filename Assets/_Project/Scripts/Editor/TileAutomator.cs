using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor.U2D.Sprites;
using System.Linq;

public class TileAutomator
{
    [MenuItem("Tools/AutoRenameAndRule")]
    public static void Run()
    {
        // 1. Rename Sprites
        string path = "Assets/_Project/Art/Sprites/Pixel Crawler - Free Pack/Environment/Tilesets/Floors_Tiles.png";
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) { Debug.LogError("Texture not found at " + path); return; }

        // Use ISpriteEditorDataProvider
        var factory = new UnityEditor.U2D.Sprites.SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();
        
        var spriteRects = dataProvider.GetSpriteRects().ToList();
        bool changed = false;

        // Map X,Y indices to Names (Assuming 0,0 is Bottom-Left)
        Dictionary<Vector2Int, string> names = new Dictionary<Vector2Int, string>()
        {
            // Row 0 (Bottom)
            { new Vector2Int(0,0), "Grass_BotLeft" }, { new Vector2Int(1,0), "Grass_Bot" }, { new Vector2Int(2,0), "Grass_BotRight" },
            // Row 1 (Middle)
            { new Vector2Int(0,1), "Grass_Left" },    { new Vector2Int(1,1), "Grass_Center" }, { new Vector2Int(2,1), "Grass_Right" },
            // Row 2 (Top)
            { new Vector2Int(0,2), "Grass_TopLeft" }, { new Vector2Int(1,2), "Grass_Top" }, { new Vector2Int(2,2), "Grass_TopRight" }
        };

        // Helper to find sprite by name later
        Dictionary<string, Sprite> spriteLookup = new Dictionary<string, Sprite>();

        for (int i = 0; i < spriteRects.Count; i++)
        {
             var rect = spriteRects[i];
             string oldName = rect.name;
             // Expected format: Floors_Tiles_0_0
             string[] parts = oldName.Split('_');
             if (parts.Length >= 4) 
             {
                 if (int.TryParse(parts[parts.Length-2], out int x) && int.TryParse(parts[parts.Length-1], out int y))
                 {
                     Vector2Int key = new Vector2Int(x, y);
                     if (names.ContainsKey(key))
                     {
                         rect.name = names[key];
                         spriteRects[i] = rect;
                         changed = true;
                     }
                 }
             }
        }

        if (changed)
        {
            dataProvider.SetSpriteRects(spriteRects.ToArray());
            dataProvider.Apply();
            importer.SaveAndReimport();
            Debug.Log("Renamed Sprites successfully.");
        }

        // reload all sprites at path to get the actual Sprite assets
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach(Object o in assets)
        {
            if (o is Sprite s)
            {
                spriteLookup[s.name] = s;
            }
        }

        // 2. Create Rule Tile
        // We'll try to find the RuleTile type. It is usually just 'RuleTile'.
        RuleTile tile = ScriptableObject.CreateInstance<RuleTile>();
        if (tile == null) { Debug.LogError("Could not create RuleTile instance. Missing package?"); return; }

        tile.m_DefaultSprite = GetSprite(spriteLookup, "Grass_Center");
        tile.m_TilingRules = new List<RuleTile.TilingRule>();

        // Add Rules (Simplified for the 9-slice)
        // 0 = Don't Care, 1 = This, 2 = Not This
        
        // Example: Top Edge (Air on Top)
        // Neighbor order: 0:TopLeft, 1:Top, 2:TopRight, 3:Left, 4:Right, 5:BotLeft, 6:Bot, 7:BotRight
        // (Wait, RuleTile neighbor order varies by implementation, but typically follows standard index or TilingRuleOutput)
        // Actually, TilingRule.m_Neighbors consists of int[8].
        // 0=TopLeft, 1=Top, 2=TopRight, 3=Left, 4=Right, 5=BotLeft, 6=Bot, 7=BotRight
        
        // Define Constants for clarity
        const int Any = 0;
        const int This = 1;
        const int Not = 2;

        AddRule(tile, GetSprite(spriteLookup, "Grass_Top"),      new int[] { Not,Not,Not, This,This, This,This,This });
        AddRule(tile, GetSprite(spriteLookup, "Grass_Bot"),      new int[] { This,This,This, This,This, Not,Not,Not });
        AddRule(tile, GetSprite(spriteLookup, "Grass_Left"),     new int[] { Not,This,This, Not,This, Not,This,This });
        AddRule(tile, GetSprite(spriteLookup, "Grass_Right"),    new int[] { This,This,Not, This,Not, This,This,Not });
        
        AddRule(tile, GetSprite(spriteLookup, "Grass_TopLeft"),  new int[] { Any,Not,Not, Not,This, Not,This,This }); 
        AddRule(tile, GetSprite(spriteLookup, "Grass_TopRight"), new int[] { Not,Not,Any, This,Not, This,This,Not });
        AddRule(tile, GetSprite(spriteLookup, "Grass_BotLeft"),  new int[] { Any,This,This, Not,This, Any,Not,Not }); // Simplified corners
        AddRule(tile, GetSprite(spriteLookup, "Grass_BotRight"), new int[] { This,This,Any, This,Not, Not,Not,Any });

        string tilePath = "Assets/_Project/Art/Sprites/Pixel Crawler - Free Pack/Environment/Tilesets/Auto_Grass.asset";
        AssetDatabase.CreateAsset(tile, tilePath);
        Debug.Log($"Created Rule Tile at {tilePath}");
    }

    static Sprite GetSprite(Dictionary<string, Sprite> lookup, string name)
    {
        if (lookup.TryGetValue(name, out Sprite s)) return s;
        return null;
    }

    static void AddRule(RuleTile tile, Sprite s, int[] neighbors)
    {
        if (s == null) return;
        RuleTile.TilingRule rule = new RuleTile.TilingRule();
        rule.m_Sprites = new Sprite[] { s };
        rule.m_Neighbors = new List<int>(neighbors);
        rule.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Single;
        tile.m_TilingRules.Add(rule);
    }
}
