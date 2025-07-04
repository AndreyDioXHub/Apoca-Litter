using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insider : MonoBehaviour
{
    public bool dfgsd;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        dfgsd = true;
    }

    private void OnTriggerExit(Collider other)
    {
        dfgsd = false;
    }
}
