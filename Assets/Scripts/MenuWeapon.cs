using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWeapon : MonoBehaviour
{
    public bool Avaleble{ 
        get
        {
            bool avaleble = PlayerPrefs.GetInt($"{_weaponName}_avaleble", 0) == 1;

            avaleble = !avaleble ? _avaleble : avaleble;
            _avaleble = avaleble;

            PlayerPrefs.SetInt($"{_weaponName}_avaleble", _avaleble ? 1 : 0);

            return avaleble;
        }
        set
        {
            _avaleble = value;

            PlayerPrefs.SetInt($"{_weaponName}_avaleble", _avaleble ? 1 : 0);
        }
    }

    public int Cost;

    [SerializeField]
    private string _weaponName;

    [SerializeField]
    private List<GameObject> _grips = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _lasers = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _muzzles = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _scopes = new List<GameObject>();

    [SerializeField]
    private int _indexGrip = -1;
    [SerializeField]
    private int _indexLaser = -1;
    [SerializeField]
    private int _indexMuzzle = 0;
    [SerializeField]
    private int _indexScope = -1;

    [SerializeField]
    private bool _avaleble;

    void Start()
    {
        _indexGrip = PlayerPrefs.GetInt($"{_weaponName}_grip", _indexGrip);
        if(_indexGrip>0)
            _grips[_indexGrip].SetActive(true);

        _indexLaser = PlayerPrefs.GetInt($"{_weaponName}_laser", _indexLaser);
        if(_indexLaser > 0)
            _lasers[_indexLaser].SetActive(true);

        _indexMuzzle = PlayerPrefs.GetInt($"{_weaponName}_muzzle", _indexMuzzle);
        if(_indexMuzzle > 0)
            _muzzles[_indexMuzzle].SetActive(true);

        _indexScope = PlayerPrefs.GetInt($"{_weaponName}_scope", _indexScope);
        if(_indexScope > 0)
            _scopes[_indexScope].SetActive(true);
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (MenuKitsUI.Instance != null)
        {
            MenuKitsUI.Instance.SetNewWeapon(this);
        }
    }

    public void Next(int type, int first=-1)
    {
        int index = 0;
        List<GameObject> objects = new List<GameObject>();
        string types = "";

        switch (type)
        {
            case 0:
                index = _indexGrip;
                objects = _grips;
                types = "grip";
                break;
            case 1:
                index = _indexLaser;
                objects = _lasers;
                types = "laser";
                break;
            case 2:
                index = _indexMuzzle;
                objects = _muzzles;
                types = "muzzle";
                first = 0;
                break;
            case 3:
                index = _indexScope;
                objects = _scopes;
                types = "scope";
                break;
            default:
                break;
        }

        index++;
        index = index >= objects.Count ? first : index;

        if (index > 0)
        {
            if (objects[index].name.Equals("P_LPSP_WEP_ATT_Scope_05"))
            {
                index++;
            }
        }

        PlayerPrefs.SetInt($"{_weaponName}_{types}", index);

        foreach (var obj in objects)
        {
            obj.SetActive(false);
        }

        if (index >= 0)
        {
            objects[index].SetActive(true);
        }

        switch (type)
        {
            case 0:
                _indexGrip = index;
                break;
            case 1:
                _indexLaser = index;
                break;
            case 2:
                _indexMuzzle = index;
                break;
            case 3:
                _indexScope = index;
                break;
            default:
                break;
        }
    }

    public void Prev(int type, int first = -1)
    {
        int index = 0;
        List<GameObject> objects = new List<GameObject>();
        string types = "";

        switch (type)
        {
            case 0:
                index = _indexGrip;
                objects = _grips;
                types = "grip";
                break;
            case 1:
                index = _indexLaser;
                objects = _lasers;
                types = "laser";
                break;
            case 2:
                index = _indexMuzzle;
                objects = _muzzles;
                types = "muzzle";
                first = 0;
                break;
            case 3:
                index = _indexScope;
                objects = _scopes;
                types = "scope";
                break;
            default:
                break;
        }

        index--;
        index = index < first ? objects.Count - 1 : index;

        if(index > 0)
        {
            if (objects[index].name.Equals("P_LPSP_WEP_ATT_Scope_05"))
            {
                index--;
            }
        }

        PlayerPrefs.SetInt($"{_weaponName}_{types}", index);

        foreach (var obj in objects)
        {
            obj.SetActive(false);
        }

        if (index >= 0)
        {
            objects[index].SetActive(true);
        }

        switch (type)
        {
            case 0:
                _indexGrip = index;
                break;
            case 1:
                _indexLaser = index;
                break;
            case 2:
                _indexMuzzle = index;
                break;
            case 3:
                _indexScope = index;
                break;
            default:
                break;
        }
    }

}
