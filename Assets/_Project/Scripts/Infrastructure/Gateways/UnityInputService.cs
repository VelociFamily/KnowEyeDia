using KnowEyeDia.Domain.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KnowEyeDia.Infrastructure.Gateways
{
    public class UnityInputService : IInputService
    {
        public Vector2 GetMovementInput()
        {
            Vector2 move = Vector2.zero;

            // Keyboard
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) move.y += 1;
                if (Keyboard.current.sKey.isPressed) move.y -= 1;
                if (Keyboard.current.aKey.isPressed) move.x -= 1;
                if (Keyboard.current.dKey.isPressed) move.x += 1;
            }

            // Gamepad
            // Add to existing move so we can use both
            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.leftStick.ReadValue();
                if (stick.magnitude > 0.1f) // Deadzone check
                {
                    move += stick;
                }
            }

            return move.normalized;
        }

        public bool IsJumpPressed()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) return true;
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) return true;
            return false;
        }
        
        public bool IsInteractPressed()
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) return true;
            // "South Button" usually A (Xbox) / X (PS). "West" is X (Xbox) / Square (PS).
            // GDD says: "Interact: E / South Button".
            // GDD says: "Use Item: Left Click / West Button".
            // So Interact should be South.
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) return true;
            return false;
        }
    }
}
