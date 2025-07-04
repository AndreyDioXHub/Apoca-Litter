//Copyright 2022, Infima Games. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using InfimaGames.LowPolyShooterPack.Interface;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using game.configuration;
using NewBotSystem;
using System.Linq;

namespace InfimaGames.LowPolyShooterPack
{
    [RequireComponent(typeof(CharacterKinematics))]
    public sealed class Character : MonoBehaviour
    {

        public static Character Instance => instance;

        private static Character instance = null;

        public UnityEvent OnDie = new UnityEvent();
        public UnityEvent<float> OnDamageDeltaRecived = new UnityEvent<float>();
        public UnityEvent<float, float> OnDamageRecived = new UnityEvent<float, float>();//текущее здоровье против максимального
        public UnityEvent<float, float, string> OnFire = new UnityEvent<float, float, string>();//текущее патроны против максимального и иконка
        //public UnityEvent<bool> OnAim = new UnityEvent<bool>();

        //public UnityEvent OnReload= new UnityEvent();
        public UnityEvent OnPressEsc = new UnityEvent();
        public UnityEvent OnPressTab = new UnityEvent();
        public UnityEvent<DealerZone> OnInsideDealer = new UnityEvent<DealerZone>();
        public UnityEvent OnPressChat = new UnityEvent();
        public UnityEvent OnPressCrouch = new UnityEvent();

        public UnityEvent OnFirstTimeWeaponSettings = new UnityEvent();

        #region FIELDS SERIALIZED

        public PlayerNetworkResolver Resolver => _resolver;

        public GameObject DepthCamera;
        [SerializeField]
        private int _equipIndex;
        [SerializeField]
        private PlayerNetworkResolver _resolver;
        [SerializeField]
        private CameraLook _cameraLook;
        [SerializeField]
        private float _hitPointsMax = 100;
        [SerializeField]
        private float _hitPoints;
        [SerializeField]
        private bool _isMayDie;
        private bool _haveDamage;
        private bool _needHealPlayer;

        [SerializeField]
        private bool _aimHold = true;

        [SerializeField]
        private LayerMask _playerMask;
        [SerializeField]
        private float _meleeHitDistance = 1;
        [SerializeField]
        private float _offcet = 0.5f;

        [SerializeField]
        private LowerWeapon lowerWeapon;
        [SerializeField]
        private Inventory inventory;
        [SerializeField]
        private bool grenadesUnlimited;
        [SerializeField]
        private int grenadeTotal = 10;
        [SerializeField]
        private float grenadeSpawnOffset = 1.0f;
        [SerializeField]
        private GameObject grenadePrefab;
        [SerializeField]
        private GameObject knife;
        [SerializeField]
        private Camera cameraWorld;
        [SerializeField]
        private Camera cameraDepth;
        [SerializeField]
        private float dampTimeTurning = 0.4f;
        [SerializeField]
        private float dampTimeLocomotion = 0.15f;
        [SerializeField]
        private float dampTimeAiming = 0.3f;
        [SerializeField]
        private float runningInterpolationSpeed = 12.0f;
        [SerializeField]
        private float aimingSpeedMultiplier = 1.0f;
        [SerializeField]
        private Transform boneWeapon;
        [SerializeField]
        private Animator characterAnimator;

        [SerializeField]
        private bool enableWeaponSway = true;

        [SerializeField]
        private float weaponSwaySmoothValueInput = 8.0f;

        [SerializeField]
        private float fieldOfView = 100.0f;
        [SerializeField]
        private float fieldOfViewRunningMultiplier = 1.05f;
        [SerializeField]
        private float fieldOfViewWeapon = 55.0f;
        [SerializeField]
        private AudioClip[] audioClipsMelee;
        [SerializeField]
        private AudioClip[] audioClipsGrenadeThrow;

        private float time = 2, timecur;

        private bool _isPaused = false;

        #endregion

        #region FIELDS

        private bool aiming;
        private bool wasAiming;
        private bool running;
        [SerializeField]
        private bool holstered;
        private float lastShotTime;
        private int layerOverlay;
        private int layerHolster;
        private int layerActions;

        private Movement movementBehaviour;
        private Weapon equippedWeapon;
        private WeaponAttachmentManager weaponAttachmentManager;
        private Scope equippedWeaponScope;
        private Magazine equippedWeaponMagazine;
        private bool reloading;
        private bool inspecting;
        private bool throwingGrenade;
        private bool meleeing;
        private Vector3 swayLocation;
        private Vector3 swayRotation;
        private bool holstering;
        private float aimingAlpha;
        private float crouchingAlpha;
        private float runningAlpha;
        private Vector2 axisLook;
        private Vector2 axisLookSmooth;
        private Vector2 axisMovement;
        private Vector2 axisMovementSmooth;
        private bool bolting;
        private int grenadeCount;
        private bool holdingButtonAim;
        private bool holdingButtonRun;
        private bool holdingButtonFire;
        private bool tutorialTextVisible;
        [SerializeField]
        private bool _isNeedLockCursor;
        [SerializeField]
        private bool cursorLocked;
        private int shotsFired;
        private float aimStartTime;


        #endregion

        #region CONSTANTS

        //TODO: Get rid of all of these, and move them to the AHashes file.
        private static readonly int HashAimingAlpha = Animator.StringToHash("Aiming");
        private static readonly int HashBoltAction = Animator.StringToHash("Bolt Action");
        private static readonly int HashMovement = Animator.StringToHash("Movement");
        private static readonly int HashLeaning = Animator.StringToHash("Leaning");
        private static readonly int HashAimingSpeedMultiplier = Animator.StringToHash("Aiming Speed Multiplier");
        private static readonly int HashTurning = Animator.StringToHash("Turning");
        private static readonly int HashHorizontal = Animator.StringToHash("Horizontal");
        private static readonly int HashVertical = Animator.StringToHash("Vertical");

        private static readonly int HashPlayRateLocomotionForward = Animator.StringToHash("Play Rate Locomotion Forward");
        private static readonly int HashPlayRateLocomotionSideways = Animator.StringToHash("Play Rate Locomotion Sideways");
        private static readonly int HashPlayRateLocomotionBackwards = Animator.StringToHash("Play Rate Locomotion Backwards");
        private static readonly int HashAlphaActionOffset = Animator.StringToHash("Alpha Action Offset");

        #endregion

        #region UNITY

        protected void Awake()
        {
        }

        public void Init()
        {
        }

