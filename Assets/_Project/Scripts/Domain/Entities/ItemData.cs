using UnityEngine;

namespace KnowEyeDia.Domain.Entities
{
    // Pure C# class, but using UnityEngine for Sprite reference if needed via wrapper or direct if allowed by rules.
    // User Rules say: "Entity ... Pure C# Class. NO UnityEngine namespace (unless absolutely necessary for types like Vector3)."
    // Tech Specs says: "ItemData: ID, Name, StackSize, SpriteReference."
    // "SpriteReference" usually implies a Sprite. But "No UnityEngine namespace" suggests checking if we can avoid it.
    // However, for pure data, if we need a Sprite, we might need UnityEngine. 
    // BUT Tech Specs "IAssetProvider" suggests we might load assets.
    // "ItemData" often is a ScriptableObject in Unity, but rules say "Pure C# Class".
    // I will stick to Pure C# POCO. SpriteReference might be a string (Addressable Key / Resource Path).
    
    public class ItemData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int MaxStackSize { get; set; }
        public string SpritePath { get; set; } // Using path instead of Sprite object to avoid Unity dependency in Entity layer
    }
}
