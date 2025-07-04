using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class SurfaceBaker : MonoBehaviour
{
    public static SurfaceBaker Instance;
    public bool NavMeshReady;
    /*
    [SerializeField]
    private GameObject _startPlate;*/ //тут в инспекторе торчал куб. Просто Cube размером 300 1 300 в позиции 0    
    private NavMeshSurface _navMesh;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _navMesh = GetComponent<NavMeshSurface>();
        BuildNavMesh();
    }

    void Update()
    {
        
    }

    [ContextMenu("BuildNavMesh")]
    public void BuildNavMesh()
    {
        //_startPlate.SetActive(false);
        _navMesh.BuildNavMesh();
        NavMeshReady = true;

        /*
        if (Time.timeSinceLevelLoad > WinLose.Instance.LoadingTime)
        {
        }*/
    }
}
