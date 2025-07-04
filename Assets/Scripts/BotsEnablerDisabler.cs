using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BotsEnablerDisabler : MonoBehaviour
{
    public static BotsEnablerDisabler Instance;

    [SerializeField]
    protected List<GameObject> _bioMass = new List<GameObject>();

    [SerializeField]
    protected int _bioMassMaxCount = 20;

    public bool BotTargetsInited = false;
    public bool WeaponTargetsInited = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SlowUpdate());
    }

    void Update()
    {
        
    }

    public IEnumerator SlowUpdate()
    {
        while(!BotTargetsInited && !WeaponTargetsInited)
        {
            yield return new WaitForSeconds(0.5f);
        }

        while (true)
        {
            //List<KeyValuePair<GameObject, float>> map = new List<KeyValuePair<GameObject, float>>();
            Dictionary<GameObject, float> map = new Dictionary<GameObject, float>();

            for(int i=0; i< _bioMass.Count; i++)
            {
                if (_bioMass[i] == null)
                {

                }
                else
                {
                    map.Add(_bioMass[i], Vector3.Distance(Character.Instance.gameObject.transform.position, _bioMass[i].transform.position));
                }
            }

            var sortedMap = SortDictionary(map);

            int count = sortedMap.Count;

            count = count > _bioMassMaxCount? _bioMassMaxCount: count;


            for (int i = 0; i < sortedMap.Count; i++)
            {
                if(i < count)
                {
                    sortedMap[i].Key.SetActive(true);
                }
                else
                {
                    sortedMap[i].Key.SetActive(false);
                }
            }
            /*
            for (int i=0; i < count; i++)
            {
                sortedMap[i].Key.SetActive(true);
            }
            */
            yield return new WaitForSeconds(0.5f);
        }
    }

    public List<KeyValuePair<GameObject, float>> SortDictionary(Dictionary<GameObject, float> map)
    {
        // Сортируем словарь по значению (float) и преобразуем в список
        var sortedList = map.OrderBy(kvp => kvp.Value).ToList();
        return sortedList;
    }

    public void Register(GameObject bot)
    {
        _bioMass.Add(bot);
    }
}
