using game.configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuPanelLocator : MonoBehaviour
{
    public static GameMenuPanelLocator Instance { get; private set; }

    #region Panels

    public GameObject WinPanel => GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Sandbox ? _winPanelSandbox : _winPanelMissions;
    public GameObject LosePanel;

    [SerializeField]
    private GameObject _winPanelSandbox;
    [SerializeField]
    private GameObject _winPanelMissions;

    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
