using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinDefaultView : MonoBehaviour
{
    public string Name => _name;

    public SkinnedMeshRenderer BodyRenderer => _bodyRenderer;

    [SerializeField]
    private string _memoryLickText = "";
    [SerializeField]
    private string _name = "";
    [SerializeField]
    private GameObject _body;
    [SerializeField]
    private SkinnedMeshRenderer _bodyRenderer;
    [SerializeField]
    private Material _materialBody;
    [SerializeField]
    private Material _materialHair;
    [SerializeField]
    private Shader _sharer;
    [SerializeField]
    private List<GameObject> _hats = new List<GameObject>();
    [SerializeField]
    private List<MeshRenderer> _hatRenderers = new List<MeshRenderer>();
    [SerializeField]
    private List<Texture2D> _faces = new List<Texture2D>();
    [SerializeField]
    private List<Color> _colors = new List<Color>();

    [SerializeField]
    private Texture2D _bodyMask;
    [SerializeField]
    private Texture2D _shoeMask;
    [SerializeField]
    private Texture2D _pentsMask;
    [SerializeField]
    private Texture2D _beltMask;
    [SerializeField]
    private Texture2D _tshirtMask;

    void Start()
    {
        var defaultSkinModels = MenuPuppeteer.Instance.transform.GetComponentsInChildren<DefaultSkinModel>(true);

        foreach (var skinModel in defaultSkinModels)
        {
            if (skinModel.Name.Equals(_name))
            {
                skinModel.SetView(this);
            }
        }

        _materialBody = new Material(_sharer);
        _materialBody.name = "body";

        _materialBody.SetTexture("_BodyMask", _bodyMask);
        _materialBody.SetTexture("_ShoesMask", _shoeMask);
        _materialBody.SetTexture("_PantsMask", _pentsMask);
        _materialBody.SetTexture("_BeltMask", _beltMask);
        _materialBody.SetTexture("_TshirtMask", _tshirtMask);

        _materialHair = new Material(_sharer);
        _materialHair.name = "hair1";
    }

    void Update()
    {
        
    }

    public void SetActiveBody(bool isActive)
    {
        _body.SetActive(isActive);
    }

    public void Equip(string name, int index)
    {
        switch (name)
        {
            case "bodytype":
                break;
            case "haircut":
                foreach (GameObject go in _hats)
                {
                    go.SetActive(false);
                }
                _hats[index].SetActive(true);
                break;
            case "face":
                //Debug.Log(_memoryLickText);
                _materialBody.SetTexture("_FaceMask", _faces[index]);

                var bodymaterials = _bodyRenderer.materials;
                List<Material> bodymaterialsList = new List<Material>();

                foreach (var material in bodymaterials)
                {
                    bodymaterialsList.Add(material);
                }

                for (int i = 0; i < bodymaterials.Length; i++)
                {
                    bodymaterialsList[i] = _materialBody;
                }

                _bodyRenderer.SetMaterials(bodymaterialsList);
                break;
            case "haircolor":

                //Debug.Log(_memoryLickText);
                _materialHair.SetColor("_MainColor", _colors[index]);
                foreach (MeshRenderer renderer in _hatRenderers)
                {
                    var materials = renderer.materials;
                    List<Material> materialsList = new List<Material>();

                    foreach (var material in materials)
                    {
                        materialsList.Add(material);
                    }

                    for(int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i].name.Contains("hair1"))
                        {
                            materialsList[i] = _materialHair;
                        }
                    }

                    renderer.SetMaterials(materialsList);
                }
                break;
            case "bodycolor":
                //Debug.Log(_memoryLickText);
                _materialBody.SetColor("_BodyColor", _colors[index]);

                bodymaterials = _bodyRenderer.materials;
                bodymaterialsList = new List<Material>();

                foreach (var material in bodymaterials)
                {
                    bodymaterialsList.Add(material);
                }

                for (int i = 0; i < bodymaterials.Length; i++)
                {
                    bodymaterialsList[i] = _materialBody;
                }

                _bodyRenderer.SetMaterials(bodymaterialsList);
                break;
            case "tshirtcolor":
                //Debug.Log(_memoryLickText);
                _materialBody.SetColor("_TshirtColor", _colors[index]);

                bodymaterials = _bodyRenderer.materials;
                bodymaterialsList = new List<Material>();

                foreach (var material in bodymaterials)
                {
                    bodymaterialsList.Add(material);
                }

                for (int i = 0; i < bodymaterials.Length; i++)
                {
                    bodymaterialsList[i] = _materialBody;
                }

                _bodyRenderer.SetMaterials(bodymaterialsList);
                break;
            case "pantscolor":
                //Debug.Log(_memoryLickText);
                _materialBody.SetColor("_PentsColor", _colors[index]);

                bodymaterials = _bodyRenderer.materials;
                bodymaterialsList = new List<Material>();

                foreach (var material in bodymaterials)
                {
                    bodymaterialsList.Add(material);
                }

                for (int i = 0; i < bodymaterials.Length; i++)
                {
                    bodymaterialsList[i] = _materialBody;
                }

                _bodyRenderer.SetMaterials(bodymaterialsList);
                break;
            case "shoescolor":
                //Debug.Log(_memoryLickText);
                _materialBody.SetColor("_ShoesColor", _colors[index]);

                bodymaterials = _bodyRenderer.materials;
                bodymaterialsList = new List<Material>();

                foreach (var material in bodymaterials)
                {
                    bodymaterialsList.Add(material);
                }

                for (int i = 0; i < bodymaterials.Length; i++)
                {
                    bodymaterialsList[i] = _materialBody;
                }

                _bodyRenderer.SetMaterials(bodymaterialsList);
                break;
        }
    }
}