        protected void Start()
        {
            if (instance == null)
            {
                if (_resolver.IsLocalPlayer)
                {
                    //Debug.Log("instance IsLocalPlayer");
                    instance = this;
                }
            }

            #region Lock Cursor
            if (_isNeedLockCursor)
            {
                //Always make sure that our cursor is locked when the game starts!
                cursorLocked = true;
                //Update the cursor's state.
            }
            else
            {
                cursorLocked = true;
            }

            UpdateCursorState();
            #endregion


            if (PauseScreen.Instance == null)
            {
                //Debug.Log("Character PauseScreen.Instance == null");
            }
            else
            {
                PauseScreen.Instance.OnPaused.AddListener(UpdateCursorState);
            }

            inventory.OnWeaponSelected.AddListener(WeaponSelectedOnStart);
            inventory.OnWeaponSelected.AddListener(WeaponSelected);
            inventory.OnWeaponAttachmentManageкSelected.AddListener(WeaponAttachmentManageкSelected);
            //Cache the movement behaviour.
            movementBehaviour = GetComponent<Movement>();

            inventory.Init(this);

            _resolver.OnReceiveDamage.AddListener(ReceiveDamage);

            Latestart();
            LateSubscribe();
        }

        public async void NewRoundStarted()
        {
            //Debug.Log("NewRoundStarted");
            await UniTask.WaitForSeconds(1);
            //Equip(0);
        }

        public async void LateSubscribe()
        {
            while (EventsBus.Instance == null)
            {
                await UniTask.Yield();
            }

            EventsBus.Instance.OnExplosionCenter.AddListener(ExplosionProcess);

            while (PauseScreen.Instance == null)
            {
                await UniTask.Yield();
            }

            PauseScreen.Instance.OnPaused.AddListener(Paused);

            inventory.OnAmmoVestIsEmpty.AddListener(OnTryMelee);
        }

        public void Paused(bool isPaused)
        {
            holdingButtonFire = false;
        }

        public void ExplosionProcess(Vector3 center)
        {
            float damage = Vector3.Distance(transform.position, center);
            //Debug.Log($"{damage}");
            damage = damage / 5;
            damage = damage > 1 ? 1 : damage;
            damage = (1 - damage) * 95;
            //Debug.Log($"{damage}");

            ReceiveDamage(damage, 0, 0);
        }

        public async void Latestart()
        {
            await UniTask.Yield();
            UpdateCursorState(PauseScreen.Instance.IsPaused);

            int equippedIndex = PlayerPrefs.GetInt(PlayerPrefsConsts.equippedIndex, 0);
            //Debug.Log($"equippedIndex {equippedIndex}");

            Equip(equippedIndex);
            await UniTask.Yield();
            OnFirstTimeWeaponSettings?.Invoke();
        }

        public void WeaponAttachmentManageкSelected(WeaponAttachmentManager manager)
        {
            weaponAttachmentManager = manager;
            equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();
            equippedWeaponMagazine = weaponAttachmentManager.GetEquippedMagazine();
        }

        public void WeaponSelected(GameObject weapon)
        {
            //Debug.Log($"WeaponSelected weapon {weapon}");
            equippedWeapon = weapon.GetComponent<Weapon>();
            RefreshWeaponSetup();
        }

        public void WeaponSelectedOnStart(GameObject weapon)
        {
            //Debug.Log("WeaponSelectedOnStart");

            WeaponSelected(weapon);

            _hitPoints = _hitPointsMax;

            grenadeTotal = inventory.GetAmmoCount(AmmoType.grenade);
            grenadeCount = grenadeTotal;

            CharacterValues.UpdateValue(CharacterValueKey.GrenadeCountCurent, new ValueCurentMax(grenadeCount, grenadeTotal));

            if (knife != null)
            {
                knife.SetActive(false);
            }

            layerHolster = characterAnimator.GetLayerIndex("Layer Holster");
            layerActions = characterAnimator.GetLayerIndex("Layer Actions");
            layerOverlay = characterAnimator.GetLayerIndex("Layer Overlay");

            UpdateCursorState();

            inventory.RemoveAmmo(equippedWeapon.GetAmmoType(), 0);
            inventory.OnWeaponSelected.RemoveListener(WeaponSelectedOnStart);
        }

        public void SetNeedHealPlayer()
        {
            _needHealPlayer = true;
        }

        protected void Update()
        {
            if (equippedWeapon == null)
            {
                //Debug.Log("Character Update equippedWeapon = null");
                return;
            }

            if (_needHealPlayer)
            {
                if (_haveDamage)
                {
                    timecur += Time.deltaTime;

                    if (timecur > time)
                    {
                        timecur = 0;
                        _haveDamage = false;
                    }
                }
                else
                {
                    if (_hitPoints == _hitPointsMax)
                    {
                        _needHealPlayer = false;
                    }
                    else
                    {
                        ReceiveDamage(-1, _resolver.PlayerID, _resolver.PlayerID);
                    }
                }
            }

            //Match Aim.

            if (_aimHold)
            {
                aiming = holdingButtonAim && CanAim();
            }
            else
            {

            }

            //aiming = true;
            //Match Run.


            //running = holdingButtonRun && CanRun();

            running = CanRun();
            _resolver.IsRun = running;

            //

            //Check if we're aiming.
            switch (aiming)
            {
                //Just Started.
                case true when !wasAiming:
                    equippedWeaponScope.OnAim();
                    break;
                //Just Stopped.
                case false when wasAiming:
                    equippedWeaponScope.OnAimStop();
                    break;
            }

            if (holdingButtonFire)
            {
                if (CanPlayAnimationFire() && equippedWeapon.HasAmmunition() && equippedWeapon.IsAutomatic())
                {
                    if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                    {
                        Fire();

                        if (equippedWeapon.WeaponName.Equals("Jackhammer"))
                        {
                            _resolver.PlayerEmtyDrillOnServet(true);
                        }
                        else
                        {

                        }
                    }

                }
                else
                {
                    shotsFired = 0;
                }
            }
            else
            {

                if (equippedWeapon.WeaponName.Equals("Jackhammer"))
                {
                    _resolver.PlayerEmtyDrillOnServet(false);
                }
                else
                {

                }
            }

            if (enableWeaponSway)
            {
                CalculateSway();
            }

            //Interpolate Movement Axis.
            axisMovementSmooth = Vector2.Lerp(axisMovementSmooth, axisMovement, Time.deltaTime * weaponSwaySmoothValueInput);
            //Interpolate Look Axis.
            axisLookSmooth = Vector2.Lerp(axisLookSmooth, axisLook, Time.deltaTime * weaponSwaySmoothValueInput);

            //Update Animator.
            UpdateAnimator();

            //Update Aiming Alpha. We need to get this here because we're using the Animator to interpolate the aiming value.
            aimingAlpha = characterAnimator.GetFloat(HashAimingAlpha);

            //Interpolate the crouching alpha. We do this here as a quick and dirty shortcut, but there's definitely better ways to do this.
            crouchingAlpha = Mathf.Lerp(crouchingAlpha, movementBehaviour.IsCrouching() ? 1.0f : 0.0f, Time.deltaTime * 12.0f);
            //Interpolate the running alpha. We do this here as a quick and dirty shortcut, but there's definitely better ways to do this.
            runningAlpha = Mathf.Lerp(runningAlpha, running ? 1.0f : 0.0f, Time.deltaTime * runningInterpolationSpeed);

            //Running Field Of View Multiplier.
            float runningFieldOfView = Mathf.Lerp(1.0f, fieldOfViewRunningMultiplier, runningAlpha);

            //Interpolate the world camera's field of view based on whether we are aiming or not.
            cameraWorld.fieldOfView = Mathf.Lerp(fieldOfView, fieldOfView * equippedWeapon.GetFieldOfViewMultiplierAim(), aimingAlpha) * runningFieldOfView;
            //Interpolate the depth camera's field of view based on whether we are aiming or not.
            cameraDepth.fieldOfView = Mathf.Lerp(fieldOfViewWeapon, fieldOfViewWeapon * equippedWeapon.GetFieldOfViewMultiplierAimWeapon(), aimingAlpha);

            //Save Aiming Value.
            wasAiming = aiming;
        }

