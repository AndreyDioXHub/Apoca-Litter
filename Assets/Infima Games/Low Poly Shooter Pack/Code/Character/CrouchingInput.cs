//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Crouching Input.
    /// </summary>
    public class CrouchingInput : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "References")]

        [Tooltip("The character's CharacterBehaviour component.")]
        [SerializeField, NotNull]
        private Character characterBehaviour;
        
        [Tooltip("The character's MovementBehaviour component.")]
        [SerializeField, NotNull]
        private Movement movementBehaviour;

        #endregion

        private void Start()
        {
            characterBehaviour.OnPressCrouch.AddListener(Crouch);
        }

        #region INPUT
        public void Crouch()
        {
            //Check that all our references are correctly assigned.
            if (characterBehaviour == null || movementBehaviour == null)
            {
                //ReferenceError.
                Log.ReferenceError(this, this.gameObject);

                //Return.
                return;
            }

            //Block while the cursor is unlocked.
            if (!characterBehaviour.IsCursorLocked())
                return;

            movementBehaviour.TryToggleCrouch();
        }
        /*
        /// <summary>
        /// Crouch. Calling this from the new Unity Input component will directly make the character
        /// crouch/un-crouch depending on its state.
        /// Keep in mind that this method is called from an input event, so it doesn't have any direct references.
        /// </summary>
        public void Crouch(InputAction.CallbackContext context)
        {
            //Check that all our references are correctly assigned.
            if (characterBehaviour == null || movementBehaviour == null)
            {
                //ReferenceError.
                Log.ReferenceError(this, this.gameObject);

                //Return.
                return;
            }
            
            //Block while the cursor is unlocked.
            if (!characterBehaviour.IsCursorLocked())
                return;

            //Switch.
            switch (context.phase)
            {
                //Performed.
                case InputActionPhase.Performed:
                    //TryToggleCrouch.
                    movementBehaviour.TryToggleCrouch();
                    break;
            }
        }
        */
        #endregion
    }
}