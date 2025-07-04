using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseFailedScreen : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {

    }
    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveFailedScreen(true);
    }
    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveFailedScreen(false);
    }
}
