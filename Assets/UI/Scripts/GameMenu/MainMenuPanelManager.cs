using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuPanelManager : MonoBehaviour
{

    private static MainMenuPanelManager _instance;

    private Stack<MenuPanel> _history = new Stack<MenuPanel>();
    public static MainMenuPanelManager Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject(nameof(MainMenuPanelManager));
                _instance = go.AddComponent<MainMenuPanelManager>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private List<MenuPanel> menuPanels = new List<MenuPanel>();
    private MenuPanel _currentPanel;

    public void RegisterPanel(MenuPanel panel) {
        if (!menuPanels.Contains(panel)) {
            menuPanels.Add(panel);
            panel.gameObject.SetActive(panel.EnableOnStart);
            if(panel.IsMainPanel) {
                _currentPanel = panel;
            }
        }
    }

    public void SwitchPanel(string panelName) {
        
        if (_currentPanel != null) {
            _history.Push(_currentPanel);
        }
        foreach (var panel in menuPanels) {
            panel.gameObject.SetActive(panel.name == panelName);
            
            if(panel.gameObject.activeSelf) {
                _currentPanel = panel;

            }

        }
    }

    public void Back() {
        if(_history.Count > 0) {
            var lastPanel = _history.Pop();
            SwitchPanel(lastPanel.name);
        }
    }


    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    internal void Close(MenuPanel menuPanel) {
        if (_currentPanel == menuPanel) _currentPanel = null;
        menuPanel.gameObject.SetActive(false);
    }
}
