using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static Mirror.BouncyCastle.Math.EC.ECCurve;

public class SkinResolverListener : MonoBehaviour
{
    public SkinnedMeshRenderer ActiveRenderer;
    public string SexMane;

    [SerializeField]
    private PlayerNetworkResolver _resolver;

    [SerializeField]
    private List<SkinDefaultView> _views = new List<SkinDefaultView>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init(string name, string json)
    {

        int index = name.Equals("Man") ? 0 : 1;

        foreach (var view in _views)
        {
            view.SetActiveBody(false);
        }

        _views[index].SetActiveBody(true);

        SkinChanged(name, json);

        _resolver.OnSkinChanged.AddListener(SkinChanged);
    }

    public void SkinChanged(string name, string json)
    {
        foreach (var view in _views)
        {
            if (view.Name.Equals(name))
            {
                SexMane = name;

                DefaultSkinConfig config = JsonConvert.DeserializeObject<DefaultSkinConfig>(json);

                view.SetActiveBody( config.isActive);
                view.Equip("haircut", config.hairCutIndex);
                view.Equip("haircolor", config.hairColorIndex);
                view.Equip("face", config.faceIndex);
                view.Equip("bodycolor", config.bodycolorIndex);
                view.Equip("tshirtcolor", config.tshirtcolorIndex);
                view.Equip("pantscolor", config.pantscolorIndex);
                view.Equip("shoescolor", config.shoescolorIndex);
                ActiveRenderer = view.BodyRenderer;
            }
        }
    }
}
