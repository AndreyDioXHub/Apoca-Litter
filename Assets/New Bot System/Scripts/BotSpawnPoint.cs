using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawnPoint : MonoBehaviour
{
    public List<Transform> PatrolPoints => _patrolPoints;

    [SerializeField]
    private List<Transform> _patrolPoints = new List<Transform>();


    void Start()
    {
        for(int i=0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name.Contains("PatrolPoint"))
            {
                _patrolPoints.Add(transform.GetChild(i));
            }
        }
        /*
        if (BotManager.Instance.NeedCollectSpawnPoints)
        {
            //BotManager.Instance.RegisterSpawnPoint(transform);
        }*/
    }

    void Update()
    {
        
    }
}
