using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerPuppeteer : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;

    void Start()
    {
        _resolver.OnWeaponChanged.AddListener(MenuPuppeteer.Instance.WeaponChanged);

        _resolver.OnWeaponSelected.AddListener(MenuPuppeteer.Instance.WeaponSelected);

        _resolver.OnAttachmentChanged.AddListener(MenuPuppeteer.Instance.AttachmentChanged);

        _resolver.OnWeaponAttachmentManagerSelected.AddListener(MenuPuppeteer.Instance.WeaponAttachmentManagerSelected);
    }

    void Update()
    {
        
    }
}
