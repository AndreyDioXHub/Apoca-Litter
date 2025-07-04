//Copyright 2022, Infima Games. All Rights Reserved.

using game.configuration;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{

    public class Movement : MonoBehaviour
    {

        [SerializeField]
        private PlayerNetworkResolver _resolver;
        [SerializeField]
        private float acceleration = 9.0f;
        [SerializeField]
        private float accelerationInAir = 3.0f;
        [SerializeField]
        private float deceleration = 11.0f;
        [SerializeField]
        private float speedWalking = 4.0f;
        [SerializeField]
        private float speedAiming = 3.2f;
        [SerializeField]
        private float speedCrouching = 3.5f;

        [SerializeField]
        private float speedRunning = 6.8f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float walkingMultiplierForward = 1.0f;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float walkingMultiplierSideways = 1.0f;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float walkingMultiplierBackwards = 1.0f;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float airControl = 0.8f;

        [SerializeField]
        private float gravity = 1.1f;
        [SerializeField]
        private float jumpGravity = 1.0f;
        [SerializeField]
        private float jumpForce = 100.0f;
        [SerializeField]
        private float stickToGroundForce = 0.03f;
        [SerializeField]
        private bool canCrouch = true;

        [SerializeField]
        private bool canCrouchWhileFalling = false;

        [SerializeField]
        private bool canJumpWhileCrouching = true;

        [SerializeField]
        private float crouchHeight = 1.0f;

        [SerializeField]
        private LayerMask crouchOverlapsMask;

        [SerializeField]
        private float rigidbodyPushForce = 1.0f;

        [SerializeField]
        private int _fuelMax = 100;

        [SerializeField]
        private int _fuelCur = 0;
        private int _fuelFlowRate = 1;

        private float _jetPackTickTime=0, _jetPackTickTimeCur;

        [SerializeField]
        private bool _jetPack;

        private bool _isPaused = false;

        private CharacterController controller;

        [SerializeField]
        private Character _character;

        private Weapon equippedWeapon;

        private float standingHeight;

        private Vector3 velocity;

        private bool isGrounded;

        private bool wasGrounded;

        private bool jumping;

        private bool crouching;

        public void AddFuel(int count)
        {
            _fuelCur += count;
            _fuelCur = _fuelCur > _fuelMax ? _fuelMax : _fuelCur;
        }

        protected void Awake()
        {
        }

        protected void Start()
        {

            controller = GetComponent<CharacterController>();

            standingHeight = controller.height;

            if (PauseScreen.Instance == null)
            {

            }
            else
            {
                PauseScreen.Instance.OnPaused.AddListener(PauseProcessing);
            }

            Inventory.Instance.OnWeaponSelected.AddListener(WeaponSelected);

            equippedWeapon = _character.GetInventory().GetEquipped();

            if (GameConfig.CurrentConfiguration.CurrentScene == null)
            {
                _jetPackTickTime = 2;
            }
            else
            {
                _jetPackTickTime = GameConfig.CurrentConfiguration.CurrentScene.JetPackFuelTick;
            }
        }

        public void WeaponSelected(GameObject weapon)
        {
            equippedWeapon = weapon.GetComponent<Weapon>();
        }

        private void PauseProcessing(bool isPause)
        {
            _isPaused = isPause;
        }

        protected void Update()
        {
            isGrounded = IsGrounded();
            if (isGrounded && !wasGrounded)
            {
                jumping = false;
            }
            MoveCharacter();
            wasGrounded = isGrounded;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.moveDirection.y > 0.0f && velocity.y > 0.0f)
            {
                velocity.y = 0.0f;
            }

            Rigidbody hitRigidbody = hit.rigidbody;

            if (hitRigidbody == null)
            {
                return;
            }
            Vector3 force = (hit.moveDirection + Vector3.up * 0.35f) * velocity.magnitude * rigidbodyPushForce;
            hitRigidbody.AddForceAtPosition(force, hit.point);
        }

        private void MoveCharacter()
        {

            if (PauseTimePizduk.Instance != null)
            {
                if (PauseTimePizduk.Instance.gameObject.activeSelf)
                {
                    return;
                }
            }

            if (_isPaused)
            {
                return;
            }

            if (BotsCanMove.Instance == null)
            {

            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            Vector2 frameInput = Vector3.ClampMagnitude(_character.GetInputMovement(), 1.0f);
            var desiredDirection = new Vector3(frameInput.x, 0.0f, frameInput.y);

            if (_character.IsRunning())
            {
                desiredDirection *= speedRunning;
            }
            else
            {
                if (crouching)
                {
                    desiredDirection *= speedCrouching;
                }
                else
                {
                    if (_character.IsAiming())
                    {
                        desiredDirection *= speedAiming;
                    }
                    else
                    {
                        desiredDirection *= speedWalking;
                        desiredDirection.x *= walkingMultiplierSideways;
                        desiredDirection.z *= (frameInput.y > 0 ? walkingMultiplierForward : walkingMultiplierBackwards);
                    }
                }
            } 

            desiredDirection = transform.TransformDirection(desiredDirection);

            if (equippedWeapon == null)
            {

            }
            else
            {
                desiredDirection *= equippedWeapon.GetMultiplierMovementSpeed();
            }

            if (isGrounded == false)
            {
                if (wasGrounded && !jumping)
                {
                    velocity.y = 0.0f;
                }

                velocity += desiredDirection * accelerationInAir * airControl * Time.deltaTime;
                velocity.y -= (velocity.y >= 0 ? jumpGravity : gravity) * Time.deltaTime;
            }
            else if(!jumping)
            {
                velocity = Vector3.Lerp(velocity, new Vector3(desiredDirection.x, velocity.y, desiredDirection.z), Time.deltaTime * (desiredDirection.sqrMagnitude > 0.0f ? acceleration : deceleration));
            }

            if (_fuelCur <= 0)
            {
                _jetPack = false;
            }

            if (_jetPack)
            {
                velocity = new Vector3(velocity.x, 1, velocity.z);

                _jetPackTickTimeCur += Time.deltaTime;

                if(_jetPackTickTimeCur > _jetPackTickTime)
                {
                    _jetPackTickTimeCur = 0;
                    _fuelCur -= _fuelFlowRate;
                    _fuelCur= _fuelCur < 0 ? 0 : _fuelCur;
                }
            }
            else
            {
                _jetPackTickTimeCur = 0;
            }

            Vector3 applied = velocity * Time.deltaTime;
            if (controller.isGrounded && !jumping && _fuelCur<=0)
                applied.y = -stickToGroundForce;
            controller.Move(applied);

        }

        public bool CanCrouch(bool newCrouching)
        {
            if (canCrouch == false)
                return false;
            if (isGrounded == false && canCrouchWhileFalling == false)
                return false;
            if (newCrouching)
                return true;
            Vector3 sphereLocation = transform.position + Vector3.up * standingHeight;
            return (Physics.OverlapSphere(sphereLocation, controller.radius, crouchOverlapsMask).Length == 0);
        }
        public bool IsCrouching() => crouching;

        public void Jump()
        {
            if (crouching && !canJumpWhileCrouching)
                return;
            if (!isGrounded)
                return;

            if (_fuelCur > 0)
            {
                return;
            }
            jumping = true;
            velocity = new Vector3(velocity.x, Mathf.Sqrt(2.0f * jumpForce * jumpGravity), velocity.z);
        }

        public void Jetpack(bool jetpack)
        {
            if(_fuelCur<=0)
            {
                _jetPack = false;
                return;
            }

            if (jetpack)
            {
                velocity = Vector3.zero;
                _jetPack = true;
            }
            else
            {
                _jetPack = false;
            }
        }

        public void Crouch(bool newCrouching)
        {
            crouching = newCrouching;
            _resolver.IsCrouch = crouching;
            controller.height = crouching ? crouchHeight : standingHeight;
            controller.center = controller.height / 2.0f * Vector3.up;
        }

        public void TryToggleCrouch()
        {
            if (crouching == false && CanCrouch(true))
            {
                Crouch(true);
            }

            else if (crouching)
            {
                StartCoroutine(nameof(TryUncrouch));
            }
        }

        private IEnumerator TryUncrouch()
        {
            yield return new WaitUntil(() => CanCrouch(false));
            Crouch(false);
        }

        public float GetMultiplierForward() => walkingMultiplierForward;
        public float GetMultiplierSideways() => walkingMultiplierSideways;
        public float GetMultiplierBackwards() => walkingMultiplierBackwards;
        public Vector3 GetVelocity() => controller.velocity;
        public bool IsGrounded()
        {
            if (controller == null)
            {
                //Debug.Log("IsGrounded controller is null");
                return true;
            }

            return controller.isGrounded;
        }
    }
}