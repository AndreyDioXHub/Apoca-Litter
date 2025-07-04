using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotAnimationEventHandled : MonoBehaviour
{
    public UnityEvent OnActackStart = new UnityEvent();
    public UnityEvent OnActackImpact = new UnityEvent();
    public UnityEvent OnActackEnd = new UnityEvent();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ActackStart()
    {
        //Debug.Log("OnStartActack");
        OnActackStart?.Invoke();    
    }

    public void AtackImpact()
    {
        //Debug.Log("OnAtackImpact");
        OnActackImpact?.Invoke();    
    }

    public void ActackEnd()
    {
        //Debug.Log("OnEndActack");
        OnActackEnd?.Invoke();
    }
}
