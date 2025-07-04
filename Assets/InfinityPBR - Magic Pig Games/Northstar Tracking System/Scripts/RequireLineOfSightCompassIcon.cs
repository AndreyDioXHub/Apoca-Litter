using UnityEngine;
using UnityEngine.UI;

namespace MagicPigGames.Northstar
{
    [System.Serializable]
    [RequireComponent(typeof(CompassIcon))]
    public class RequireLineOfSightCompassIcon : MonoBehaviour
    {
        [Header("Options")] 
        public Transform playerTransform;
        public bool mustBeInCameraView = true;
        public LayerMask layerMask = -1;

        private Transform _playerTransform;
        private Image _image;
        private CompassIcon _compassIcon;
        
        private void Start()
        {
            _playerTransform = playerTransform == null
                ? Camera.main.transform
                : playerTransform;

            _image = GetComponent<Image>();
            _compassIcon = GetComponent<CompassIcon>();
            
            if (_playerTransform != null) return;

            Debug.LogError("No transform was found! Should have found the camera if no player was provided");
            Destroy(this);
        }

        private void LateUpdate() => _image.enabled = HasLineOfSight();

        private bool HasLineOfSight()
        {
            var playerPosition = _playerTransform.position;
            var targetPosition = _compassIcon.target.position;
            var direction = targetPosition - playerPosition;
            var distance = direction.magnitude;

            // First we check if the target is within the NorthstarSystem's detection range
            if (distance > NorthstarSystem.Instance.detectionRange)
                return false;

            if (mustBeInCameraView)
            {
                // Then we check if the target is within the camera's view frustrum
                var planes = GeometryUtility.CalculateFrustumPlanes(_playerTransform.GetComponent<Camera>());
                if (!GeometryUtility.TestPlanesAABB(planes, _compassIcon.target.GetComponent<Collider>().bounds))
                    return false;
            }

            /*
             * NOTE: If the icon is flickering in/out, you may be colliding with your own collider, or the player etc.
             * Put that on it's own layer (often "Player") and set the LayerMask to ignore that layer.
             */
            
            // Cast a ray toward the target, and return true if the first hit is the target
            var ray = new Ray(playerPosition, direction);
            if (Physics.Raycast(ray, out var hit, distance + 1, layerMask))
                return hit.transform == _compassIcon.target;

            // If it didn't hit the target, we return false
            return false;
        }
    }

}
