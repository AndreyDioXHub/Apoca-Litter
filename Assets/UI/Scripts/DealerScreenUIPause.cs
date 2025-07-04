using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerScreenUIPause : MonoBehaviour
{


    void Start()
    {
        
    }

    void Update()
    {

    }
    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveDillerScreen(true);
    }

    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveDillerScreen(false); 
    }
}
