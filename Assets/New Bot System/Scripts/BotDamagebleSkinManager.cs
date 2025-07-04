using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewBotSystem
{
    public class BotDamagebleSkinManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _excludedCollidersObjects = new List<GameObject>();

        private void Awake()
        {
            FindExcludedCollidersObjects();
            //GetComponentInChildren<BotDamage>().SetExcludedCollidersObjects(_excludedCollidersObjects);
        }

        void Start()
        {

        }

        void Update()
        {

        }

        [ContextMenu("Find Excluded Colliders Objects")]
        public void FindExcludedCollidersObjects()
        {
            var excludedCollidersObjects = GetComponentsInChildren<Damageble>();

            foreach(var item in excludedCollidersObjects)
            {
                _excludedCollidersObjects.Add(item.gameObject);   
            }
        }
    }
}
