using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.EventSystems;

public class BotFindMoveDirection : MonoBehaviour
{
    public bool HaveDirection;
    public CharacterController Controller => _controller;
    public BotMovingStatus Status => _status;
    public bool IsMayMove => _isMayMove;
    public Transform Target => _navigator.DestinationTransform;
    public Navigator CurrentNavigator => _navigator;

    public bool IsDisable
    {
        get { return _isDisable; }
        set { _isDisable = value; }
    }

    [SerializeField]
    private Navigator _navigator;
    [SerializeField]
    private bool _isDisable;
    [SerializeField]
    private bool _isMayMove;
    [SerializeField]
    private BotMovingStatus _status;
    [SerializeField]
    private float _distanceToNeedFindNewTarget = 0.1f;
    [SerializeField]
    private float _angle = 10f;
    [SerializeField]
    private Vector2 _yClamp = new Vector2(-89, 89);
    [SerializeField]
    private float _interpolationSpeed = 45;
    [SerializeField]
    private bool _smooth = true;
    [SerializeField]
    private float _timeOut = 10, _time;
    [SerializeField]
    private bool _isNeedOrder;
    [SerializeField]
    private float _velocity;

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private bool _closeTheWall;
    private bool _waitingCheckTheWall;
    [SerializeField]
    private float _timeCheckTheWall = 2, _timeCheckTheWallCur;
    private float _distanceToWall = 0.5f;
    [SerializeField]
    private bool forward1, forward2, forward3, forward4;
    [SerializeField]
    private List<HitPointNormal> _contactPoints = new List<HitPointNormal>();

    private float _angleCurent = 10f;
    private Vector2 _currentDirection;
    private Vector2 _frameInput;
    private int _pathIndex = 0;
    private bool _navigatorIsBuisy = false;
    private float _horizontalMaxAngle = 180f;
    private float _verticalMaxAngle = 90f;
    private float _smoothTime = 0.2f;

    private Vector2 _sensitivity = new Vector2(3, 3);
    private Quaternion _rotationCharacter;
    private Quaternion _rotationCamera;

    private List<Vector3> _path = new List<Vector3>();
    private CharacterController _controller;

    private Vector3 _targetPosition;
    private bool _isWorldCreating;


    void Start()
    {
        _controller = GetComponent<CharacterController>();
        StartFinding();
        //GameWorld.Instance.OnWorldCreated.AddListener(WaitForCommand);
        _status = BotMovingStatus.idle;
        _navigator.OnBotCanMove.AddListener(()=> _isMayMove = true);
        _navigator.OnDisableAllBots.AddListener(()=> _isMayMove = false);

        SlowUpdate();
    }

    public async void WaitForCommand()
    {
        while (!_isMayMove)
        {
            await UniTask.Yield();
        }

        while (_isDisable)
        {
            await UniTask.Yield();
        }

        _isNeedOrder = false;
        StartFinding();
    }

    [ContextMenu("Start Finding")]
    public void StartFinding()
    {
        _isWorldCreating = true;
        _rotationCharacter = transform.localRotation;

        if(_navigator.DestinationTransform == null)
        {
            _path.Clear();
            _path = new List<Vector3>();
            _targetPosition = transform.position;
            HaveDirection = false;
        }
        else
        {
            if (Vector3.Distance(transform.position, _targetPosition) > 5)
            {
                _path.Clear();
                _path = new List<Vector3>();
                _targetPosition = _navigator.DestinationTransform.position;
                HaveDirection = false;
            }
            else
            {
                _navigator.AddBotToQueue(this, out _navigatorIsBuisy);
            }
        }
    }

    public void UnBun()
    {
        if (_isDisable)
        {

        }
        else
        {
            if (_isMayMove)
            {
                _isNeedOrder = false;
            }
            else
            {
                WaitForCommand();
            }
        }
    }

    public async void BreakingTheWall()
    {
        _status = BotMovingStatus.idle;

        foreach (var hit in _contactPoints)
        {
            GameWorld.Instance.IntentionСhangeBlock(hit.point, hit.normal, 50);
        }

        _closeTheWall = false;
        forward1 = false;
        forward2 = false;
        forward3 = false;
        forward4 = false;

        _contactPoints.Clear();
        _contactPoints = new List<HitPointNormal>();
        _waitingCheckTheWall = true;
        await UniTask.WaitForSeconds(1);
        _isNeedOrder = false;
    }

    public void FindingEnd(List<Vector3> pathForAdding)
    {

        _path.Clear();
        _path = new List<Vector3>();

        foreach (var pathItem in pathForAdding)
        {
            _path.Add(pathItem);
        }

        _pathIndex = 0;
        _navigatorIsBuisy = false;

        if (_path.Count == 1)
        {
            _path.Clear();
            _path = new List<Vector3>();
            _status = BotMovingStatus.stopped;
            HaveDirection = false;
            _isNeedOrder = true;
            //AfterFall(10);
        }

        //HaveDirection = true;
    }

