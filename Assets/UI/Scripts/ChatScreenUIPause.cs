using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScreenUIPause : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private Inventory _inventory;
    [SerializeField]
    private Character _character;
    [SerializeField]
    private ChatViewExpander _chatViewExpander;
    void Start()
    {
        if(_chatViewExpander == null) 
        {
            _chatViewExpander = GetComponent<ChatViewExpander>();
        }

        _chatViewExpander.OnExpanded.AddListener(ChatActivate);
        _chatViewExpander.OnMinimized.AddListener(ChatClosed);

        PauseScreen.Instance.RegisterChat(this);
    }

    public void Show()
    {
        if(!_chatViewExpander.IsExpanded)
        {
            _chatViewExpander.Expand(true);
        }
    }

    public void Hide()
    {
        _chatViewExpander.Expand(false);
    }

    public void Init(PlayerNetworkResolver resolver, Inventory inventory)
    {
        _resolver = resolver;
        _inventory = inventory;
    }

    public void ChatActivate()
    {
        PauseScreen.Instance?.SetActiveChatScreen(true);
    }

    public void ChatClosed()
    {
        PauseScreen.Instance?.SetActiveChatScreen(false);
    }

    void Update()    {
        
    }


}
