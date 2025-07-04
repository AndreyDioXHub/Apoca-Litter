using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem.XR;
using System;

public class Navigator : MonoBehaviour
{
    public UnityEvent OnFindingEnd = new UnityEvent();
    public UnityEvent OnBotCanMove = new UnityEvent();
    public UnityEvent OnDisableAllBots = new UnityEvent();
    public int IterationMax => _iterationMax;
    public Transform DestinationTransform => _destinationTransform;

    [SerializeField]
    private GameObject _navigationPointPrefab;
    [SerializeField]
    private Transform _destinationTransform;
    [SerializeField]
    private float _timeOut = 3;
    [SerializeField]
    private int _maxBotCount = 3;
    [SerializeField]
    private int _iterationMax = 5;

    public Dictionary<NavigationPoint, bool> Pool = new Dictionary<NavigationPoint, bool>();
    public List<NavigationPoint> PoolList = new List<NavigationPoint>();

    public Dictionary<Vector3, NavigationPoint> Field = new Dictionary<Vector3, NavigationPoint>();

    private Dictionary<Vector3, NavigationPoint> _fieldEnding = new Dictionary<Vector3, NavigationPoint>();
    private Dictionary<Vector3, int> _fieldWeight = new Dictionary<Vector3, int>();

    private int _previosFieldCount = 0;
    private BotFindMoveDirection _botDirection;
    private Queue<BotFindMoveDirection> _botDirections = new Queue<BotFindMoveDirection>();

    private bool _navigatorIsBuisy = false;

    void Start()
    {
    }

    public void StartNewWaveActivePhase()
    {
        if(DestinationTransform == null)
        {

        }
        else
        {
            OnBotCanMove?.Invoke();
        }
    }

    public void SetDestenation(Transform destinationTransform)
    {
        _destinationTransform = destinationTransform;
    }

    public NavigationPoint GetNavigationPoint(GameObject sender)
    {
        //Debug.Log("Sender", sender);
        NavigationPoint navigationPoint = PoolList.Find(p => !p.IsBuisy);
        if(navigationPoint == null)
        {
            var go = Instantiate(_navigationPointPrefab);
            navigationPoint = go.GetComponent<NavigationPoint>();
            PoolList.Add(navigationPoint);
        }
        navigationPoint.ClearPoint();
        return navigationPoint;
    }

    public GameObject GetNextPoint(Vector3 position)
    {
        Vector3 flooredPosition = FloorPposition(transform.position);
        return Field[flooredPosition].NextPoint;
    }

    public List<Vector3> GetPath(Vector3 position)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 flooredPosition = FloorPposition(transform.position);

        for(int i=0; i< 10; i++)//10 - is _iterationMax from NavigationPoint
        {
            path.Add(Field[flooredPosition].gameObject.transform.position);
            flooredPosition = FloorPposition(Field[flooredPosition].NextPoint.transform.position);
        }

        /*
        while(Field.TryGetValue(flooredPosition, out NavigationPoint navigationPooint))
        {
        }*/
        //OnDisableAll?.Invoke();
        return path;
    }

    public Vector3 FloorPposition(Vector3 position)
    {
        Vector3 flooredPosition = new Vector3(
            (Mathf.Ceil(position.x * 2) / 2f) - 0.25f,
            Mathf.Ceil(position.y * 2) / 2f,
            (Mathf.Ceil(position.z * 2) / 2f) - 0.25f
        );

        return flooredPosition;
    }

    public void AddBotToQueue(BotFindMoveDirection botDirection, out bool isAdded)
    {
        isAdded = false;
        return;

        if(_destinationTransform != null)
        {
            if (_botDirections.Count < _maxBotCount)
            {
                if (_botDirections.Contains(botDirection))
                {

                }
                else
                {
                    _botDirections.Enqueue(botDirection);
                    isAdded = true;
                }
            }
            else
            {

            }
        }
    }

    [ContextMenu("RecalculatePath")]
    public void RecalculatePath()
    {
        _navigatorIsBuisy = true;
        Debug.Log($"RecalculatePath {_botDirections.Count}");

        _botDirection = _botDirections.Dequeue();
        transform.position = FloorPposition(_botDirection.transform.position - 
            Vector3.up * _botDirection.Controller.skinWidth);

        foreach (var kvp in Field)
        {
            if (kvp.Value != null)
            {
                kvp.Value.DestroyNavigationPoint();
            }
        }

        Field.Clear();
        Field = new Dictionary<Vector3, NavigationPoint>();

        _fieldEnding.Clear();
        _fieldEnding = new Dictionary<Vector3, NavigationPoint>();

        _fieldWeight.Clear();
        _fieldWeight = new Dictionary<Vector3, int>();

        int _previosFieldCount = 0;

        //var go = Instantiate(_navigationPointPrefab);
        int nextIteration = 0;
        nextIteration++;
        int weight = 0;

        try
        {
            NavigationPoint navigationPoint = GetNavigationPoint(gameObject);
            navigationPoint.SetDestenation(_destinationTransform, nextIteration,
                transform.position, weight, this, null, _iterationMax);
            navigationPoint.transform.position = transform.position;
            Field.Add(transform.position, navigationPoint);

            WaitUntilFieldFillingEnd();
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e);
        }
    }

    private async void WaitUntilFieldFillingEnd()
    {
        await UniTask.WaitForSeconds(Time.fixedDeltaTime);

        while (_previosFieldCount != Field.Count)
        {
            _previosFieldCount = Field.Count;
            //Debug.Log($"Navigator {Field.Count}");
            await UniTask.Yield();
            //await UniTask.WaitForSeconds(0.1f);
        }

        int iterationMax = _iterationMax;

        while (_fieldEnding.Count == 0)
        {
            foreach (var kvp in Field)
            {
                if (kvp.Value.Iteration == iterationMax)
                {
                    _fieldEnding.Add(kvp.Key, kvp.Value);
                }
            }

            iterationMax--;
            await UniTask.Yield();
        }

        foreach(var kvp in _fieldEnding)
        {
            _fieldWeight.Add(kvp.Key, kvp.Value.Weight);
        }

        int minWeight = _fieldWeight.Values.Min();

        List<Vector3> candidates = _fieldEnding
            .Where(pair => pair.Value.Weight == minWeight)
            .Select(pair => pair.Key)
            .ToList();

        Vector3 randomPath = candidates[UnityEngine.Random.Range(0, candidates.Count)];

        _fieldEnding[randomPath].DrawPath(gameObject);

        await UniTask.WaitForSeconds(Time.fixedDeltaTime);

        List<Vector3> path = GetPath(_botDirection.transform.position -
            Vector3.up * _botDirection.Controller.skinWidth);

        _botDirection.FindingEnd(path);
        OnFindingEnd?.Invoke();

        /*
        if(_botDirections.Count != 0)
        {
            RecalculatePath();
        }*/

        await UniTask.WaitForSeconds(_timeOut);
        _navigatorIsBuisy = false;
    }

    void Update()
    {
        return;
        if(_botDirections.Count > 0 && !_navigatorIsBuisy) 
        {
            RecalculatePath();
        }
    }
}
