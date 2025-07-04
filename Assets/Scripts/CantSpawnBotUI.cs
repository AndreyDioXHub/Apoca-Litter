using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantSpawnBotUI : MonoBehaviour
{
    public static CantSpawnBotUI Instance;

    [SerializeField]
    private GameObject _screen;

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

    public void Show()
    {
        StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        _screen.SetActive(true);
        yield return new WaitForSeconds(2);
        _screen.SetActive(false);
    }
}
