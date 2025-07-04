using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvalebleWeaponView : MonoBehaviour
{
    [SerializeField]
    private MenuWeapon _weapon;

    private GameObject _avaleble;
    private GameObject _notAvaleble;

    void Start()
    {
        for(int i = 0; i< transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals("Avaleble"))
            {
                _avaleble = transform.GetChild(i).gameObject;
            }

            if (transform.GetChild(i).name.Equals("NotAvaleble"))
            {
                _notAvaleble = transform.GetChild(i).gameObject;
            }
        }

        CoinsManager.Instance.OnCoinsCountChanged.AddListener(UpdateView);

        UpdateView();
    }



    public void UpdateView()
    {
        StartCoroutine(UpdateViewtCoroutine());
    }

    IEnumerator UpdateViewtCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        bool isAvaleble = _weapon.Avaleble;

        _avaleble.SetActive(isAvaleble);
        _notAvaleble.SetActive(!isAvaleble);
    }
}
