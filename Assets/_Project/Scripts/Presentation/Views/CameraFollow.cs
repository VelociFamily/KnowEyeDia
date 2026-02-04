using UnityEngine;

namespace KnowEyeDia.Presentation.Views
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0, 10, -10);
        [SerializeField] private float _smoothSpeed = 0.125f;
        
        [Header("Map Bounds")]
        [SerializeField] private bool _enableBounds = true;
        [SerializeField] private Vector2 _minPosition = new Vector2(0, 0);
        [SerializeField] private Vector2 _maxPosition = new Vector2(30, 30);

        private void LateUpdate()
        {
            if (_target == null) return;

            // Follow Position
            Vector3 desiredPosition = _target.position + _offset;

            if (_enableBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, _minPosition.x, _maxPosition.x);
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, _minPosition.y, _maxPosition.y); // Use y of vector2 for z bounds
            }

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
