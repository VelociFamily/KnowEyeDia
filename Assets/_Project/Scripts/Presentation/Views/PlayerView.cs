using UnityEngine;

namespace KnowEyeDia.Presentation.Views
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _visuals;

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
    }
}
