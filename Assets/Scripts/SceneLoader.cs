using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public UnityEvent OnLoadScene = new UnityEvent();

    [SerializeField]
    private int _index;
    [SerializeField]
    private bool  _enyKey;

    public void LoadScene(int index)
    {
        Debug.Log("LoadScene");
        Time.timeScale = 1;
        OnLoadScene?.Invoke();
        _index = index;
        SceneManager.LoadScene(_index);
    }

    public void LoadSceneDelay(int index, float delay)
    {
        StartCoroutine(LoadSceneDelayCoroutine(index, delay));
    }

    public void LoadSceneDelay(int index)
    {
        Time.timeScale = 1;
        LoadSceneDelay(index, 1);
    }

    IEnumerator LoadSceneDelayCoroutine(int index, float delay)
    {
        yield return new WaitForSeconds(delay); 
        LoadScene(index);
    }

    public void Update()
    {
        if (_enyKey && Time.timeSinceLevelLoad > 3)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(_index);
            }
        }
    }

    public void LoadSceneWithLoader(int index) {
        AddressableSceneManager.Instance.LoadBuiltinScene(index).Forget();
    }
}
