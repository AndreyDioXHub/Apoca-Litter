using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerPlayerCollider : MonoBehaviour
{
    public PlayerNetworkResolver Resolver => _resolver;

    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private DealerZone _dillerZone;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (_resolver.isClient)
        {
            if (other.name.Contains("DealerZone"))
            {
                Debug.Log("DealerZone is founded");
                _dillerZone = other.GetComponent<DealerZone>();
                
                if (_dillerZone.IsDudeInside)
                {

                }
                else
                {
                    if (_dillerZone.IsHaveTask)
                    {
                        _dillerZone.SetPlayerResolver(_resolver);
                        _resolver.GoIntoDealerZone(_dillerZone);
                        _resolver.SetDealerInsideOnServer(true);
                    }
                    else
                    {
                        if (_resolver.IsHaveTask)
                        {
                            _dillerZone.SetPlayerResolver(_resolver);
                            _resolver.GoIntoDealerZone(_dillerZone);
                            _resolver.SetDealerInsideOnServer(true);
                        }                        
                    }
                }
            }
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        /*
        if (_resolver.isClient)
        {
            if (other.name.Contains("DealerZone"))
            {
                if (_dillerZone != null)
                {
                    Debug.Log("Live DealerZone");
                    _resolver.SetDealerInsideOnServer(false);
                    _dillerZone.SetPlayerResolver(null);
                    _dillerZone = null;
                }
            }
        }*/
    }
}
