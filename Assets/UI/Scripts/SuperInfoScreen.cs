using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperInfoScreen : MonoBehaviour
{
    [SerializeField]
    private Animator _drill;
    [SerializeField]
    private Animator _wave;

    void Start()
    {
        WaitBotManagerNetwork();
    }

    public async void WaitBotManagerNetwork()
    {
        while (BotManagerNetwork.Instance == null)
        {
            await UniTask.Yield();
        }
    }

    public void ShowDrill()
    {
        _drill.SetTrigger("Trigger");
    }

    public void ShowWave()
    {
        _wave.SetTrigger("Trigger");
    }

    void Update()
    {
        
    }
}
