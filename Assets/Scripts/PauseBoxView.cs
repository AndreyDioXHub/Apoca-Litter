using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBoxView : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;

    [SerializeField]
    private GameObject _skin;
    [SerializeField]
    private GameObject _box;
    [SerializeField]
    private GameObject _assystSphere;

    void Start()
    {
        LateStart();
    }

    public async void LateStart()
    {
        await UniTask.WaitForSeconds(0.3f);
        if(_resolver != null)
        {
            ShowHideBox(_resolver.IsPaused);
        }
    }

    void Update()
    {
        
    }

    public void ShowHideAssystSphere(bool playerIsDead)
    {
        _assystSphere.SetActive(!playerIsDead);
    }

    public void ShowHideBox(bool isPause)
    {
        _skin.SetActive(!isPause);
        _box.SetActive(isPause);

        if(_assystSphere != null)
        {
            if (_resolver.IsDead)
            {
                _assystSphere.SetActive(false);
            }
            else
            {
                _assystSphere.SetActive(!isPause);
            }
        }
    }
}
