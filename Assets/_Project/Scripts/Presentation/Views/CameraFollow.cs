using UnityEngine;

namespace KnowEyeDia.Presentation.Views
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0, 10, -10);
        [SerializeField] private float _smoothSpeed = 0.125f;
        [SerializeField] private bool _useIsometricAngle = true;

        private void LateUpdate()
        {
            if (_target == null) return;

            // Follow Position
            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
            transform.position = smoothedPosition;

            // Isometric Rotation
            if (_useIsometricAngle)
            {
                // Typical Isometric angle: 30 or 45 degrees X, 45 degrees Y if rotated?
                // For "2.5D" usually just looking down is enough.
                // Let's set X rotation to look down at the target.
                transform.LookAt(_target);
            }
        }
    }
}
