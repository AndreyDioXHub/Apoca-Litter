using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BotNavigation : MonoBehaviour
{
    public UnityEvent OnReachedDestination = new UnityEvent();

    public Vector2 BestDirection => new Vector2(_bestDirection.x, _bestDirection.z);

    public Vector3 Destination => _destination;

    [SerializeField]
    private Transform _destinationTransform;
    private Vector3 _destination;

    [SerializeField]
    private float _updatetame = 0.5f;
    [SerializeField]
    private float _distanceToDestination = 0.6f;

    private Vector3 _bestDirection;
    [SerializeField]
    private bool _isStop;

    private Vector3[] _directionsList = new Vector3[8]
    {
        Vector3.forward,                
        (Vector3.forward + Vector3.left).normalized,  
        Vector3.left,                    
        (Vector3.back + Vector3.left).normalized,  
        Vector3.back,                    
        (Vector3.back + Vector3.right).normalized,  
        Vector3.right,                  
        (Vector3.forward + Vector3.right).normalized 
    };

    private Dictionary<Vector3, int> _directions = new Dictionary<Vector3, int>()
    {
        {Vector3.forward, 0},
        {(Vector3.forward + Vector3.left).normalized, 0},
        {Vector3.left, 0},
        {(Vector3.back + Vector3.left).normalized, 0},
        {Vector3.back, 0},
        {(Vector3.back + Vector3.right).normalized, 0},
        {Vector3.right, 0},
        {(Vector3.forward + Vector3.right).normalized , 0}
    };

    private Dictionary<Vector3, int> _directionsAdded = new Dictionary<Vector3, int>()
    {
        {Vector3.forward, 0},
        {(Vector3.forward + Vector3.left).normalized, 0},
        {Vector3.left, 0},
        {(Vector3.back + Vector3.left).normalized, 0},
        {Vector3.back, 0},
        {(Vector3.back + Vector3.right).normalized, 0},
        {Vector3.right, 0},
        {(Vector3.forward + Vector3.right).normalized , 0}
    };

    private void Start()
    {
        _isStop = true;
        _destination = transform.position;
        PathFifing();
    }
    void Update()
    {
        _destination = _destinationTransform.position;
    }

    private async void PathFifing()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(_updatetame);

            if (_isStop) 
            {
                _bestDirection = Vector3.zero; 
            }
            else
            {
                Vector3 toDestination = (_destination - transform.position).normalized;

                UpdateWeights(toDestination);

                foreach (var dir in _directions)
                {
                    Vector3 worldDir = transform.TransformDirection(dir.Key) * 0.5f;

                    switch (dir.Value)
                    {
                        case 0:
                            Debug.DrawRay(transform.position, worldDir, Color.white, _updatetame);
                            break;
                        case 1:
                            Debug.DrawRay(transform.position, worldDir, Color.green, _updatetame);
                            break;
                        case 2:
                            Debug.DrawRay(transform.position, worldDir, Color.yellow, _updatetame);
                            break;
                        case 3:
                            Debug.DrawRay(transform.position, worldDir, Color.red, _updatetame);
                            break;
                        case 4:
                            Debug.DrawRay(transform.position, worldDir, Color.blue, _updatetame);
                            break;
                        case 5:
                            Debug.DrawRay(transform.position, worldDir, Color.blue, _updatetame);
                            break;
                        case 6:
                            Debug.DrawRay(transform.position, worldDir, Color.black, _updatetame);
                            break;
                    }
                }

            }
        }
    }

    
    public void Stop()
    {
        _isStop = true;
        _destination = transform.position;
    }

    public void SetDestination(Vector3 destination)
    {
        _isStop = false;
        _destination = destination;
    }

    public Vector3 GetBestDirection()
    {
        if (_directions.Count == 0)
        {
            Debug.LogWarning("Directions dictionary is empty!");
            return Vector3.zero;
        }

        int minWeight = _directions.Values.Min();

        List<Vector3> candidates = _directions
            .Where(pair => pair.Value == minWeight)
            .Select(pair => pair.Key)
            .ToList();

        return candidates[Random.Range(0, candidates.Count)];
    }


    void UpdateWeights(Vector3 targetDir)
    {
        for (int i = 0; i < _directionsList.Length; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(_directionsList[i]);
            float angle = Vector3.Angle(worldDirection, targetDir);
            int weight = CalculateWeight(angle);

            switch (i)
            {
                case 0:
                    _directions[Vector3.forward] = weight;
                    break;
                case 1:
                    _directions[(Vector3.forward + Vector3.left).normalized] = weight;
                    break;
                case 2:
                    _directions[Vector3.left] = weight;
                    break;
                case 3:
                    _directions[(Vector3.back + Vector3.left).normalized] = weight;
                    break;
                case 4:
                    _directions[Vector3.back] = weight;
                    break;
                case 5:
                    _directions[(Vector3.back + Vector3.right).normalized] = weight;
                    break;
                case 6:
                    _directions[Vector3.right] = weight;
                    break;
                case 7:
                    _directions[(Vector3.forward + Vector3.right).normalized] = weight;
                    break;
            }
        }
    }

    int CalculateWeight(float angle)
    {
        return angle switch
        {
            <= 22.5f => 1,   // 0° ±22.5°
            <= 67.5f => 2,   // 45° ±22.5°
            <= 112.5f => 3,  // 90° ±22.5°
            <= 157.5f => 4,  // 135° ±22.5°
            _ => 5           // 180° ±22.5°
        };
    }
}