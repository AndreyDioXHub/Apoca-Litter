using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPVInventory : MonoBehaviour
{
    public List<GameObject> ExternalWeapon => _externalWeapon;

    /*
    [SerializeField]
    private PlayerNetworkResolver _resolver;*/

    private TPVInputController _input;
    [SerializeField]
    private Transform _magazineSlotHand;
    [SerializeField]
    private List<GameObject> _externalWeapon = new List<GameObject>();
    [SerializeField]
    private int _indexOld = 0;
    [SerializeField]
    private int _indexNew = 0;

    void Start()
    {
    }

    public void Init(TPVInputController input)
    {
        _input = input;

        _input.OnWeaponChanged.AddListener(OnWeaponChanged);
        _input.OnWeaponPut.AddListener(WeaponPut);
        _input.OnWeaponPull.AddListener(WeaponPull);

        foreach (GameObject weapon in _externalWeapon)
        {
            weapon.GetComponent<TPVWeapon>().Init(_input, _magazineSlotHand);
        }

        /*OnWeaponChanged(_input.IndexWeaponPrev, _input.IndexWeaponCur);
        InitPutPull();*/
    }

    public async void InitPutPull()
    {
        await UniTask.WaitForSeconds(0.1f);
        WeaponPut();
        await UniTask.WaitForSeconds(0.5f);
        WeaponPull();
    }

    void Update()
    {

    }

    private void OnEnable()
    {
        if(_input != null)
        {
            foreach (var weapon in _externalWeapon)
            {
                weapon.SetActive(false);
            }

            _externalWeapon[_input.CurrentWeaponIndex].SetActive(true);
        }
    }

    public void OnWeaponChanged(int indexOld, int indexNew)
    {
        //Debug.Log($"TPVInventory OnWeaponChanged {indexOld} {indexNew}");
        _indexOld = indexOld;
        _indexNew = indexNew;
    }

    public void WeaponPut()
    {
        foreach (var weapon in _externalWeapon)
        {
            weapon.SetActive(false);
        }
        //_externalWeapon[_indexOld].SetActive(false);
    }

    public void WeaponPull()
    {
        _externalWeapon[_indexNew].SetActive(true);
    }

}