    public async void AfterFall(float timeout)
    {
        await UniTask.WaitForSeconds(timeout);
        /*
        while (_navigatorIsBuisy)
        {
            Debug.Log("Navigator Is Buisy");
            await UniTask.Yield();
        }*/

        _path.Clear();
        _path = new List<Vector3>();
        _status = BotMovingStatus.crashed;
        HaveDirection = false;
        _isNeedOrder = true;
        //StartFinding();
    }

    public async void Stop(float timeout)
    {
        await UniTask.WaitForSeconds(timeout);
        _path.Clear();
        _path = new List<Vector3>();
        _status = BotMovingStatus.stopped;
        HaveDirection = false;
        _isNeedOrder = true;
    }
    public async void Stuck(float timeout)
    {
        await UniTask.WaitForSeconds(timeout);
        _path.Clear();
        _path = new List<Vector3>();
        _status = BotMovingStatus.stuck;
        HaveDirection = false;
        _isNeedOrder = true;
    }

    public async void Idled(float timeout)
    {
        await UniTask.WaitForSeconds(timeout);

        _path.Clear();
        _path = new List<Vector3>();
        _status = BotMovingStatus.idle;
        HaveDirection = false;
        _isNeedOrder = true;
        //StartFinding();
    }

    public async void SlowUpdate()
    {
        float tick = 0.25f;

        while (true)
        {
            await UniTask.WaitForSeconds(tick);

            if (_isDisable)
            {
            }
            else
            {

                if (_navigator.DestinationTransform != null)
                {
                    if (_isNeedOrder)
                    {

                    }
                    else
                    {
                        if (_isWorldCreating)
                        {
                            _time += tick;

                            if (_time > _timeOut)
                            {
                                if (_status == BotMovingStatus.idle)
                                {
                                    StartFinding();
                                }
                                _time = 0;
                            }

                            if (Vector3.Distance(transform.position, _targetPosition) > 5)
                            {
                                //Debug.Log("Far avay");
                                _targetPosition = _navigator.DestinationTransform.position;
                            }
                            else
                            {
                                //Debug.Log("Spam");

                                for (int i = 0; i < _path.Count - 1; i++)
                                {
                                    Debug.DrawLine(_path[i], _path[i + 1], Color.green);
                                }

                                if (_path.Count == 0)
                                {
                                    _targetPosition = _navigator.DestinationTransform.position;

                                    if (_navigatorIsBuisy)
                                    {

                                    }
                                    else
                                    {
                                        StartFinding();
                                    }
                                }
                                else
                                {
                                    if (_pathIndex + 1 < _path.Count)
                                    {
                                        _targetPosition = _path[_pathIndex + 1];

                                        if (Vector3.Distance(transform.position, _targetPosition) < _distanceToNeedFindNewTarget)
                                        {
                                            _pathIndex++;

                                            if (_navigatorIsBuisy)
                                            {

                                            }
                                            else
                                            {
                                                StartFinding();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _path.Clear();
                                        _path = new List<Vector3>();
                                        _status = BotMovingStatus.stopped;
                                        _isNeedOrder = true;
                                        HaveDirection = false;
                                    }
                                }
                            }

                            if (_isNeedOrder)
                            {

                            }
                            else
                            {
                                Vector3 flatToTarget = (_targetPosition - transform.position).normalized;
                                flatToTarget.y = 0;

                                _angleCurent = Vector3.Angle(transform.forward, flatToTarget);
                                HaveDirection = _angleCurent < _angle;
                            }
                        }
                    }


                    _contactPoints.Clear();
                    _contactPoints = new List<HitPointNormal>();

                    if (_waitingCheckTheWall)
                    {
                        _closeTheWall = false;
                        forward1 = false;
                        forward2 = false;
                        forward3 = false;
                        forward4 = false;

                        _timeCheckTheWallCur += tick;

                        if (_timeCheckTheWallCur > _timeCheckTheWall)
                        {
                            _timeCheckTheWallCur = 0;
                            _waitingCheckTheWall = false;
                        }
                    }
                    else
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(transform.position + Vector3.up * 0.75f, transform.forward, out hit, _distanceToWall, _layerMask))
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 0.75f, transform.forward * hit.distance, Color.yellow);
                            //Debug.Log("Did Hit");
                            forward1 = true;
                            _contactPoints.Add(new HitPointNormal(hit.point, hit.normal));
                        }
                        else
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 0.75f, transform.forward * _distanceToWall, Color.white);
                            //Debug.Log("Did not Hit");
                            forward1 = false;
                        }

                        if (Physics.Raycast(transform.position + Vector3.up * 1.25f, transform.forward, out hit, _distanceToWall, _layerMask))
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 1.25f, transform.forward * hit.distance, Color.yellow);
                            //Debug.Log("Did Hit");
                            forward2 = true;
                            _contactPoints.Add(new HitPointNormal(hit.point, hit.normal));
                        }
                        else
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 1.25f, transform.forward * _distanceToWall, Color.white);
                            //Debug.Log("Did not Hit");
                            forward2 = false;
                        }

                        if (Physics.Raycast(transform.position + Vector3.up * 1.75f, transform.forward, out hit, _distanceToWall, _layerMask))
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 1.75f, transform.forward * hit.distance, Color.yellow);
                            //Debug.Log("Did Hit");
                            forward3 = true;
                            _contactPoints.Add(new HitPointNormal(hit.point, hit.normal));
                        }
                        else
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 1.75f, transform.forward * _distanceToWall, Color.white);
                            //Debug.Log("Did not Hit");
                            forward3 = false;
                        }

                        if (Physics.Raycast(transform.position + Vector3.up * 0.25f, transform.forward, out hit, _distanceToWall, _layerMask))
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 0.25f, transform.forward * hit.distance, Color.yellow);
                            //Debug.Log("Did Hit");
                            forward4 = true;
                            _contactPoints.Add(new HitPointNormal(hit.point, hit.normal));
                        }
                        else
                        {
                            Debug.DrawRay(transform.position + Vector3.up * 0.25f, transform.forward * _distanceToWall, Color.white);
                            //Debug.Log("Did not Hit");
                            forward4 = false;
                        }

                        if (!_closeTheWall && (forward1 || forward2 || forward3 || forward4))
                        {
                            Debug.Log("OnWallDetected");
                        }

                        _closeTheWall = forward1 || forward2 || forward3 || forward4;

                        if (_closeTheWall)
                        {
                            HaveDirection = false;
                            _status = BotMovingStatus.stuck;
                        }
                    }

                }
                else
                {
                    _path.Clear();
                    _path = new List<Vector3>();

                    try
                    {
                        _targetPosition = transform.position;
                    }
                    catch(MissingReferenceException e)
                    {

                    }

                    HaveDirection = false;
                }
            }
        }
    }

    void Update()
    {
        if (_isDisable)
        {
        }
        else
        {

            if (_navigator.DestinationTransform != null)
            {
                if (_isNeedOrder)
                {

                }
                else
                {
                    Rotating();
                }
            }
        }
    }



    public void VelocityChanged(float velocity)
    {
        _velocity = velocity;

        if (_isNeedOrder)
        {
        }
        else
        {
            if (_closeTheWall)
            {
                Stuck(Time.fixedDeltaTime);
            }
            else
            {
                if (Mathf.Abs(velocity) > 0.7f)
                {
                    _status = BotMovingStatus.move;
                }
                else
                {
                    _status = BotMovingStatus.idle;
                }
            }
        }
    }

    public void Rotating()
    {
        Vector2 rawDirection = Vector2.zero;
        rawDirection = GetNormalizedScreenDirection();
        
        /*if (_path.Count > 0)
        {
        }*/

        _currentDirection = rawDirection;// Vector2.SmoothDamp(_currentDirection, rawDirection, ref _velocity, _smoothTime);

        _frameInput = new Vector2(_currentDirection.x, 0);

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
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotationCharacter, Time.deltaTime * _interpolationSpeed);
        }
        else
        {

            localRotation *= rotationPitch;
            localRotation = Clamp(localRotation);
            transform.rotation *= rotationYaw;
        }

        if (localRotation.IsNaN())
        {

        }
        else
        {
            transform.localRotation = localRotation;
        }
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

    public Vector2 GetNormalizedScreenDirection()
    {
        Vector3 toTarget = _targetPosition - transform.position;
        Vector3 direction = toTarget.normalized;

        // Рассчитываем горизонтальный угол
        float horizontalAngle = Vector3.SignedAngle(
            transform.forward,
            direction,
            transform.up
        );

        // Рассчитываем вертикальный угол
        float verticalAngle = Vector3.SignedAngle(
            transform.forward,
            direction,
            transform.right
        );

        // Нормализуем в диапазон [-1, 1]
        return new Vector2(
            Mathf.Clamp(horizontalAngle / _horizontalMaxAngle, -1f, 1f),
            Mathf.Clamp(verticalAngle / -_verticalMaxAngle, -1f, 1f)
        );
    }

}

[Serializable]
public class HitPointNormal
{
    public Vector3 point;
    public Vector3 normal;
    public HitPointNormal(Vector3 point, Vector3 normal)
    {
        this.point = point;
        this.normal = normal;
    }
}

public static class QuaternionExtensions
{
    // Проверка кватерниона на наличие NaN
    public static bool IsNaN(this Quaternion q)
    {
        return float.IsNaN(q.x) ||
               float.IsNaN(q.y) ||
               float.IsNaN(q.z) ||
               float.IsNaN(q.w);
    }
}
