using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasWinLoseUI : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        WinLose.Instance.OnWin.AddListener(()=> PlayClip("Win_Lose In game Animation"));
    }

    void Update()
    {

    }


    public virtual void PlayClip(string clip)
    {
        AnimatorClipInfo[] infos = _animator.GetCurrentAnimatorClipInfo(_animator.GetLayerIndex("Base Layer"));

        if (infos.Length > 0)
        {
            if (infos[0].clip.name.Equals(clip))
            {
            }
            else
            {
                _animator.CrossFade(clip, 0.05f, _animator.GetLayerIndex("Base Layer"), 0);
            }
        }
    }
}
