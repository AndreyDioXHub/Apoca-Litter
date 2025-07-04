using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasSpawner : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private GameObject _canvasPrefab;
    [SerializeField]
    private Inventory _inventory;
    [SerializeField]
    private Character _character;

    void Start()
    {
    }

    public void SpawnCanvas(Inventory inventory, Character character)
    {
        var go = Instantiate(_canvasPrefab);
        _inventory = inventory;

        PauseScreen pauseScreen = go.GetComponent<PauseScreen>();
        pauseScreen.Init(_resolver, character, _inventory);

        ChangeAttachementScreen changeAttachementScreen = go.GetComponentInChildren<ChangeAttachementScreen>(true);
        changeAttachementScreen.Init(_resolver, character, _inventory);

        ChatScreenUIPause chatScreenUIPause = go.GetComponentInChildren<ChatScreenUIPause>(true);
        chatScreenUIPause.Init(_resolver, _inventory);

        ChelikiMenuModel menuModel = go.GetComponentInChildren<ChelikiMenuModel>(true);

        var defaultSkinModels = menuModel.transform.GetComponentsInChildren<DefaultSkinModel>(true);
        var defaultSkinViews = MenuPuppeteer.Instance.transform.GetComponentsInChildren<SkinDefaultView>(true);
        List<SkinDefaultView> defaultSkinViewsList = new List<SkinDefaultView>();

        foreach (var view in defaultSkinViews)
        {
            defaultSkinViewsList.Add(view);
        }

        foreach (var skinModel in defaultSkinModels)
        {
            SkinDefaultView view = defaultSkinViewsList.Find(v => v.Name.Equals(skinModel.Name));
            skinModel.SetView(view);
        }

        menuModel.Init(_resolver, character, _inventory);
    }

    void Update()
    {
        
    }
}
