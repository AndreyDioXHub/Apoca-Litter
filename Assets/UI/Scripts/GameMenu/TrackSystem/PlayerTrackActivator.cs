using InfimaGames.LowPolyShooterPack;
using MagicPigGames.Northstar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackActivator : MonoBehaviour
{
    NorthstarOverlay _overlay;
    [SerializeField]
    NavigationBar _usedBarPrefab;

    // Start is called before the first frame update
    void Awake()
    {
    }

    public void Init(Camera playerCamera, Character character)
    {
        gameObject.SetActive(true);
        _overlay = GetComponentInChildren<NorthstarOverlay>(true);
        //Set camera
        _overlay.characterPlayer = character;
        _overlay.targetCamera = playerCamera;
        _overlay.enableNavigationBar = true;

        GameObject barGO = Instantiate(_usedBarPrefab.gameObject, _overlay.transform, false);
        NavigationBar bar = barGO.GetComponent<NavigationBar>();
        _overlay.navigationBar = bar;
        bar.northstarOverlay = _overlay;

        _overlay.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
