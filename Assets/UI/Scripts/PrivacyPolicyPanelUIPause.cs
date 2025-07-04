using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyPanelUIPause : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnEnable()
    {
        PauseScreen.Instance?.SetActivePrivacyPolicyPanelScreen(true);
    }
    private void OnDisable()
    {
        PauseScreen.Instance?.SetActivePrivacyPolicyPanelScreen(false);
    }
}
