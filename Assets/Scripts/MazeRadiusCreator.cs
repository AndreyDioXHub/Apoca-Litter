using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRadiusCreator : MonoBehaviour
{

    [SerializeField]
    private Vector3 _center;
    [SerializeField]
    private float _radius = 10;


    void Start()
    {
        
    }
    /*
    [ContextMenu("Spawn")]
    private void Spawn()
    {
        int x, z;
        float angle = 0.0f;
        float interval = 0.025f;

        while (angle < 2 * Mathf.PI)
        {
            x = (int)(_radius * Mathf.Cos(angle));
            z = (int)(_radius * Mathf.Sin(angle));
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_mazeCubePrefab as GameObject);
            go.transform.position = new Vector3(x, 0, z);
            //var go = Instantiate(_mazeCubePrefab, new Vector3(x, 0, z), Quaternion.identity);
            go.transform.SetParent(transform);
            angle += interval;
        }
    }
    */

    [ContextMenu("Spawn")]
    private void Spawn()
    {/*
        List<Vector3> upts = PointsOnSphere(128);

        foreach (var poiny in upts)
        {
            Debug.DrawLine(_center, poiny, Color.red, 5);
        }*/

        for(int i=0; i< 128; i++)
        {
            Vector3 rand = Random.insideUnitSphere;
            Debug.DrawLine(_center, rand, Color.red, 5);
        }

        /*
        int x, z;
        float angle = 0.0f;
        float interval = 2 * Mathf.PI / 20;

        while (angle < 2 * Mathf.PI)
        {
            x = (int)(_radius * Mathf.Cos(angle));
            z = (int)(_radius * Mathf.Sin(angle));
            Debug.DrawLine(_center, new Vector3(x, 0, z), Color.red, 5);
            angle += interval;
        }*/
    }

    public List<Vector3> PointsOnSphere(int count)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / count;

        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;

        for (var k = 0; k < count; k++)
        {
            y =  (k * off - 1 + (off / 2));
            r =  Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;
            Debug.Log(r);
            upts.Add(new Vector3(x, y, z));

        }

        return upts;
    }

    /*function PointsOnSphere(n) {
    var upts = new Array();
    var inc : float = Mathf.PI * (3 - Mathf.Sqrt(5));
    var off : float = 2.0 / n;
    var x : float;
    var y : float;
    var z : float;
    var r : float;
    var phi : float;
   
    for (var k = 0; k < n; k++){
        y = k * off - 1 + (off /2);
        r = Mathf.Sqrt(1 - y * y);
        phi = k * inc;
        x = Mathf.Cos(phi) * r;
        z = Mathf.Sin(phi) * r;
       
        upts.Push(Vector3(x, y, z));
    }
    var pts : Vector3[] = upts.ToBuiltin(Vector3);
    return pts;
}
     */

    void Update()
    {
        
    }
}
