using System;
using Mirror.Examples.Basic;
using Newtonsoft.Json;
using UnityEngine;

public class DefaultSkinModel : MonoBehaviour
{
    public string Name => _config.sex;

    private ChelikiMenuModel _model;

    [SerializeField]
    private DefaultSkinConfig _config;
    [SerializeField]
    private SkinDefaultView _view;
    [SerializeField]
    private GameObject _hairCutMenu;
    [SerializeField]
    private GameObject _hairColorMenu;
    [SerializeField]
    private GameObject _faceMenu;
    [SerializeField]
    private GameObject _bodyMenu;
    [SerializeField]
    private GameObject _tshirtMenu;
    [SerializeField]
    private GameObject _pantsMenu;
    [SerializeField]
    private GameObject _shoesMenu;

    [SerializeField]
    private string _skinString;

    void Start()
    {
    }

    public void Init(ChelikiMenuModel model)
    {
        _model = model;
        _model.OnSelectNameIndex.AddListener(Equip);
    }

    public void SetView(SkinDefaultView view)
    {
        _view = view;
    }

    void Update()
    {

    }

    public void Equip(string name, int index)
    {
        _view.Equip(name, index);

        switch (name)
        {
            case "bodytype":
                _config.isActive = index == _config.sexIndex;
                _view.SetActiveBody(_config.isActive);
                break;
            case "haircut":
                _config.hairCutIndex = index;
                break;
            case "haircolor":
                _config.hairColorIndex = index;
                break;
            case "face":
                _config.faceIndex = index;
                break;
            case "bodycolor":
                _config.bodycolorIndex = index;
                break;
            case "pantscolor":
                _config.pantscolorIndex = index;
                break;
            case "tshirtcolor":
                _config.tshirtcolorIndex = index;
                break;
            case "shoescolor":
                _config.shoescolorIndex = index;
                break;
        }

        _skinString = JsonConvert.SerializeObject(_config);
        
        _model.OnSkinChanged?.Invoke(_config.sex, _skinString);

        PlayerPrefs.SetString(Name, _skinString);
    }

    public void CloseCategory()
    {
        gameObject.SetActive(false);
    }

    public void OpenHairMenu()
    {
        _hairCutMenu.SetActive(true);
    }

    public void OpenHairColorMenu()
    {
        _hairColorMenu.SetActive(true);
    }

    public void CloseHairCutMenu()
    {
        _hairCutMenu.SetActive(false);
    }

    public void CloseHairColorMenu()
    {
        _hairColorMenu.SetActive(false);
    }
    public void OpenFaceMenu()
    {
        _faceMenu.SetActive(true);
    }

    public void CloseFaceMenu()
    {
        _faceMenu.SetActive(false);
    }

    public void OpenBodyColorMenu()
    {
        _bodyMenu.SetActive(true);
    }

    public void CloseBodyColorMenu()
    {
        _bodyMenu.SetActive(false);
    }

    public void OpenTshirtColorMenu()
    {
        _tshirtMenu.SetActive(true);
    }

    public void CloseTshirtColorMenu()
    {
        _tshirtMenu.SetActive(false);
    }    

    public void OpenPantsColorMenu()
    {
        _pantsMenu.SetActive(true);
    }

    public void ClosePantsColorMenu()
    {
        _pantsMenu.SetActive(false);
    }

    public void OpenShoesColorMenu()
    {
        _shoesMenu.SetActive(true);
    }

    public void CloseShoesColorMenu()
    {
        _shoesMenu.SetActive(false);
    }

}

[Serializable]
public class DefaultSkinConfig
{
    public string sex;
    public bool isActive;
    public int sexIndex;
    public int hairCutIndex;
    public int hairColorIndex;
    public int faceIndex;
    public int bodycolorIndex;
    public int tshirtcolorIndex;
    public int pantscolorIndex;
    public int shoescolorIndex;
}
