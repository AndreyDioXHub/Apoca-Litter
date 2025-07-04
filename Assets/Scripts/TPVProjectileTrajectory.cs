using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TPVProjectileTrajectory : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private bool _isDrill;

    private PlayerNetworkResolver _resolver;

    private Vector3 _startPoint;    
    private Vector3 _endPoint; 
    private float _flightTime = 60f;
    private float _elapsedTime = 0f;

    private Color _trajectoryColor = Color.red;
    private float _simulationStep = 0.02f;    

    private Vector3 _gravity = new Vector3(0, -9.81f, 0);
    private Vector3 _initialVelocity;

    private List<Vector3> _points = new List<Vector3>();
    private GameObject _selfGameObject;

    void Start()
    {
    }

    public void Init(Vector3 endPoint, float simulationStep, PlayerNetworkResolver resolver)
    {
        _selfGameObject = gameObject;
        _startPoint = transform.position;
        _endPoint = endPoint;
        _simulationStep =simulationStep;

        _resolver = resolver;
        _resolver.OnBulletCollided.AddListener(BulletCollided);

        if(_isDrill )
        {

        }
        else
        {
            CalculateInitialVelocity();
            CalculateTrajectory();
        }
    }

    public async void BulletCollided()
    {
        if (gameObject.activeSelf)
        {
            if (!_resolver.IsLocalPlayer)
            {
                if (_audioSource == null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    _audioSource.Play();
                    float time = _audioSource.clip.length;
                    await UniTask.WaitForSeconds(time);
                    Destroy(gameObject);
                }
            }
        }
    }

    void FixedUpdate()
    {
        _elapsedTime += _simulationStep;

        if (_elapsedTime > _flightTime)
        {
            Destroy(gameObject);
        }
    }

    void CalculateInitialVelocity()
    {
        // Формула: V0 = (B - A - 0.5 * g * t²) / t
        Vector3 displacement = _endPoint - _startPoint;
        _initialVelocity = (displacement - 0.5f * _gravity * _flightTime * _flightTime) / _flightTime;
    }

    void CalculateTrajectory()
    {
        Vector3 previousPosition = _startPoint;

        _points.Add(_startPoint);

        for (float t = _simulationStep; t <= _flightTime; t += _simulationStep)
        {
            Vector3 currentPosition = CalculatePosition(t);
            _points.Add(currentPosition);
            //Debug.DrawLine(previousPosition, currentPosition, _trajectoryColor);
            previousPosition = currentPosition;
        }

        ProcessingTrajectory();
    }

    public async void ProcessingTrajectory()
    {
        for(int i =0; i < _points.Count-2; i++)
        {
            await UniTask.WaitForSeconds(_simulationStep);

            if (_selfGameObject == null)
            {
                i = _points.Count;
            }
            else
            {
                _selfGameObject.transform.position = _points[i];
            }
        }
    }

    Vector3 CalculatePosition(float t)
    {
        return _startPoint +
               _initialVelocity * t +
               0.5f * _gravity * t * t;
    }
}