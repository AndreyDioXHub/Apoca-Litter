using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImpactUI : MonoBehaviour
{
    public static ImpactUI Instance;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Color _impactColorGood;
    [SerializeField]
    private Color _impactColorBad;
    [SerializeField]
    private Color _impactColorStuff;
    [SerializeField]
    private List<Image> _borders = new List<Image>();

    private float _cur;
    private float _delta;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var borders = GetComponentsInChildren<Image>();

        foreach (var b in borders)
        {
            _borders.Add(b);
        }

        AddListenerOnCharackter();
    }

    public void Impact(int colorIndex)
    {
        foreach (var b in _borders)
        {
            switch (colorIndex)
            {
                case 0:
                    b.color = _impactColorGood;
                    break;
                case 1:
                    b.color = _impactColorBad;
                    break;
                case 2:
                    b.color = _impactColorStuff;
                    break;
                default:
                    b.color = Color.white;
                    break;
            }
        }

        PlayClip("ImpactUIImpactAnimation");
    }

    public void Impact(float cur, float max)
    {
        _cur = _cur == 0 ? max : _cur;

        _delta = _cur - cur;

        if (_delta == 0)
        {
            return;
        }

        if (_delta < 0)
        {
            Impact(0);
        }
        else
        {
            Impact(1);
        }

        _cur = cur;
    }

    private async void AddListenerOnCharackter()
    {
        while (Character.Instance == null)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }
        Character.Instance.OnDamageRecived.AddListener(Impact);
    }

    public void PlayClip(string clip)
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

    void Update()
    {
        
    }
}
