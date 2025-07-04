using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DethCamUIScreen : MonoBehaviour
{
    void Start()
    {
        //BotManagerNetwork.Instance.OnNewRoundStarted.AddListener(() => gameObject.SetActive(false));
    }

    void Update()
    {

    }

    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveDethCam(true);
    }

    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveDethCam(false);;
    }
}
