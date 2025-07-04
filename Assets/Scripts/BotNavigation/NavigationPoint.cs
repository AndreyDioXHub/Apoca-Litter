using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPoint : MonoBehaviour
{
    public Vector2 BestDirection => new Vector2(_bestDirection.x, _bestDirection.z);

    public Vector3 Destination => _destination;
    public GameObject NextPoint => _nextPoint;
    public int Weight => _weight;
    public int Iteration => _iteration;

    [SerializeField]
    private Transform _destinationTransform;
    [SerializeField]
    private GameObject _nextPoint;
    [SerializeField]
    private GameObject _previosPoint;
    [SerializeField]
    private int _iteration, _iterationMax = 5;

    private Vector3 _destination;

    [SerializeField]
    private float _updatetame = 0.5f;
    [SerializeField]
    private float _distanceToDestination = 4;

    [SerializeField]
    private Vector3 _prevPosition;

    private Vector3 _bestDirection;

    [SerializeField]
    private int _weight;
    [SerializeField]
    private Navigator _navigator;
    private bool _drawPath; 
    public bool IsBuisy; 

    private Vector3[] _directionsList = new Vector3[8]
    {
        Vector3.forward,
        (Vector3.forward + Vector3.left),
        Vector3.left,
        (Vector3.back + Vector3.left),
        Vector3.back,
        (Vector3.back + Vector3.right),
        Vector3.right,
        (Vector3.forward + Vector3.right)
    };

    private Dictionary<Vector3, int> _directions = new Dictionary<Vector3, int>()
    {
        {Vector3.forward, 0},
        {(Vector3.forward + Vector3.left), 0},
        {Vector3.left, 0},
        {(Vector3.back + Vector3.left), 0},
        {Vector3.back, 0},
        {(Vector3.back + Vector3.right), 0},
        {Vector3.right, 0},
        {(Vector3.forward + Vector3.right) , 0}
    };

    private Dictionary<Vector3, int> _directionsAdded = new Dictionary<Vector3, int>()
    {
        {Vector3.forward, 0},
        {(Vector3.forward + Vector3.left), 0},
        {Vector3.left, 0},
        {(Vector3.back + Vector3.left), 0},
        {Vector3.back, 0},
        {(Vector3.back + Vector3.right), 0},
        {Vector3.right, 0},
        {(Vector3.forward + Vector3.right) , 0}
    };

    private void Start()
    {
    }

    public void DrawPath(GameObject nextPoint)
    {
        _nextPoint = nextPoint;
        _drawPath = true;
        if(_previosPoint != null)
        {
            _previosPoint.GetComponent<NavigationPoint>().DrawPath(gameObject);
        }
    }


    void Update()
    {
        /*
        if(_drawPath)
        {
            if (_previosPoint != null)
            {
                Debug.DrawLine(transform.position, _previosPoint.transform.position, Color.green);
            }
        }*/


        //_floor = Vector3Int.FloorToInt(transform.position);
        //
        /*
        foreach (var dir in _directions)
        {
            Vector3 worldDir = transform.TransformDirection(dir.Key) * 0.5f;

            switch (dir.Value)
            {
                case 0:
                    Debug.DrawRay(transform.position, worldDir, Color.white);
                    break;
                case 1:
                    Debug.DrawRay(transform.position, worldDir, Color.green);
                    break;
                case 2:
                    Debug.DrawRay(transform.position, worldDir, Color.yellow);
                    break;
                case 3:
                    Debug.DrawRay(transform.position, worldDir, Color.red);
                    break;
                case 4:
                    Debug.DrawRay(transform.position, worldDir, Color.blue);
                    break;
                case 5:
                    Debug.DrawRay(transform.position, worldDir, Color.blue);
                    break;
                case 6:
                    Debug.DrawRay(transform.position, worldDir, Color.black);
                    break;
            }
        }*/
    }

    public void ClearPoint()
    {
        _destinationTransform = null;
        _destination = Vector3.zero;
        _iteration = 0;
        _prevPosition = Vector3.zero; 
        _weight = 0;
        _navigator = null;

        _nextPoint = null;
        _previosPoint = null;

        //_destination = Vector3.zero;
        _destination = Vector3.zero;

        _navigator = null;

        IsBuisy = true;

    }

    public void SetDestenation(Transform destinationTransform, int iteration, 
        Vector3 prevPosition, int weight, Navigator navigator, GameObject previosPoint, int iterationMax)
    {
        _destinationTransform = destinationTransform;
        _destination = _destinationTransform.position;
        _iteration = iteration;
        _prevPosition = prevPosition;
        _weight = weight;
        _navigator = navigator;
        _previosPoint = previosPoint;
        //_destination = transform.position;
        _destination = _destinationTransform.position;

        _navigator.OnFindingEnd.AddListener(FindingEnd);

        IsBuisy = true;

        _iterationMax = iterationMax;

        gameObject.SetActive(true);

    }

    private void OnEnable()
    {
        PathFifing();
    }

    private void FindingEnd()
    {        
        if (!_drawPath)
        {
            gameObject.SetActive(false);
            IsBuisy = false;
            //_navigator.OnFindingEnd.RemoveListener(FindingEnd);
            //Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _navigator.OnFindingEnd.RemoveListener(FindingEnd);
    }

    private void OnDisable()
    {
        _navigator.OnFindingEnd.RemoveListener(FindingEnd);
    }

    public void DestroyNavigationPoint()
    {
        //Destroy(gameObject);

        gameObject.SetActive(false);
        IsBuisy = false; 
        _drawPath = false;
    }

    private async void PathFifing()
    {
        await UniTask.Yield();
        try
        {
            Vector3 toDestination = (_destination - transform.position).normalized;

            UpdateWeights(toDestination);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        
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

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    public async void SpawnNextPoints()
    {
        if (_directions.Count == 0)
        {
            Debug.LogWarning("Directions dictionary is empty!");
        }

        int minWeight = _directions.Values.Min();

        List<Vector3> candidates = new List<Vector3>();
        /*
            _directions
            .Where(pair => pair.Value == minWeight)
            .Select(pair => pair.Key)
            .ToList();*/

        foreach (var direction in _directions)
        {
            candidates.Add(direction.Key);
            /*if (direction.Value < 6)
            {
            }*/
        }

        foreach (Vector3 candidat in candidates)
        {
        }
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
                    _directions[(Vector3.forward + Vector3.left)] = weight;
                    break;
                case 2:
                    _directions[Vector3.left] = weight;
                    break;
                case 3:
                    _directions[(Vector3.back + Vector3.left)] = weight;
                    break;
                case 4:
                    _directions[Vector3.back] = weight;
                    break;
                case 5:
                    _directions[(Vector3.back + Vector3.right)] = weight;
                    break;
                case 6:
                    _directions[Vector3.right] = weight;
                    break;
                case 7:
                    _directions[(Vector3.forward + Vector3.right)] = weight;
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
