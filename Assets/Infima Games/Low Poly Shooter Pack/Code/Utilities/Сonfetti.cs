using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Сonfetti : MonoBehaviour
{

    [SerializeField]
    private GameObject _impactPrefab;
    [SerializeField]
    private Damageble _damageble;

    private void Awake()
    {
        if (_damageble == null)
        {
            _damageble = GetComponent<Damageble>();
        }
    }

    void Start()
    {
        //_damageble.OnDamage.AddListener(ReceiveDamage);
    }

    private void OnDestroy()
    {
        //_damageble.OnDamage.RemoveListener(ReceiveDamage);
    }

    void Update()
    {

    }

    /// <summary>
    /// метод ресив дамаг висит в инспекторе на коллайдере со скриптом Damageble
    /// метод прокидывается на событие OnDamage на скрипте Damageble
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    public void ReceiveDamage(float damage, Vector3 position, Vector3 normal, GameObject sender)
    {
        if (_impactPrefab != null)
        {
            Instantiate(_impactPrefab, position, Quaternion.LookRotation(normal));
        }
    }
}
