using InfimaGames.LowPolyShooterPack;
using InfimaGames.LowPolyShooterPack.Legacy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AimAssyst : MonoBehaviour
{

    public Vector2 CurrentDirection => _currentDirection * _power;

    [SerializeField]
    private Character _character;
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private bool _aimAssystIsOn;
    [SerializeField]
    private bool _isNeedAim;

    /*
    [SerializeField] 
    private float _distancPointTransform; 
    [SerializeField] 
    private float _distancBotTransform; 
    [SerializeField] 
    private Vector3 _g; 
    [SerializeField] 
    private Vector3 _b; 
    */

    [SerializeField] 
    private LayerMask _layerMask; // Фильтр по слоям
    [SerializeField] 
    private LayerMask _layerGroundMask; // Фильтр по слоям

    [SerializeField]
    private float _maxDistance = 100f;// Максимальная дистанция луча
    [SerializeField]
    private float _powerMax = 10f;

    private float _dot, _distancePower, _distanceMax = 10;

    private float _power, _powerCurent = 0, _powerMin = 0f;
    private float _timerPowerCur, _timerPowerMax = 1f;

    private Vector2 _currentDirection;
    private Vector2 _velocity;

    private float _horizontalMaxAngle = 180f;
    private float _verticalMaxAngle = 90f;
    private float _smoothTime = 0.2f;

    private Transform _target;
    private Transform _currentTarget;

    private bool _wantChangeTarget = false;
    private bool _wantLoseTarget = false;
    private bool _dirtFound = false;
    /*
    [SerializeField] 
    private float _checkInterval = 0.2f; // Оптимизация проверок

    private float _timer;*/

    private bool _hasDirtObstacle;

    private void Awake()
    {
        //_mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        EventsBus.Instance.OnUseAutoAim.AddListener(SwitchAutoAim);
        EventsBus.Instance.OnUseAimAssist.AddListener(SwitchAimAssist);
    }

    public void SwitchAutoAim(bool isNeenAim)
    {
        _isNeedAim = isNeenAim;
    }

    public void SwitchAimAssist(bool aimAssystIsOn)
    {
        _aimAssystIsOn = aimAssystIsOn;
    }

    private void LateUpdate()
    {
        if (_aimAssystIsOn)
        {
            _wantChangeTarget = false;
            _wantLoseTarget = false;

            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, _maxDistance, _layerMask);

            // Фильтруем попадания по тегам и сортируем по расстоянию
            var validHits = hits
                .Where(h => h.collider.CompareTag("AssystSphere"))
                .OrderBy(h => h.distance)
                .ToArray();


            if (validHits.Length > 0)
            {
                // Берем ближайший объект
                GameObject nearestSphere = validHits[0].collider.gameObject;                

                // Получаем компонент
                Transform newTarget = nearestSphere.transform;

                if (newTarget != null && newTarget != _currentTarget)
                {
                    if (_powerCurent > 0)
                    {
                        if (_currentTarget != null)
                        {
                            _wantChangeTarget = true;
                            ////Debug.Log($"Хотим сменить таргет: {_currentTarget.name}", _currentTarget);
                        }
                    }
                    else
                    {
                        _currentTarget = newTarget;
                        ////Debug.Log($"Новая цель: {_currentTarget.name}", _currentTarget);
                    }
                }

                if (_isNeedAim)
                {
                    RaycastHit hitGround;

                    if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hitGround, _maxDistance, _layerGroundMask))
                    {
                        bool groundCloseThanEnemy = Vector3.Distance(transform.position, hitGround.point) >
                            Vector3.Distance(transform.position, newTarget.position);

                        _character.OnTryAiming(groundCloseThanEnemy);
                    }
                    else
                    {
                        _character.OnTryAiming(validHits.Length > 0);
                    }
                }
            }
            else
            {
                if (_currentTarget != null)
                {
                    if (_powerCurent > 0)
                    {
                        _wantLoseTarget = true;
                        ////Debug.Log($"Хотим потерять таргет", _currentTarget);
                    }
                    else
                    {
                        ////Debug.Log("Цель потеряна");
                        _currentTarget = null;
                    }
                }

                if (_isNeedAim)
                {
                    _character.OnTryAiming(false);
                }
            }

            if (_currentTarget == null)
            {
                //_target = null;
            }
            else
            {
                _target = _currentTarget;
            }

            CheckForDirtObstacle();

            // Визуализация луча
            Debug.DrawRay(ray.origin, ray.direction * _maxDistance,
                _currentTarget != null ? Color.green : Color.red);

            if (_target == null)
            {
            }
            else
            {
                Vector2 rawDirection = GetNormalizedScreenDirection();
                _currentDirection = rawDirection;// Vector2.SmoothDamp(_currentDirection, rawDirection, ref _velocity, _smoothTime);
            }

            _currentDirection = _target == null ? Vector2.zero : _currentDirection;


            if (_wantChangeTarget || _target == null || _wantLoseTarget)
            {
                if (_dirtFound)
                {
                    _timerPowerCur -= Time.deltaTime;
                }

                if (_wantLoseTarget)
                {
                    _timerPowerCur -= Time.deltaTime;
                }
            }
            else
            {
                _timerPowerCur += Time.deltaTime;
            }

            _timerPowerCur = _timerPowerCur > _timerPowerMax ? _timerPowerMax : _timerPowerCur;
            _timerPowerCur = _timerPowerCur < 0 ? 0 : _timerPowerCur;

            /* _timerPowerCur = _target == null ? _timerPowerCur - Time.deltaTime : _timerPowerCur + Time.deltaTime;
            _timerPowerCur = _timerPowerCur > _timerPowerMax ? _timerPowerMax : _timerPowerCur;
            _timerPowerCur = _timerPowerCur < 0 ? 0 : _timerPowerCur;*/

            _powerCurent = Mathf.Lerp(_powerMin, _powerMax, _timerPowerCur / _timerPowerMax);

            if (_powerCurent == 0)
            {
                ////Debug.Log($"Окончательно теряем цель");
                _target = null;
            }

            float distance = _distanceMax;

            if (_target == null)
            {
                _dot = 0;
            }
            else
            {
                if (_target.gameObject.activeSelf)
                {

                    Vector3 _targetDir = (_target.position - _mainCamera.transform.position).normalized;
                    _targetDir.y = 0;
                    Vector3 _cameraDir = _mainCamera.transform.forward;
                    _targetDir.y = 0;

                    _dot = Vector3.Dot(_targetDir, _cameraDir);

                    _dot = _dot < 0 ? 0 : _dot;
                    distance = Vector3.Distance(_target.position, _mainCamera.transform.position);
                }
                else
                {
                    _dot = 0;
                }
            }


            _distancePower = Mathf.Lerp(1, 0, distance / _distanceMax);

            _power = _powerCurent * _dot * _distancePower;
        }
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(_g, new Vector3(0.1f, 0.1f, 0.1f));

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(_b, new Vector3(0.1f, 0.1f, 0.1f));
    }
    */
    private void CheckForDirtObstacle()
    {
        if(_target == null)
        {
            return;
        }

        Vector3 start = transform.position;
        Vector3 end = _target.transform.position;
        Vector3 direction = end - start;
        float distance = Vector3.Distance(start, end);

        RaycastHit[] hits = Physics.RaycastAll(start, direction.normalized, distance, _layerMask);
        _dirtFound = false;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Dirt"))
            {
                _dirtFound = true;

                NotifyObstacleStatus(_dirtFound);


                if (_dirtFound)
                {
                    if (_powerCurent == 0)
                    {
                        //Debug.Log("Цель потеряна за препятствием", _currentTarget);
                        _currentTarget = null;
                    }
                    else
                    {
                        //Debug.Log($"Хотим потерять таргет за препятствием", _currentTarget);
                        _wantChangeTarget = true;
                    }
                }
                break;
            }
        }

        if (_dirtFound != _hasDirtObstacle)
        {
            _hasDirtObstacle = _dirtFound;
        }

        Debug.DrawLine(start, end, _dirtFound ? Color.red : Color.green);
    }

    private void NotifyObstacleStatus(bool status)
    {
        //Debug.Log(status ? "Обнаружено препятствие Dirt!" : "Препятствий нет");
        // Здесь можно добавить вызов события или другую логику реакции
    }

    public Vector2 GetNormalizedScreenDirection()
    {
        Vector3 toTarget = _target.position - _mainCamera.transform.position;
        Vector3 direction = toTarget.normalized;

        // Рассчитываем горизонтальный угол
        float horizontalAngle = Vector3.SignedAngle(
            _mainCamera.transform.forward,
            direction,
            _mainCamera.transform.up
        );

        // Рассчитываем вертикальный угол
        float verticalAngle = Vector3.SignedAngle(
            _mainCamera.transform.forward,
            direction,
            _mainCamera.transform.right
        );

        // Нормализуем в диапазон [-1, 1]
        return new Vector2(
            Mathf.Clamp(horizontalAngle / _horizontalMaxAngle, -1f, 1f),
            Mathf.Clamp(verticalAngle / -_verticalMaxAngle, -1f, 1f)
        );
    }

    // Визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        if (_target == null || _mainCamera == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_mainCamera.transform.position, _target.position);

        // Рисуем оси направления
        Vector3 forward = _mainCamera.transform.forward * 2f;
        Vector3 right = _mainCamera.transform.right * 2f;
        Vector3 up = _mainCamera.transform.up * 2f;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_mainCamera.transform.position, forward);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_mainCamera.transform.position, right);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_mainCamera.transform.position, up);
    }
}
