using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsBus : MonoBehaviour
{
    public static EventsBus Instance;

    public UnityEvent<string> OnNameUpdated = new UnityEvent<string>();
    public UnityEvent<Vector3> OnExplosionCenter = new UnityEvent<Vector3>();

    public UnityEvent<bool> OnUseAutoAim = new UnityEvent<bool>();
    public UnityEvent<bool> OnUseAimAssist = new UnityEvent<bool>();

    public UnityEvent<float> OnMouseSensitivityChanged = new UnityEvent<float>();
    public UnityEvent<float> OnAudioLVLChanged = new UnityEvent<float>();
    public UnityEvent<float> OnAudioDrillLVLChanged = new UnityEvent<float>();
    public UnityEvent<float> OnAudioMusicLVLChanged = new UnityEvent<float>();
    public UnityEvent<float> OnDrawingDistanceChanged = new UnityEvent<float>();
    public UnityEvent<int, int> OnEnemyCount = new UnityEvent<int, int>();

    public UnityEvent OnTeleport = new UnityEvent();
    public UnityEvent OnSandBoxStart = new UnityEvent();
    public UnityEvent OnSandBoxAborted = new UnityEvent();

    public UnityEvent<GameObject, GameObject> OnBotDieFromKiller = new UnityEvent<GameObject, GameObject>();
    public UnityEvent OnLevelContinue = new UnityEvent();
    public UnityEvent OnNextLevel = new UnityEvent();
    public UnityEvent OnOpenMainMenu = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
