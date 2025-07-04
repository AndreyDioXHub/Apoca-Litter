using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TPVWeapon : MonoBehaviour
{
    public string WeaponName;

    private TPVInputController _input;
    //private PlayerNetworkResolver _resol
    //
    //ver;
    [SerializeField]
    private List<GameObject> _scopes = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _muzzles = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _lasers = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _grips = new List<GameObject>();

    [SerializeField]
    private GameObject _magazine;
    [SerializeField]
    private Transform _magazineSlot;
    [SerializeField]
    private Transform _magazineSlotHand;

    [SerializeField]
    private GameObject _hideWhenFired;

    [SerializeField]
    private AudioSource _tpvAudioSource;

    [SerializeField]
    private AudioClip _reloadingOpen;
    [SerializeField]
    private AudioClip _reloadingInsert;
    [SerializeField]
    private AudioClip _reloadingClose;
    [SerializeField]
    private AudioClip _fireEmpty;
    [SerializeField]
    private AudioClip _boltAction;

    /*
    [SerializeField]
    private List<string> _materialNamesForChanging = new List<string>();
    [SerializeField]
    private Material _baseMTL;
    [SerializeField]
    private Material _casingMTL;
    private List<Transform> _childTransforms = new List<Transform>();
    */

    void Start()
    {
    }

    public void Init(TPVInputController input, Transform magazineSlotHand)
    {
        //_resolver = resolver;
        _input = input;
        _magazineSlotHand = magazineSlotHand;

        foreach(var muzzle in _muzzles)
        {
            muzzle.GetComponent<TPVMuzzle>().Init(gameObject);
        }

        _input.OnAttachmentChanged.AddListener(AttachmentChanged);
        _input.OnFire.AddListener(HideWhenFired);

        _input.OnReloadingOpen.AddListener(ReloadingOpen);
        _input.OnMagazineInArm.AddListener(MagazineInArm);
        _input.OnReloadingInsert.AddListener(ReloadingInsert);
        _input.OnBoltAction.AddListener(BoltAction);
    }

    /*
    [ContextMenu("Show Materials Names")]
    public void ShowMaterialsNames()
    {
        _childTransforms.Clear();
        _childTransforms = new List<Transform>();

        ShowMaterialsNames(transform);

        foreach (Transform trans in _childTransforms)
        {
            if (trans.TryGetComponent(out MeshRenderer renderer))
            {
                List<Material> m = new List<Material>();
                renderer.GetSharedMaterials(m);

                foreach (Material mat in m)
                {
                    if (_materialNamesForChanging.Count == 0)
                    {
                        if(mat.name.Equals("M_Laser_Beam") || mat.name.Equals("ATT_Scope_Unlit")
                            || mat.name.Equals("M_ATT_Sight_Fade_Round")
                            || mat.name.Equals("M_ATT_Sight_Dot_Red_02")
                            || mat.name.Equals("M_ATT_Sight_Fade_Square")
                            || mat.name.Equals("M_ATT_Sight_Square_Blue")
                            || mat.name.Equals("M_ATT_Sight_Arrow_Blue")
                            || mat.name.Equals("M_ATT_Sight_Arrow_Red")
                            || mat.name.Equals("M_ATT_Sight_Transparent")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_001")
                            || mat.name.Equals("M_ATT_Sight_Render")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_003")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_002")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_005"))
                        {

                        }
                        else
                        {
                            _materialNamesForChanging.Add(mat.name);
                        }
                    }

                    if (_materialNamesForChanging.Contains(mat.name))
                    {

                    }
                    else
                    {
                        if (mat.name.Equals("M_Laser_Beam") || mat.name.Equals("ATT_Scope_Unlit")
                            || mat.name.Equals("M_ATT_Sight_Fade_Round")
                            || mat.name.Equals("M_ATT_Sight_Dot_Red_02")
                            || mat.name.Equals("M_ATT_Sight_Fade_Square")
                            || mat.name.Equals("M_ATT_Sight_Square_Blue")
                            || mat.name.Equals("M_ATT_Sight_Arrow_Blue")
                            || mat.name.Equals("M_ATT_Sight_Arrow_Red")
                            || mat.name.Equals("M_ATT_Sight_Transparent")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_001")
                            || mat.name.Equals("M_ATT_Sight_Render")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_003")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_002")
                            || mat.name.Equals("M_ATT_Sight_Crosshair_005"))
                        {

                        }
                        else
                        {
                            _materialNamesForChanging.Add(mat.name);
                        }
                    }
                }
            }
        }
    }

    [ContextMenu("SetUP Materials")]
    public void SetUPMaterials()
    {
        foreach (Transform trans in _childTransforms)
        {
            if (trans.TryGetComponent(out MeshRenderer renderer))
            {
                //Debug.Log($"SetUP {trans.name}");
                List<Material> m = new List<Material>();
                renderer.GetSharedMaterials(m);
                List<Material> newMaterials = m;

                for (int i = 0; i < newMaterials.Count; i++)
                {
                    foreach (string changingName in _materialNamesForChanging)
                    {
                        if (newMaterials[i].name.Equals(changingName))
                        {
                            if (newMaterials[i].name.Contains("Casings"))
                            {
                                newMaterials[i] = _casingMTL;
                            }
                            else
                            {
                                newMaterials[i] = _baseMTL;
                            }
                        }
                    }
                }

                //Debug.Log($"SetUP {m.Count} {newMaterials.Count}");

                renderer.SetSharedMaterials(newMaterials);
            }
        }

    }

    public void ShowMaterialsNames(Transform parent)
    {
        foreach (Transform trans in parent)
        {
            _childTransforms.Add(trans);
            ShowMaterialsNames(trans);
        }
    }


    [ContextMenu("SpawnEmpty")]
    public void SpawnEmpty()
    {
        GameObject scopeEmpty = new GameObject("Scope_Empty");
        GameObject laserEmpty = new GameObject("Laser_Empty");
        GameObject gripEmpty = new GameObject("Grip_Empty");

        scopeEmpty.transform.SetParent(transform);
        laserEmpty.transform.SetParent(transform);
        gripEmpty.transform.SetParent(transform);

        scopeEmpty.transform.localPosition = Vector3.zero;
        laserEmpty.transform.localPosition = Vector3.zero;
        gripEmpty.transform.localPosition = Vector3.zero;

        scopeEmpty.transform.localRotation = Quaternion.Euler(Vector3.zero);
        laserEmpty.transform.localRotation = Quaternion.Euler(Vector3.zero);
        gripEmpty.transform.localRotation = Quaternion.Euler(Vector3.zero);

        scopeEmpty.transform.localScale = Vector3.one;
        laserEmpty.transform.localScale = Vector3.one;
        gripEmpty.transform.localScale = Vector3.one;

        List<GameObject> scopes = new List<GameObject>();// = _scopes.ToArray();
        List<GameObject> lasers = new List<GameObject>();// = _scopes.ToArray();
        List<GameObject> grips = new List<GameObject>();// = _scopes.ToArray();

        scopes.Add(scopeEmpty);
        lasers.Add(laserEmpty);
        grips.Add(gripEmpty);

        foreach (GameObject scope in _scopes)
        {
            scopes.Add(scope);
        }

        _scopes = scopes;

        foreach (GameObject laser in _lasers)
        {
            lasers.Add(laser);
        }

        _lasers = lasers;

        foreach (GameObject grip in _grips)
        {
            grips.Add(grip);
        }

        _grips = grips;


    }
    */
    public void ReloadingOpen()
    {
        if (gameObject.activeSelf)
        {
            if (!_input.IsLocalPlayer)
            {
                if (_reloadingOpen == null)
                {
                    //Debug.Log("_reloadingOpen == null");
                }
                else
                {
                    _tpvAudioSource.Stop();
                    _tpvAudioSource.clip = _reloadingOpen;
                    _tpvAudioSource.Play();
                }
            }

            if (_magazine == null)
            {
                //Debug.Log("_magazine == null");
            }
            else
            {
                _magazine.transform.SetParent(_magazineSlotHand);
                _magazine.transform.localPosition = Vector3.zero;
                _magazine.transform.rotation = _magazineSlotHand.rotation;
                _magazine.SetActive(false);
            }
        }
    }

    public void MagazineInArm()
    {
        if (gameObject.activeSelf)
        {
            if (_magazine == null)
            {
                //Debug.Log("_magazine == null");
            }
            else
            {
                _magazine.SetActive(true);
            }
        }
    }
    public void HideWhenFired()
    {
        if (gameObject.activeSelf)
        {
            if (_hideWhenFired == null)
            {
                //Debug.Log("_hideWhenFired == null");
            }
            else
            {
                _hideWhenFired.SetActive(false);
            }
        }
    }

    public void ReloadingInsert()
    {
        if (gameObject.activeSelf)
        {
            if (!_input.IsLocalPlayer)
            {
                if (_reloadingInsert == null)
                {
                    //Debug.Log("_reloadingInsert == null");
                }
                else
                {
                    _tpvAudioSource.Stop();
                    _tpvAudioSource.clip = _reloadingInsert;
                    _tpvAudioSource.Play();
                }
            }

            if (_magazine == null)
            {
                //Debug.Log("_magazine == null");
            }
            else
            {
                _magazine.transform.SetParent(_magazineSlot);
                _magazine.transform.localPosition = Vector3.zero;
                _magazine.transform.rotation = _magazineSlot.rotation;
                _magazine.SetActive(true);
            }
        }
    }

    public void BoltAction()
    {
        if (gameObject.activeSelf)
        {
            if (!_input.IsLocalPlayer)
            {
                if (_boltAction == null)
                {
                    //Debug.Log("_boltAction == null");
                }
                else
                {
                    _tpvAudioSource.Stop();
                    _tpvAudioSource.clip = _boltAction;
                    _tpvAudioSource.Play();
                }
            }
        }
    }

    public async void AttachmentChanged(int scope, int muzzle, int laser, int grip)
    {
        await UniTask.Yield();
        if (gameObject != null)
        {
            if (gameObject.activeSelf)
            {
                foreach (GameObject scopeGO in _scopes)
                {
                    scopeGO.SetActive(false);
                }

                if (scope >= 0)
                {
                    _scopes[scope].SetActive(true);
                }

                foreach (GameObject muzzleGO in _muzzles)
                {
                    muzzleGO.SetActive(false);
                }

                if (muzzle >= 0)
                {
                    _muzzles[muzzle].SetActive(true);
                }

                foreach (GameObject laserGO in _lasers)
                {
                    laserGO.SetActive(false);
                }

                if (laser >= 0)
                {
                    _lasers[laser].SetActive(true);
                }

                foreach (GameObject gripGO in _grips)
                {
                    gripGO.SetActive(false);
                }

                if (grip >= 0)
                {
                    _grips[grip].SetActive(true);
                }
            }
        }
    }

    void Update()
    {
        
    }
}
