using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewBotSystem
{
    public class BotAnimatorManager : MonoBehaviour
    {
        public Animator BotAnimator => _animator;

        [SerializeField]
        private Animator _animator;

        private void Awake()
        {
        }

        void Start()
        {
        }

        void Update()
        {

        }

        public void PlayClip(string clip)
        {
            //Debug.Log($"PlayClip {clip}");

            if (_animator == null)
            {
                //Debug.Log($"Here is no animator");
                return;
            }

            AnimatorClipInfo[] infos = _animator.GetCurrentAnimatorClipInfo(_animator.GetLayerIndex("Base Layer"));

            if (infos.Length > 0)
            {
                //Debug.Log($"infos Length > 0");

                if (infos[0].clip.name.Equals(clip))
                {
                    //Debug.Log($"try play curent clip");
                }
                else
                {
                    //Debug.Log($"push new clip {clip} to animator");
                    _animator.CrossFade(clip, 0.05f, _animator.GetLayerIndex("Base Layer"), 0);
                }
            }
            else
            {
                //Debug.Log($"infos Length = 0");
            }
        }

        public string CurrentAnimation()
        {
            if (_animator == null)
            {
                return "";
            }

            AnimatorClipInfo[] infos = _animator.GetCurrentAnimatorClipInfo(_animator.GetLayerIndex("Base Layer"));

            string name = "";
            try
            {
                name = infos[0].clip.name;
            }
            catch (Exception e)
            {
                //Debug.Log(e.Message);
            }

            return name;

        }
    }
}
