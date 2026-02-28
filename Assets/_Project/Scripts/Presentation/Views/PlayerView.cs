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

        [SerializeField] private string _sortingLayerName = "Ground";
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void LateUpdate()
        {
            UpdateSorting();
        }

        private void UpdateSorting()
        {
            if (_visuals != null)
            {
                _visuals.sortingLayerName = _sortingLayerName;
                
                // Use collider bottom (feet) for sorting if available, otherwise pivot
                float yPos = _collider != null ? _collider.bounds.min.y : transform.position.y;
                
                int sortOrder = Mathf.RoundToInt(-yPos * 100);
                _visuals.sortingOrder = sortOrder;
            }
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
