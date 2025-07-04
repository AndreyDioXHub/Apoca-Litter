using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class BotSection : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _spawnPoints = new List<Transform>();

    [SerializeField]
    private float _timeWait = 1, _timeWaitCur;

    private bool _isSpawn;
    private bool _isExit;
    private bool _needWait;

    private bool _iconsSetted;

    private bool _isGD;

    public bool IsChildSelected;

    void Start()
    {
        
    }

    void Update()
    {
        if (_isSpawn && _isExit && _needWait)
        {
            _timeWaitCur += Time.deltaTime;

            if(_timeWaitCur > _timeWait)
            {
                _isSpawn = false;
                _isExit = false;
                _timeWaitCur = 0;
            }
        }
    }

    public void SectionEnter()
    {
        if (!_isSpawn)
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                if(spawnPoint != null)
                {
                    //BotsManager.Instance.SpawnBot(spawnPoint.position);
                }
            }
            _isSpawn = true;
        }        
    }

    public void SectionExit()
    {
        if (!_isExit)
        {
            _isExit = true;
        }

        if (!_needWait)
        {
            _isSpawn = false;

        }

        Destroy(gameObject);

    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        IsChildSelected = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            var go = transform.GetChild(i).gameObject;

            if (Selection.Contains(go))
            {
                IsChildSelected = true;
                i = transform.childCount;
            }
        }

        if (Selection.Contains(gameObject))
        {
            IsChildSelected = true;
        }

        if (IsChildSelected)
        {

            var iconMain = EditorGUIUtility.IconContent("sv_label_2");
            EditorGUIUtility.SetIconForObject(gameObject, (Texture2D)iconMain.image);

            var iconSP = EditorGUIUtility.IconContent("sv_label_3");

            for (int i = 0; i < _spawnPoints.Count; i++)
            {
                if (_spawnPoints[i] != null)
                {
                    _spawnPoints[i].gameObject.name = $"SpawnPoint ({i}) S{gameObject.name.Replace("BotSection", "")}";
                    EditorGUIUtility.SetIconForObject(_spawnPoints[i].gameObject, (Texture2D)iconSP.image);
                }
            }
            _iconsSetted = false;
        }
        else
        {
            if (!_iconsSetted)
            {
                _iconsSetted = true;
                var iconMain = EditorGUIUtility.IconContent("none");
                EditorGUIUtility.SetIconForObject(gameObject, (Texture2D)iconMain.image);

                var iconSP = EditorGUIUtility.IconContent("none");

                for (int i = 0; i < _spawnPoints.Count; i++)
                {
                    if (_spawnPoints[i] != null)
                    {
                        EditorGUIUtility.SetIconForObject(_spawnPoints[i].gameObject, (Texture2D)iconSP.image);
                    }
                }
            }
        }

    }

#endif
    /*
    */
}
