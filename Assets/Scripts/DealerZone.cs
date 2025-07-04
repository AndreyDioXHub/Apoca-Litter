using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class DealerZone : MonoBehaviour
{

    public Transform DealerOut => _dealerOut;
    public float TaskTime => _taskTime;
    public DealerZone SiblingZone => _siblingZone;

    [SerializeField]
    private float _taskTime = 10;
    [SerializeField]
    private Transform _dealerOut;
    [SerializeField]
    private DealerZone _siblingZone;

    [SerializeField]
    private GameObject _markerCommon;
    [SerializeField]
    private GameObject _markerTask;

    [SerializeField]
    private PlayerNetworkResolver _resolverInside;

    void Start()
    {
        //DealerZonesManager.Instance.RegisterZone(this);
        _markerTask.transform.position = _markerCommon.transform.position;
        AwateDealerZonesManager();
    }

    public async void AwateDealerZonesManager()
    {
        while (DealerZonesManager.Instance == null)
        {
            await UniTask.Yield();
        }
        DealerZonesManager.Instance.RegisterZone(this);
    }


    void Update()
    {
    }

    private void FixedUpdate()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("DealerPlayerCollider"))
        {
            _resolverInside = other.GetComponent<DealerPlayerCollider>().Resolver;
        }

        //Debug.Log(PlayerName);

        _resolverInside.GoIntoDealerZone(this);
        _resolverInside.SetDealerInside(true);
    }

    public void HideDefaultMarker()
    {
        _markerCommon.SetActive(false);
    }

    public void HideTaskMarker()
    {
        _markerTask.SetActive(false);
    }

    public void ShowDefaultMarker()
    {
        _markerCommon.SetActive(true);
    }

    public void ShowTaskMarker()
    {
        _markerTask.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    { 
    }

    public void SetNewSibling(DealerZone siblingZone)
    {
        _siblingZone = siblingZone;

        if(siblingZone == null)
        {
            //Debug.Log($"{gameObject.name} Clear Sibling");
        }
        else
        {
            //Debug.Log($"{gameObject.name} SetNewSibling {siblingZone.gameObject.name}");
            //siblingZone.SetNewSibling(this);
        }
    }
}
