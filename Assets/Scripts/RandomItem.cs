using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItem : MonoBehaviour
{
    [SerializeField]
    private float _destroyTime = 10;
    [SerializeField]
    private GameObject _impactPrefab;
    [SerializeField]
    private GameObject _main;
    [SerializeField]
    private List<GameObject> _items = new List<GameObject>();
    [SerializeField]
    private GameObject _item;
    [SerializeField]
    private bool _destroyAfterSpawn;

    void Start()
    {
        string randomName = GameWeaponStarter.Instance.RandomAvalebleName();

        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].name.Equals(randomName))
            {
                _item = _items[i];
                _item.SetActive(true);
                i = _items.Count;
            }
        }

        if (_destroyAfterSpawn)
        {
            if (Random.Range(0, 2) == 0 || _item == null)
            {
                Destroy(_main);
            }
        }
        /*
        if (_destroyTime > 0)
        {
            Destroy(_main, _destroyTime);
        }*/
    }

    void Update()
    {
        /*
        if (Character.Instance == null)
        {

        }
        else
        {
            if (Vector3.Distance(transform.position, Character.Instance.transform.position) > 15)
            {
                Destroy(_main);
            }
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && _item != null)
        {
            var player = Character.Instance; //other.GetComponent<CharacterBehaviour>();

            //CoinsManager.Instance.AddCoins(50);

            switch (_item.name)
            {
                case "pistol":
                    player.AddAmmo(AmmoType.pistol, Random.Range(10, 20));
                    break;
                case "grenade":
                    player.AddAmmo(AmmoType.grenade, Random.Range(1, 3));
                    break;
                case "grenadeLaunched":
                    player.AddAmmo(AmmoType.grenadeLaunched, Random.Range(1, 3));
                    break;
                case "rocket":
                    player.AddAmmo(AmmoType.rocket, 1);
                    break;
                case "rifle":
                    player.AddAmmo(AmmoType.rifle, Random.Range(15, 30));
                    break;
                case "smg":
                    player.AddAmmo(AmmoType.smg, Random.Range(20, 40));
                    break;
                case "shotgun":
                    player.AddAmmo(AmmoType.shotgun, Random.Range(4, 8));
                    break;
                case "sniperRifle":
                    player.AddAmmo(AmmoType.sniperRifle, Random.Range(5, 10));
                    break;
                case "life":
                    Debug.Log("Хилка сломана");
                    //player.ReceiveDamage(-20, player.transform.position, Vector3.up, null);
                    break;
                default:
                    break;
            }

            Instantiate(_impactPrefab, transform.position, Quaternion.identity);
            Destroy(_main);
        }    
    }

    private void OnDestroy()
    {
    }
}