        protected void LateUpdate()
        {
            //Ignore if we don't have a weapon bone assigned.
            if (boneWeapon == null)
                return;

            //We need a weapon for this!
            if (equippedWeapon == null)
                return;

            //Weapons without a scope should not be a thing! Ironsights are a scope too!
            if (equippedWeaponScope == null)
            {
                return;
            }

            //Get the weapon offsets.
            Offsets weaponOffsets = equippedWeapon.GetWeaponOffsets();

            //Frame Location Local.
            Vector3 frameLocationLocal = swayLocation;
            //Offset Location.
            frameLocationLocal += Vector3.Lerp(weaponOffsets.StandingLocation, weaponOffsets.AimingLocation, aimingAlpha);
            //Scope Aiming Location.
            frameLocationLocal += Vector3.Lerp(default, equippedWeaponScope.GetOffsetAimingLocation(), aimingAlpha);
            //Crouching Location.
            frameLocationLocal += Vector3.Lerp(default, weaponOffsets.CrouchingLocation, crouchingAlpha * (1 - aimingAlpha));
            //Running Location.
            frameLocationLocal += Vector3.Lerp(default, weaponOffsets.RunningLocation, runningAlpha * (1 - aimingAlpha));
            //Action Offset Location. This is a helping value to make actions like throwing a grenade different per-weapon.
            frameLocationLocal += Vector3.Lerp(weaponOffsets.ActionLocation * characterAnimator.GetFloat(HashAlphaActionOffset), default, aimingAlpha);

            //Frame Rotation Local.
            Vector3 frameRotationLocal = swayRotation;
            //Offset Rotation.
            frameRotationLocal += Vector3.Lerp(weaponOffsets.StandingRotation, weaponOffsets.AimingRotation, aimingAlpha);
            //Scope Aiming Rotation.
            frameRotationLocal += Vector3.Lerp(default, equippedWeaponScope.GetOffsetAimingRotation(), aimingAlpha);
            //Crouching Rotation.
            frameRotationLocal += Vector3.Lerp(default, weaponOffsets.CrouchingRotation, crouchingAlpha * (1 - aimingAlpha));
            //Running Rotation.
            frameRotationLocal += Vector3.Lerp(default, weaponOffsets.RunningRotation, runningAlpha * (1 - aimingAlpha));
            //Action Offset Rotation. This is a helping value to make actions like throwing a grenade different per-weapon.
            frameRotationLocal += Vector3.Lerp(weaponOffsets.ActionRotation * characterAnimator.GetFloat(HashAlphaActionOffset), default, aimingAlpha);

            #region Automatic Aim Offsets

            // Transform socketScopeCorrected = equippedWeaponScope.transform.GetChild(0).GetChild(0).GetChild(0);
            // Transform socketScopes = equippedWeaponScope.transform.parent.parent;
            //
            // Log.wtf(equippedWeaponScope.transform.parent.parent.parent.localPosition);
            // Vector3 localPosition = equippedWeaponScope.transform.GetChild(0).GetChild(0).localPosition;
            // Log.wtf(localPosition);
            // boneWeapon.localPosition -= Vector3.Lerp(default, localPosition, aimingAlpha);

            #endregion

            //Add to the weapon location.
            boneWeapon.localPosition += frameLocationLocal;
            //Add to the weapon rotation.
            boneWeapon.localEulerAngles += frameRotationLocal;
        }

        #endregion

        #region GETTERS
        public int GetShotsFired() => shotsFired;

        public bool IsLowered()
        {
            //Weapons are never lowered if we don't even have a LowerWeapon component.
            if (lowerWeapon == null)
            {
                return false;
            }

            //Return.
            return lowerWeapon.IsLowered();
        }

        public Camera GetCameraWorld() => cameraWorld;
        public Camera GetCameraDepth() => cameraDepth;

        private (Vector3 location, Vector3 rotation) GetSwayLook(Sway sway)
        {
            //Horizontal Axis.
            float horizontalAxis = Mathf.Clamp(axisLookSmooth.x, -1.0f, 1.0f);
            //Vertical Axis.
            float verticalAxis = Mathf.Clamp(axisLookSmooth.y, -1.0f, 1.0f);

            //Horizontal Axis Location.
            Vector3 horizontalLocation = horizontalAxis * sway.Look.Location.Horizontal;
            //Horizontal Axis Rotation.
            Vector3 horizontalRotation = horizontalAxis * sway.Look.Rotation.Horizontal;

            //Vertical Axis Location.
            Vector3 verticalLocation = verticalAxis * sway.Look.Location.Vertical;
            //Vertical Axis Rotation.
            Vector3 verticalRotation = verticalAxis * sway.Look.Rotation.Vertical;

            //Return.
            return (horizontalLocation + verticalLocation, horizontalRotation + verticalRotation);
        }

        private (Vector3 location, Vector3 rotation) GetSwayMovement(Sway sway)
        {
            //Horizontal Axis.
            float horizontalAxis = Mathf.Clamp(axisMovementSmooth.x, -1.0f, 1.0f);
            //Vertical Axis.
            float verticalAxis = Mathf.Clamp(axisMovementSmooth.y, -1.0f, 1.0f);

            //Horizontal Axis Location.
            Vector3 horizontalLocation = horizontalAxis * sway.Movement.Location.Horizontal;
            //Horizontal Axis Rotation.
            Vector3 horizontalRotation = horizontalAxis * sway.Movement.Rotation.Horizontal;

            //Vertical Axis Location.
            Vector3 verticalLocation = verticalAxis * sway.Movement.Location.Vertical;
            //Vertical Axis Rotation.
            Vector3 verticalRotation = verticalAxis * sway.Movement.Rotation.Vertical;

            //Return.
            return (horizontalLocation + verticalLocation, horizontalRotation + verticalRotation);
        }

