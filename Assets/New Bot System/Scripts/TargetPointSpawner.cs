using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointSpawner : MonoBehaviour
{
    private GameObject _targetPoint;

    void Start()
    {
        GameObject targetPointPrefab = Resources.Load<GameObject>("TargetPoint");// AssetDatabase.LoadAssetAtPath("Assets/PointArrow/TargetPoint.prefab", typeof(GameObject));
        _targetPoint = Instantiate(targetPointPrefab);
        _targetPoint.GetComponent<TargetPoint>().Init(gameObject, "");
        _targetPoint.transform.localPosition = Vector3.up * 0.5f;
        GetComponent<BotHP>().OnDead.AddListener(Sleep);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Sleep(GameObject sender, GameObject killer)
    {
        _targetPoint.transform.SetParent(null);
        _targetPoint.GetComponent<TargetPoint>().DestroyEvent();
    }
}
