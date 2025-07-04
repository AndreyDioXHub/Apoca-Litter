using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinedStatusAnimation : MonoBehaviour
{
    public UnityEvent OnWinedAnimationEnd = new UnityEvent();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimationEnd()
    {
        OnWinedAnimationEnd?.Invoke();
    }
}