        public Inventory GetInventory() => inventory;
        public int GetGrenadesCurrent() => grenadeCount;
        public int GetGrenadesTotal() => grenadeTotal;
        public bool IsRunning() => running;
        public bool IsHolstered() => holstered;
        public bool IsCrouching() => movementBehaviour.IsCrouching();
        public bool IsReloading() => reloading;
        public bool IsThrowingGrenade() => throwingGrenade;
        public bool IsMeleeing() => meleeing;
        public bool IsAiming() => aiming;
        public bool IsCursorLocked() => cursorLocked;
        public bool IsTutorialTextVisible() => tutorialTextVisible;
        public Vector2 GetInputMovement() => axisMovement;
        public Vector2 GetInputLook() => axisLook;
        public AudioClip[] GetAudioClipsGrenadeThrow() => audioClipsGrenadeThrow;
        public AudioClip[] GetAudioClipsMelee() => audioClipsMelee;
        public bool IsInspecting() => inspecting;
        public bool IsHoldingButtonFire() => holdingButtonFire;

        #endregion

        #region METHODS

        private void UpdateAnimator()
        {
            #region Reload Stop

            //Check if we're currently reloading cycled.
            const string boolNameReloading = "Reloading";

            if (characterAnimator.GetBool(boolNameReloading))
            {
                //If we only have one more bullet to reload, then we can change the boolean already.
                if (equippedWeapon.GetAmmunitionTotal() - equippedWeapon.GetAmmunitionCurrent() < 1)
                {
                    //Update the character animator.
                    characterAnimator.SetBool(boolNameReloading, false);
                    //Update the weapon animator.
                    equippedWeapon.GetAnimator().SetBool(boolNameReloading, false);
                }
            }

            #endregion

            //Leaning. Affects how much the character should apply of the leaning additive animation.
            float leaningValue = Mathf.Clamp01(axisMovement.y);
            characterAnimator.SetFloat(HashLeaning, leaningValue, 0.5f, Time.deltaTime);

            //Movement Value. This value affects absolute movement. Aiming movement uses this, as opposed to per-axis movement.
            float movementValue = Mathf.Clamp01(Mathf.Abs(axisMovement.x) + Mathf.Abs(axisMovement.y));
            characterAnimator.SetFloat(HashMovement, movementValue, dampTimeLocomotion, Time.deltaTime);

            //Aiming Speed Multiplier.
            characterAnimator.SetFloat(HashAimingSpeedMultiplier, aimingSpeedMultiplier);

            //Turning Value. This determines how much of the turning animation to play based on our current look rotation.
            characterAnimator.SetFloat(HashTurning, Mathf.Abs(axisLook.x), dampTimeTurning, Time.deltaTime);

            //Horizontal Movement Float.
            characterAnimator.SetFloat(HashHorizontal, axisMovementSmooth.x, dampTimeLocomotion, Time.deltaTime);
            //Vertical Movement Float.
            characterAnimator.SetFloat(HashVertical, axisMovementSmooth.y, dampTimeLocomotion, Time.deltaTime);

            //Update the aiming value, but use interpolation. This makes sure that things like firing can transition properly.
            characterAnimator.SetFloat(HashAimingAlpha, Convert.ToSingle(aiming), dampTimeAiming, Time.deltaTime);

            //Set the locomotion play rate. This basically stops movement from happening while in the air.
            const string playRateLocomotionBool = "Play Rate Locomotion";
            characterAnimator.SetFloat(playRateLocomotionBool, movementBehaviour.IsGrounded() ? 1.0f : 0.0f, 0.2f, Time.deltaTime);

            #region Movement Play Rates

            //Update Forward Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
            characterAnimator.SetFloat(HashPlayRateLocomotionForward, movementBehaviour.GetMultiplierForward(), 0.2f, Time.deltaTime);
            //Update Sideways Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
            characterAnimator.SetFloat(HashPlayRateLocomotionSideways, movementBehaviour.GetMultiplierSideways(), 0.2f, Time.deltaTime);
            //Update Backwards Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
            characterAnimator.SetFloat(HashPlayRateLocomotionBackwards, movementBehaviour.GetMultiplierBackwards(), 0.2f, Time.deltaTime);

            #endregion

            //Update Animator Aiming.
            const string boolNameAim = "Aim";
            characterAnimator.SetBool(boolNameAim, aiming);

            //Update Animator Running.
            const string boolNameRun = "Running";
            characterAnimator.SetBool(boolNameRun, running);

            //Update Animator Crouching.
            const string boolNameCrouch = "Crouching";
            characterAnimator.SetBool(boolNameCrouch, movementBehaviour.IsCrouching());
        }

        private void Inspect()
        {
            //State.
            inspecting = true;
            //Play.
            characterAnimator.CrossFade("Inspect", 0.0f, layerActions, 0);
        }

        private void CalculateSway()
        {
            //We need a scope!
            if (equippedWeaponScope == null)
            {
                return;
            }

            //Weapon Sway Values.
            Sway sway = equippedWeapon.GetSway();
            //Weapon Sway Smooth Value.
            float swaySmoothValue = equippedWeapon.GetSwaySmoothValue();

            (Vector3 location, Vector3 rotation) swayLookStanding = GetSwayLook(sway);
            (Vector3 location, Vector3 rotation) swayLookAiming = GetSwayLook(equippedWeaponScope.GetSwayAiming());

            (Vector3 location, Vector3 rotation) swayMovementStanding = GetSwayMovement(sway);
            (Vector3 location, Vector3 rotation) swayMovementAiming = GetSwayMovement(equippedWeaponScope.GetSwayAiming());

            //Get Look Sway.
            (Vector3 location, Vector3 rotation) swayLook = default;
            swayLook.location = Vector3.Lerp(swayLookStanding.location, swayLookAiming.location, aimingAlpha);
            swayLook.rotation = Vector3.Lerp(swayLookStanding.rotation, swayLookAiming.rotation, aimingAlpha);

            //Get Movement Sway.
            (Vector3 location, Vector3 rotation) swayMovement = default;
            swayMovement.location = Vector3.Lerp(swayMovementStanding.location, swayMovementAiming.location, aimingAlpha);
            swayMovement.rotation = Vector3.Lerp(swayMovementStanding.rotation, swayMovementAiming.rotation, aimingAlpha);

            //Calculate Sway Location.
            Vector3 frameLocation = swayLook.location + swayMovement.location;
            //Interpolate.
            swayLocation = Vector3.LerpUnclamped(swayLocation, frameLocation, Time.deltaTime * swaySmoothValue);

            //Calculate Sway Rotation.
            Vector3 frameRotation = swayLook.rotation + swayMovement.rotation;
            //Interpolate.
            swayRotation = Vector3.LerpUnclamped(swayRotation, frameRotation, Time.deltaTime * swaySmoothValue);
        }

        private async void Fire()
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            shotsFired++;
            lastShotTime = Time.time;

            while (equippedWeaponScope == null)
            {
                equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();
                await UniTask.Yield(); // Ожидание следующего кадра
            }

            if (equippedWeaponScope != null)
            {
                _resolver.OnFireOnServer();
                //Debug.Log("Fire Fire");
                equippedWeapon.Fire(aiming ? equippedWeaponScope.GetMultiplierSpread() : 1.0f);
            }

