using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    public class Magazine : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "Settings")]
        
        [Tooltip("Total Ammunition.")]
        [SerializeField]
        private int ammunitionTotal = 10;

        [Title(label: "Interface")]

        [Tooltip("Interface Sprite.")]
        [SerializeField]
        private Sprite sprite;

        #endregion

        #region GETTERS
        public int GetAmmunitionTotal() => ammunitionTotal;
        public Sprite GetSprite() => sprite;

        #endregion
    }
}