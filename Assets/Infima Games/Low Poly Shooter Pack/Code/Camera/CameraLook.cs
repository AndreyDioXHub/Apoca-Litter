using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _yClamp = new Vector2(-89, 89);
        [SerializeField]
        private bool _smooth = true;
        [SerializeField]
        private float _interpolationSpeed = 45;
        [SerializeField]
        private AimAssyst _aimAssyst;
        [SerializeField]
        private Vector2 _frameInput;

        private Vector2 _sensitivity = new Vector2(1, 1);  
        
        private Character _playerCharacter;
        private Rigidbody _playerCharacterRigidbody;
        private Quaternion _rotationCharacter;
        private Quaternion _rotationCamera;
        
        private void Start()
        {
            float sens = PlayerPrefs.GetFloat(PlayerPrefsConsts.sensitivity, 1);
            UpdateSencivity(sens);

            if(EventsBus.Instance != null)
            {
                EventsBus.Instance?.OnMouseSensitivityChanged.AddListener(UpdateSencivity);
            }

            _playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();   
            _rotationCharacter = _playerCharacter.transform.localRotation;
            _rotationCamera = transform.localRotation;
        }

        public void UpdateSencivity(float sens)
        {
            sens = sens * 6;
            _sensitivity = new Vector2(sens, sens);
        }

        private void LateUpdate()
        {
            _frameInput = _playerCharacter.IsCursorLocked() ? _playerCharacter.GetInputLook() : default;

            _frameInput = new Vector2(_frameInput.x + _aimAssyst.CurrentDirection.x,
                _frameInput.y + 0);// _aimAssyst.CurrentDirection.y);

            _frameInput *= _sensitivity;

            Quaternion rotationYaw = Quaternion.Euler(0.0f, _frameInput.x, 0.0f);
            Quaternion rotationPitch = Quaternion.Euler(-_frameInput.y, 0.0f, 0.0f);
            _rotationCamera *= rotationPitch;
            _rotationCamera = Clamp(_rotationCamera);
            
            _rotationCharacter *= rotationYaw;
            Quaternion localRotation = transform.localRotation;

            if (_smooth)
            {
                localRotation = Quaternion.Slerp(localRotation, _rotationCamera, Time.deltaTime * _interpolationSpeed);
                localRotation = Clamp(localRotation);
                _playerCharacter.transform.rotation = Quaternion.Slerp(_playerCharacter.transform.rotation, _rotationCharacter, Time.deltaTime * _interpolationSpeed);
            }
            else
            {

                localRotation *= rotationPitch;
                localRotation = Clamp(localRotation);
                _playerCharacter.transform.rotation *= rotationYaw;
            }

            transform.localRotation = localRotation;
        }

        private Quaternion Clamp(Quaternion rotation)
        {
            rotation.x /= rotation.w;
            rotation.y /= rotation.w;
            rotation.z /= rotation.w;
            rotation.w = 1.0f;

            float pitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(rotation.x);

            pitch = Mathf.Clamp(pitch, _yClamp.x, _yClamp.y);
            rotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * pitch);

            return rotation;
        }
    }
}