            const string stateName = "Fire";
            characterAnimator.CrossFade(stateName, 0.05f, layerOverlay, 0);

            //Play bolt actioning animation if needed, and if we have ammunition. We don't play this for the last shot.
            if (equippedWeapon.IsBoltAction() && equippedWeapon.HasAmmunition())
            {
                UpdateBolt(true);
            }

            if (!equippedWeapon.HasAmmunition() && equippedWeapon.GetAutomaticallyReloadOnEmpty())
            {
                if (inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) == 0)
                {
                    OnTryInventoryNext();
                }
                else
                {
                    StartCoroutine(nameof(TryReloadAutomatic));
                }
            }
        }

        private void PlayReloadAnimation()
        {
            #region Animation

            //Get the name of the animation state to play, which depends on weapon settings, and ammunition!
            string stateName = equippedWeapon.HasCycledReload() ? "Reload Open" :
                (equippedWeapon.HasAmmunition() ? "Reload" : "Reload Empty");

            //Play the animation state!
            characterAnimator.Play(stateName, layerActions, 0.0f);

            #endregion

            //Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
            const string boolName = "Reloading";
            reloading = true;
            characterAnimator.SetBool(boolName, reloading);

            //Reload.
            equippedWeapon.Reload();
            _resolver.OnReloadOnServer();
        }

        private IEnumerator TryReloadAutomatic()
        {
            yield return new WaitForSeconds(equippedWeapon.GetAutomaticallyReloadOnEmptyDelay());
            PlayReloadAnimation();
        }

        [ContextMenu("Equip")]
        public void Equip()
        {
            Equip(_equipIndex);
        }

        public void EquipbuyedWeapon1()
        {
            Equip(inventory.GetBuyedWeapon1Index());
        }

        public async void Equip(int index)
        {
            //Inventory.Instance.OnWeaponChanged?.Invoke(Inventory.Instance.GetEquippedIndex(), index);
            //_resolver.OnWeaponChangedOnServer(inventory.GetEquippedIndex(), index);
            _resolver.OnWeaponChangedOnServer(inventory.GetEquippedIndex(), index);

            if (!holstered)
            {
                holstering = true;
                SetHolstered(true);

                while (holstering)
                {
                    await UniTask.Yield();
                }
            }

            SetHolstered(false);
            characterAnimator.Play("Unholster", layerHolster, 0);

            inventory.Equip(index);
            _resolver.CurrentWeaponIndex = index;

            /*
            await UniTask.Yield();

            RefreshWeaponSetup();*/
        }

        private async void EquipSpawnGun()
        {
            if (!holstered)
            {
                holstering = true;

                SetHolstered(true);

                while (holstering)
                {
                    await UniTask.Yield();
                }
            }

            SetHolstered(false);
            characterAnimator.Play("Unholster", layerHolster, 0);

            inventory.EquipSpawnGun();
            /*
            await UniTask.Yield();

            RefreshWeaponSetup();*/
        }

        private void RefreshWeaponSetup()
        {
            characterAnimator.runtimeAnimatorController = equippedWeapon.GetAnimatorController();
        }

        private void FireEmpty()
        {
            if (inventory.AmmoVestIsEmpty())
            {
                Debug.Log("inventory Ammo Vest Is Empty");
            }

            lastShotTime = Time.time;
            //Play.
            characterAnimator.CrossFade("Fire Empty", 0.05f, layerOverlay, 0);
        }

        public void UpdateCursorState(bool state)
        {
            _isPaused = state;

            if (_isPaused)
            {
                axisMovement = Vector2.zero;
            }

            cursorLocked = !state;

            if (_isNeedLockCursor)
            {
                //Update cursor visibility.
                Cursor.visible = !cursorLocked;
                //Update cursor lock state.
                Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
                /*
                if (cursorLocked && _cameraLook != null)
                {
                    _cameraLook.UpdateSencivity();
                }*/
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void UpdateCursorState()
        {

            if (_isNeedLockCursor)
            {
                //Update cursor visibility.
                Cursor.visible = !cursorLocked;
                //Update cursor lock state.
                Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
                /*
                if (cursorLocked && _cameraLook != null)
                {
                    _cameraLook.UpdateSencivity();
                }*/
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void PlayGrenadeThrow()
        {
            //Start State.
            throwingGrenade = true;

            //Play Normal.
            characterAnimator.CrossFade("Grenade Throw", 0.15f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Left"), 0.0f);

            //Play Additive.
            characterAnimator.CrossFade("Grenade Throw", 0.05f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Right"), 0.0f);
        }

        private void PlayMelee()
        {
            meleeing = true;
            characterAnimator.CrossFade("Knife Attack", 0.05f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Left"), 0.0f);
            characterAnimator.CrossFade("Knife Attack", 0.05f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Right"), 0.0f);


            RaycastHit[] hits = Physics.BoxCastAll(transform.position + transform.forward * _offcet,
                Vector3.one, transform.forward, transform.rotation,
                _meleeHitDistance, _playerMask, QueryTriggerInteraction.Collide);

            float damage = 50;

            if (hits.Length > 0)
            {
                if (hits[0].transform.gameObject.TryGetComponent(out Damageble damageble))
                {
                    damageble.ReceiveDamage(damage, _resolver.PlayerID, false);
                }

                if (hits[0].transform.gameObject.TryGetComponent(out BotDamageble botDamageble))
                {
                    botDamageble.ReceiveDamage(damage, _resolver.PlayerID, false);
                }
            }


            foreach (RaycastHit hit in hits)
            {
                //Debug.Log("Вот тут нас будут ебать читеры! \nЧитеры привет! Это ОЧКО осталось непрекрытым специально для вас... Уебы... \nКЧАУ!");

            }
        }

        private void UpdateBolt(bool value)
        {
            //Update.
            characterAnimator.SetBool(HashBoltAction, bolting = value);
        }

        private void SetHolstered(bool value = true)
        {
            //Update value.
            holstered = value;

            //Update Animator.
            const string boolName = "Holstered";
            characterAnimator.SetBool(boolName, holstered);
        }

        #region ACTION CHECKS

        private bool CanPlayAnimationFire()
        {
            //Block.
            if (holstered || holstering)
            {
                return false;
            }

            //Block.
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || bolting)
            {
                return false;
            }

            //Block.
            if (inspecting)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanPlayAnimationReload()
        {
            //No reloading!
            if (reloading)
            {
                return false;
            }

            //No meleeing!
            if (meleeing)
            {
                return false;
            }

            //Not actioning a bolt.
            if (bolting)
            {
                return false;
            }

            //Can't reload while throwing a grenade.
            if (throwingGrenade)
            {
                return false;
            }

            //Block while inspecting.
            if (inspecting)
            {
                return false;
            }

            //Block Full Reloading if needed.
            if (!equippedWeapon.CanReloadWhenFull() && equippedWeapon.IsFull())
            {
                return false;
            }

            if (inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) == 0)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanPlayAnimationGrenadeThrow()
        {
            //Block.
            if (holstered || holstering)
            {
                return false;
            }

            //Block.
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || bolting)
            {
                return false;
            }

            //Block.
            if (inspecting)
            {
                return false;
            }

            //We need to have grenades!
            if (!grenadesUnlimited && grenadeCount == 0)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanPlayAnimationMelee()
        {
            //Block.
            if (holstered || holstering)
            {
                return false;
            }

            //Block.
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || bolting)
            {
                return false;
            }

            //Block.
            if (inspecting)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanPlayAnimationHolster()
        {
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || bolting)
            {
                return false;
            }

            //Block.
            if (inspecting)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanChangeWeapon()
        {
            //Block.
            if (holstering)
            {
                return false;
            }

            //Block.
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || bolting)
            {
                return false;
            }

            //Block.
            if (inspecting)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanPlayAnimationInspect()
        {
            //Block.
            if (holstered || holstering)
            {
                return false;
            }

            //Block.
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || bolting)
            {
                return false;
            }

            //Block.
            if (inspecting)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanAim()
        {
            if (holstered || inspecting)
            {
                return false;
            }

            //Block.
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || holstering)
            {
                return false;
            }

            //Return.
            return true;
        }

        private bool CanRun()
        {
            //Block.
            if (inspecting || bolting)
            {
                return false;
            }

            //No running while crouching.
            if (movementBehaviour.IsCrouching())
            {
                return false;
            }

            //Block.
            if (meleeing || throwingGrenade)
            {
                return false;
            }

            //Block.
            if (reloading || aiming)
            {
                return false;
            }

            //While trying to fire, we don't want to run. We do this just in case we do fire.
            if (holdingButtonFire && equippedWeapon.HasAmmunition())
            {
                return false;
            }

            //This blocks running backwards, or while fully moving sideways.
            if (axisMovement.y <= 0 || Math.Abs(Mathf.Abs(axisMovement.x) - 1) < 0.01f)
            {
                return false;
            }

            //Return.
            return true;
        }

        #endregion

        #region INPUT
        public void OnTryFire(InputAction.CallbackContext context)
        {
            if (PauseTimePizduk.Instance != null)
            {
                if (PauseTimePizduk.Instance.gameObject.activeSelf)
                {
                    if (!CanPlayAnimationFire())
                    {

                    }

                    if (equippedWeapon.HasAmmunition())
                    {
                        if (equippedWeapon.IsAutomatic())
                        {
                            shotsFired = 0;
                        }

                        if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                        {
                            Fire();
                        }
                    }
                    else
                    {
                        FireEmpty();
                    }

                    holdingButtonFire = false;

                    shotsFired = 0;
                    return;
                }
            }
            if (!cursorLocked)
            {
                return;
            }

            switch (context)
            {
                case { phase: InputActionPhase.Started }:
                    holdingButtonFire = true;
                    shotsFired = 0;
                    break;
                case { phase: InputActionPhase.Performed }:
                    if (!CanPlayAnimationFire())
                    {
                        break;
                    }

                    if (equippedWeapon.HasAmmunition())
                    {
                        if (equippedWeapon.IsAutomatic())
                        {
                            shotsFired = 0;
                            break;
                        }

                        if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                        {
                            Fire();
                        }
                    }
                    else
                    {
                        FireEmpty();

                        if (equippedWeapon.GetAutomaticallyReloadOnEmpty())
                        {
                            if (inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) == 0)
                            {

                            }
                            else
                            {
                                StartCoroutine(nameof(TryReloadAutomatic));
                            }
                        }
                    }
                    break;
                case { phase: InputActionPhase.Canceled }:
                    holdingButtonFire = false;
                    shotsFired = 0;
                    break;
            }
        }

        public void OnTryPlayReload(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            if (aiming)
            {
                aiming = false;
            }

            //Block.
            if (!CanPlayAnimationReload())
            {
                return;
            }

            //Switch.
            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Performed }:
                    //Play Animation.
                    PlayReloadAnimation();
                    break;
            }

        }

        public void OnEquipGun()
        {
            if (CanChangeWeapon())
            {
                EquipSpawnGun();
            }
        }

        public void OnTryPlayReload()
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            if (aiming)
            {
                aiming = false;
            }

            //Block.
            if (!CanPlayAnimationReload())
                return;

            PlayReloadAnimation();
        }

        /// <summary>
        /// Inspect.
        /// </summary>
        public void OnTryInspect(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Block.
            if (!CanPlayAnimationInspect())
                return;

            //Switch.
            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Performed }:
                    //Play Animation.
                    Inspect();
                    break;
            }
        }

        public void OnTryAiming(bool isAim)
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

            if (!cursorLocked)
                return;

            if (isAim)
            {
                holdingButtonAim = true;
                aimStartTime = Time.time;
                _resolver.OnAimOnServer(holdingButtonAim);
            }
            else
            {
                holdingButtonAim = false;
                _resolver.OnAimOnServer(holdingButtonAim);
            }
        }


        public void OnTryAiming(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            if (_aimHold)
            {
                switch (context.phase)
                {
                    case InputActionPhase.Started:
                        //Started.
                        holdingButtonAim = true;
                        //Save Time.
                        aimStartTime = Time.time;
                        _resolver.OnAimOnServer(holdingButtonAim);
                        break;
                    case InputActionPhase.Canceled:
                        //Canceled.
                        holdingButtonAim = false;
                        _resolver.OnAimOnServer(holdingButtonAim);
                        break;
                }
            }
            else
            {
                if (context.phase == InputActionPhase.Started)
                {
                    aiming = !aiming;

                    _resolver.OnAimOnServer(aiming);

                    if (aiming)
                    {
                        aimStartTime = Time.time;
                    }
                    else
                    {

                    }
                }
            }
        }

        public void OnTryHolster(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Switch.
            switch (context.phase)
            {
                //Performed.
                case InputActionPhase.Performed:
                    //Check.
                    if (CanPlayAnimationHolster())
                    {
                        //Set.
                        SetHolstered(!holstered);
                        //Holstering.
                        holstering = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Throw Grenade. 
        /// </summary>
        public void OnTryThrowGrenade(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Switch.
            switch (context.phase)
            {
                //Performed.
                case InputActionPhase.Performed:
                    //Try Play.
                    if (CanPlayAnimationGrenadeThrow())
                        PlayGrenadeThrow();
                    break;
            }
        }

        public void OnTryMelee()
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (CanPlayAnimationMelee())
            {
                PlayMelee();
            }
        }


        public void OnTryMelee(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Switch.
            switch (context.phase)
            {
                //Performed.
                case InputActionPhase.Performed:
                    //Try Play.
                    if (CanPlayAnimationMelee())
                    {
                        PlayMelee();
                    }
                    break;
            }
        }

        public void OnTryRun(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Switch.
            switch (context.phase)
            {
                //Started.
                case InputActionPhase.Started:
                    //Start.
                    holdingButtonRun = true;
                    break;
                //Canceled.
                case InputActionPhase.Canceled:
                    //Stop.
                    holdingButtonRun = false;
                    break;
            }
        }

        /// <summary>
        /// Jump. 
        /// </summary>
        public void OnTryJump(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Switch.
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    movementBehaviour.Jetpack(true);
                    break;

                //Performed.
                case InputActionPhase.Performed:
                    //Jump.
                    movementBehaviour.Jump();
                    break;
                case InputActionPhase.Canceled:
                    movementBehaviour.Jetpack(false);
                    break;
            }
        }

        public void OnTryInventoryNext()
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
            {
                return;
            }

            //Null Check.
            if (inventory == null)
            {
                return;
            }

            if (inventory.AmmoVestIsEmpty())
            {
                Debug.Log("inventory Ammo Vest Is Empty");
            }
            else
            {
                float scrollValue = 1;
                int indexNext = scrollValue > 0 ? inventory.GetNextIndex() : inventory.GetLastIndex();

                int indexCurrent = inventory.GetEquippedIndex();

                if (CanChangeWeapon() && (indexCurrent != indexNext))
                {
                    Equip(indexNext);
                }
            }
        }

        /// <summary>
        /// Next Inventory Weapon.
        /// </summary>
        public void OnTryInventoryNext(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
            {
                return;
            }

            //Null Check.
            if (inventory == null)
            {
                return;
            }

            //Switch.
            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Performed }:
                    //Get the index increment direction for our inventory using the scroll wheel direction. If we're not
                    //actually using one, then just increment by one.
                    float scrollValue = context.valueType.IsEquivalentTo(typeof(Vector2)) ? Mathf.Sign(context.ReadValue<Vector2>().y) : 1.0f;

                    //Get the next index to switch to.
                    int indexNext = scrollValue > 0 ? inventory.GetNextIndex() : inventory.GetLastIndex();

                    //Get the current weapon's index.
                    int indexCurrent = inventory.GetEquippedIndex();

                    //Make sure we're allowed to change, and also that we're not using the same index, otherwise weird things happen!
                    if (CanChangeWeapon() && (indexCurrent != indexNext))
                    {
                        Equip(indexNext);
                    }

                    break;
            }
        }

        /// <summary>
        /// Movement.
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (PauseTimePizduk.Instance != null)
            {
                if (PauseTimePizduk.Instance.gameObject.activeSelf)
                {
                    //Debug.Log("OnMove");
                    axisMovement = Vector2.zero;
                    return;
                }
            }
            if (_isPaused)
            {
                return;
            }

            if (BotsCanMove.Instance == null)
            {
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            axisMovement = cursorLocked ? context.ReadValue<Vector2>() : default;
        }
        /// <summary>
        /// Look.
        /// </summary>
        public void OnLook(InputAction.CallbackContext context)
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
                ////Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            axisLook = cursorLocked ? context.ReadValue<Vector2>() : default;

            //Make sure that we have a weapon.
            if (equippedWeapon == null)
                return;

            //Make sure that we have a scope.
            if (equippedWeaponScope == null)
                return;

            //If we're aiming, multiply by the mouse sensitivity multiplier of the equipped weapon's scope!
            axisLook *= aiming ? equippedWeaponScope.GetMultiplierMouseSensitivity() : 1.0f;
        }


        public void OnLockCursor(InputAction.CallbackContext context)
        {/*
			if (PauseTimePizduk.Instance != null)
			{
				if (PauseTimePizduk.Instance.gameObject.activeSelf)
				{
					return;
				}
			}*/
            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Performed }:
                    //Toggle the cursor locked value.

                    //Update the cursor's state.
                    break;
                case { phase: InputActionPhase.Started }:
                    /*
					_isEsc = !_isEsc;
                    //cursorLocked = !cursorLocked;

                    if (_isTab)
					{
                        _isTab = false;
                        _isEsc = false;
						cursorLocked = true;
					}
					else
                    {
                        if (_isEsc)
                        {
                            cursorLocked = false;
                        }
                        else
                        {
                            cursorLocked = true;
                        }
                    }
					*/
                    /*
                    if (PauseScreen.Instance.IsSetting)
                    {
                        cursorLocked = false;

                    }
                    else
                    {
                        cursorLocked = true;
                    }

                    UpdateCursorState();*/
                    OnPressEsc?.Invoke();
                    break;

            }
        }
        public void PressChat(InputAction.CallbackContext context)
        {
            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Performed }:
                    break;
                case { phase: InputActionPhase.Started }:
                    OnPressChat?.Invoke();
                    break;

            }
        }

        public void OnUpdateTutorial(InputAction.CallbackContext context)
        {

            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Performed }:
                    /*//Toggle the cursor locked value.
                    cursorLocked = !cursorLocked;
                    //Update the cursor's state.*/
                    //UpdateCursorState();
                    break;
                case { phase: InputActionPhase.Started }:
                    /*_isTab = !_isTab;

					if (_isEsc)
					{
						_isEsc = false;
						_isTab = true;
						cursorLocked = false;
                    }
					else
                    {
                        //cursorLocked = !cursorLocked;
                        if (_isTab)
						{
							cursorLocked = false;
                        }
						else
                        {
                            cursorLocked = true;
                        }
					}*/
                    //UpdateCursorState();
                    OnPressTab?.Invoke();
                    break;

            }

            /*
			//Switch.
			tutorialTextVisible = context switch
			{
				//Started. Show the tutorial.
				{phase: InputActionPhase.Started} => true,
				//Canceled. Hide the tutorial.
				{phase: InputActionPhase.Canceled} => false,
				//Default.
				_ => tutorialTextVisible
			};*/
        }

        #endregion

        #region ANIMATION EVENTS

        public void EjectCasing()
        {
            //Notify the weapon.
            if (equippedWeapon != null)
                equippedWeapon.EjectCasing();
        }



        public void FillAmmunition(int amount)
        {
            //Debug.Log($"FillAmmunition {amount} {equippedWeapon.GetAmmunitionCurrent()} {inventory.GetAmmoCount(equippedWeapon.GetAmmoType())}");

            int count = 0;

            if (inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) == 1)
            {
                const string boolNameReloading = "Reloading";
                //Update the character animator.
                characterAnimator.SetBool(boolNameReloading, false);
                //Update the weapon animator.
                equippedWeapon.GetAnimator().SetBool(boolNameReloading, false);
                count = 1;

                inventory.RemoveAmmo(equippedWeapon.GetAmmoType(), count);

                if (equippedWeapon != null)
                {
                    //Debug.Log($"count {count}");
                    equippedWeapon.FillAmmunition(count);
                }

                return;
            }

            if (equippedWeapon.HasCycledReload())
            {
                //Notify the weapon to fill the ammunition by the amount.
                if (inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) > 0)
                {
                    //Debug.Log($"FillAmmunition AmmoCount {equippedWeapon.GetAmmoType()} {inventory.GetAmmoCount(equippedWeapon.GetAmmoType())}");
                    count = amount;
                }
                else
                {
                    const string boolNameReloading = "Reloading";
                    //Update the character animator.
                    characterAnimator.SetBool(boolNameReloading, false);
                    //Update the weapon animator.
                    equippedWeapon.GetAnimator().SetBool(boolNameReloading, false);
                }
            }
            else
            {
                //Notify the weapon to fill the ammunition by the amount.
                if (inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) > 0)
                {
                    //Debug.Log($"FillAmmunition AmmoCount {equippedWeapon.GetAmmoType()} {inventory.GetAmmoCount(equippedWeapon.GetAmmoType())}");

                    int total = equippedWeapon.GetAmmunitionTotal();
                    int cur = equippedWeapon.GetAmmunitionCurrent();
                    int invcount = inventory.GetAmmoCount(equippedWeapon.GetAmmoType());

                    int nesserycount = total - cur;
                    nesserycount = nesserycount > invcount ? invcount : nesserycount;

                    count = nesserycount;// inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) > equippedWeapon.GetAmmunitionTotal() ? equippedWeapon.GetAmmunitionTotal() : inventory.GetAmmoCount(equippedWeapon.GetAmmoType());
                }
            }

            inventory.RemoveAmmo(equippedWeapon.GetAmmoType(), count);

            if (equippedWeapon != null)
            {
                equippedWeapon.FillAmmunition(count);
            }

        }
        /*
        public void FillAmmunition(int amount)
		{
			//Notify the weapon to fill the ammunition by the amount.
			if (inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) > 0)
			{
				//Debug.Log($"FillAmmunition AmmoCount {equippedWeapon.GetAmmoType()} {inventory.GetAmmoCount(equippedWeapon.GetAmmoType())}");

				int total = equippedWeapon.GetAmmunitionTotal();
				int cur = equippedWeapon.GetAmmunitionCurrent();
				int invcount = inventory.GetAmmoCount(equippedWeapon.GetAmmoType());

				int nesserycount = total - cur;
				nesserycount = nesserycount > invcount ? invcount : nesserycount;


				int count = nesserycount;// inventory.GetAmmoCount(equippedWeapon.GetAmmoType()) > equippedWeapon.GetAmmunitionTotal() ? equippedWeapon.GetAmmunitionTotal() : inventory.GetAmmoCount(equippedWeapon.GetAmmoType());
				inventory.RemoveAmmo(equippedWeapon.GetAmmoType(), count);
				if (equippedWeapon != null)
					equippedWeapon.FillAmmunition(count);
			}
		}*/

        public void Crouch()
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Debug.Log("Crouch");
            OnPressCrouch?.Invoke();
        }

        public void Crouch(InputAction.CallbackContext context)
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
                //Debug.Log("Character BotsCanMove.Instance == null");
            }
            else
            {
                if (!BotsCanMove.Instance.CanMove)
                {
                    return;
                }
            }

            if (!cursorLocked)
                return;

            //Switch.
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    //Debug.Log("OnPressCrouch");
                    OnPressCrouch?.Invoke();
                    break;
                //Performed.
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Canceled:
                    break;
            }
        }

        public void Grenade()
        {
            //Make sure that the grenade is valid, otherwise we'll get errors.
            if (grenadePrefab == null)
                return;

            //Make sure we have a camera!
            if (cameraWorld == null)
                return;

            //Remove Grenade.
            if (!grenadesUnlimited)
            {
                grenadeCount--;
                CharacterValues.UpdateValue(CharacterValueKey.GrenadeCountCurent, new ValueCurentMax(grenadeCount, grenadeTotal));
            }

            //Get Camera Transform.
            Transform cTransform = cameraWorld.transform;
            //Calculate the throwing location.
            Vector3 position = cTransform.position;
            position += cTransform.forward * grenadeSpawnOffset;
            //Throw.
            Instantiate(grenadePrefab, position, cTransform.rotation);
        }

        public void SetActiveMagazine(int active)
        {
            //Set magazine gameObject active.
            equippedWeaponMagazine.gameObject.SetActive(active != 0);
        }

        public void AnimationEndedBolt()
        {
            //Update.
            UpdateBolt(false);
        }
        public void AnimationEndedReload()
        {
            //Stop reloading!
            reloading = false;
        }

        public void AnimationEndedGrenadeThrow()
        {
            //Stop Grenade Throw.
            throwingGrenade = false;
        }
        public void AnimationEndedMelee()
        {
            //Stop Melee.
            meleeing = false;
        }

        public void AnimationEndedInspect()
        {
            //Stop Inspecting.
            inspecting = false;
        }
        public void AnimationEndedHolster()
        {
            //Stop Holstering.
            holstering = false;
        }

        public void SetSlideBack(int back)
        {
            //Set slide back.
            if (equippedWeapon != null)
                equippedWeapon.SetSlideBack(back);
        }

        public void SetActiveKnife(int active)
        {
            //Set Active.
            knife.SetActive(active != 0);
        }

        public void AddAmmo(AmmoType type, int count)
        {
            inventory.AddAmmo(type, count);

            if (type == AmmoType.grenade)
            {
                grenadeCount += count;

                CharacterValues.UpdateValue(CharacterValueKey.GrenadeCountCurent, new ValueCurentMax(grenadeCount, grenadeTotal));
            }
        }

        [ContextMenu("Permach")]
        public void Permach()
        {
            ReceiveDamage(120, 0, 0);

        }

        public void ReceiveDamage(float damage, uint sender, uint receiver)
        {
            Debug.Log($"player recieved damage {damage}");

            /*
            if (GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Sandbox)
            {
				if (BotManager.Instance.SandboxReady)
				{

				}
				else
				{
                    //Debug.Log($"{receiver} получил урон от {sender} : {damage}, но игра еще не началась.");
                    return;
                }
            }*/
            /*
            if (sender != null)
            {
                //Debug.Log($"{receiver} получил урон от {sender} : {damage}");
            }*/

            if (PauseScreen.Instance.IsPaused)
            {

            }
            else
            {
                _hitPoints -= damage;
                _hitPoints = _hitPoints < 0 ? 0 : _hitPoints;
                _hitPoints = _hitPoints > _hitPointsMax ? _hitPointsMax : _hitPoints;

                _haveDamage = damage > 0;
                timecur = 0;
                OnDamageRecived?.Invoke(_hitPoints, _hitPointsMax);
                OnDamageDeltaRecived?.Invoke(damage);

                CharacterValues.UpdateValue(CharacterValueKey.LifeLVL, new ValueCurentMax(_hitPoints, _hitPointsMax));

                if (_hitPoints <= 0 && _isMayDie)
                {
                    if (LocalSounds.Instance != null)
                    {
                        LocalSounds.Instance.PlaySound("die");
                    }

                    OnDie?.Invoke();
                }
            }

        }

        public float GetLife() => _hitPoints;
        public float GetLifeTotal() => _hitPointsMax;

        #endregion

        #endregion
    }

}