using System;
using UnityEngine; // Vector2 is standard in Unity games even in domain, usually. 
// Rules say: "Entity ... NO UnityEngine namespace (unless absolutely necessary for types like Vector3)."
// UseCase depends on this. I'll use Vector2.

namespace KnowEyeDia.Domain.Interfaces
{
    public interface IInputService
    {
        Vector2 GetMovementInput();
        bool IsJumpPressed();
        bool IsInteractPressed();
    }
}
