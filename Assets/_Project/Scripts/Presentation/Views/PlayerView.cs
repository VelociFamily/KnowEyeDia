using UnityEngine;

namespace KnowEyeDia.Presentation.Views
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _visuals;
        [SerializeField] private Animator _animator;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetVisualRotation(Quaternion rotation)
        {
            // Billboard handles rotation now, but maybe we want to flip X based on movement later?
            // For now, let Billboard handle the main facing.
        }

        public void SetSprite(Sprite sprite)
        {
            if (_visuals != null)
                _visuals.sprite = sprite;
        }

        public void SetInput(Vector2 input)
        {
            if (_animator != null)
            {
                // Only update direction parameters if we are actually moving
                if (input != Vector2.zero)
                {
                    _animator.SetFloat("InputX", input.x);
                    _animator.SetFloat("InputY", input.y);
                }
                
                _animator.SetBool("IsMoving", input != Vector2.zero);
            }
        }
    }
}
