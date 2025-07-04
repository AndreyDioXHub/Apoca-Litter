using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TermOfUsePanelUIPause : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveTermOfUsePanelScreen(true);
    }
    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveTermOfUsePanelScreen(false);
    }
}
