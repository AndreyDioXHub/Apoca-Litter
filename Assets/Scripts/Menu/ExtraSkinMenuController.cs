using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExtraSkinMenuController : MonoBehaviour
{
    public UnityEvent<int> OnPreviewIndexChanged = new UnityEvent<int>();

    private ChelikiMenuModel _model;

    [SerializeField]
    private AnimationCurve _spinCurve;
    [SerializeField]
    private float _spinTime;
    [SerializeField]
    private float _landingXPosition;
    [SerializeField]
    private int _hearIndexSelectedBeforExtraSkin;
    [SerializeField]
    private Transform _landing;
    [SerializeField]
    private RectTransform _overlapingSquare;
    [SerializeField]
    private GameObject _buttonPrefab;
    [SerializeField]
    private bool _buttonsReady;
    [SerializeField]
    private List<Sprite> _icons = new List<Sprite>();
    [SerializeField]
    private List<ExtraSkinSelectButton> _buttons = new List<ExtraSkinSelectButton>();


    private string _name = "haircut";
    private bool _isNotNeedUpdate;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _landingXPosition = _landing.GetComponent<RectTransform>().anchoredPosition.x;

        if (_landingXPosition < -3400 || _landingXPosition >= 0)
        {
            Vector2 landingAnchoredPosition = _landing.GetComponent<RectTransform>().anchoredPosition;
            landingAnchoredPosition.x = -1700;
            _landing.GetComponent<RectTransform>().anchoredPosition = landingAnchoredPosition;
        }
        if (_buttonsReady)
        {
            if (_buttons.Count > 0)
            {
                for (int i = 0; i < _buttons.Count; i++)
                {
                    if (RectHelp.IsOverlapping(_overlapingSquare, _buttons[i].GetComponent<RectTransform>()))
                    {
                        _buttons[i].PreviewSkin();
                    }

                    /*
                    int landingButtonAnchoredPositionX = (int)(_buttons[i].GetComponent<RectTransform>().anchoredPosition.x - 95);

                    if(landingButtonAnchoredPositionX < -)
                    if ((-1 * landingButtonAnchoredPosition.x)/100 == (int)((_landingXPosition - 95))/100)
                    {
                        _buttons[i].PreviewSkin();
                    }*/
                }
            }
        }
    }

    [ContextMenu("PrespawnButtons")]
    public void PrespawnButtons()
    {
        int coppysCount = _icons.Count * 3;

        for (int i = 0; i < coppysCount; i++)
        {
            GameObject button = Instantiate(_buttonPrefab);
            button.transform.parent = _landing;
            ExtraSkinSelectButton selectButton = button.GetComponent<ExtraSkinSelectButton>();
            _buttons.Add(selectButton);
        }
    }

    public void Init(ChelikiMenuModel model)
    {
        _model = model;
        int coppysCount = _icons.Count * 3;

        if (_buttons.Count == 0)
        {
            for (int i = 0; i < coppysCount; i++)
            {
                GameObject button = Instantiate(_buttonPrefab);
                button.transform.parent = _landing;
                ExtraSkinSelectButton selectButton = button.GetComponent<ExtraSkinSelectButton>();
                int iconIndex = i % _icons.Count;
                selectButton.Init(this, _model, _icons[iconIndex], iconIndex);
                int winingIndex = PlayerPrefs.GetInt(PlayerPrefsConsts.haircutwiningindex, -1);
                selectButton.UpdateWinningStatus(winingIndex);
                _buttons.Add(selectButton);
            }
        }
        else
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                int iconIndex = i % _icons.Count;
                _buttons[i].Init(this, _model, _icons[iconIndex], iconIndex);
                int winingIndex = PlayerPrefs.GetInt(PlayerPrefsConsts.haircutwiningindex, -1);
                _buttons[i].UpdateWinningStatus(winingIndex);
            }
        }

        _buttons[0].PreviewSkin();

        Vector2 landingAnchoredPosition = _landing.GetComponent<RectTransform>().anchoredPosition;
        landingAnchoredPosition.x = -1700;
        _landing.GetComponent<RectTransform>().anchoredPosition = landingAnchoredPosition;

        _buttonsReady = true;
        //_landingXPosition = .x;
        //_model.OnSelectNameIndex.AddListener(Select);
    }

    private void OnEnable()
    {
        if (_model != null)
        {
            _hearIndexSelectedBeforExtraSkin = _model.GetIndexByName(_name);
            int winingIndex = PlayerPrefs.GetInt(PlayerPrefsConsts.haircutwiningindex, -1);

            if (_buttons.Count > 0)
            {
                _buttons[0].PreviewSkin();

                for (int i = 0; i < _buttons.Count; i++)
                {
                    _buttons[i].UpdateWinningStatus(winingIndex);
                }
            }

            Vector2 landingAnchoredPosition = _landing.GetComponent<RectTransform>().anchoredPosition;
            landingAnchoredPosition.x = -1700;
            _landing.GetComponent<RectTransform>().anchoredPosition = landingAnchoredPosition;
            //OnPreviewIndexChanged?.Invoke(0);
        }
    }

    public void Spin()
    {
        SpinAsync();
    }
    
    public async void SpinAsync()
    {
        float timeCur = _spinTime;

        while (timeCur > 0)
        {
            float spinValue = _spinCurve.Evaluate(_spinTime - timeCur);
            int speedMin = 30;
            int speedMax = 60;
            int speed = Random.Range(speedMin, speedMax);
            Vector2 landingAnchoredPosition = Vector2.zero;

            if (_landing)
            {
                landingAnchoredPosition = _landing.GetComponent<RectTransform>().anchoredPosition;
                landingAnchoredPosition.x += speed * spinValue;
                _landing.GetComponent<RectTransform>().anchoredPosition = landingAnchoredPosition;

                await UniTask.WaitForSeconds(Time.fixedDeltaTime);
                timeCur -= Time.fixedDeltaTime;
            }
        }

        if (_buttons.Count > 0)
        {
            int winingStatusIndex = -1;

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].IsSelected)
                {
                    winingStatusIndex = _buttons[i].Index;
                }
            }

            _hearIndexSelectedBeforExtraSkin = winingStatusIndex;

            PlayerPrefs.SetInt(PlayerPrefsConsts.haircutwiningindex, _hearIndexSelectedBeforExtraSkin);

            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].UpdateWinningStatus(winingStatusIndex);
            }
        }
    }

    private void OnDisable()
    {
        ReturnHeadIndexWhenClose();
        //_isNotNeedUpdate = false;
    }

    public void ReturnHeadIndexWhenClose()
    {
        _model.Equip(_name, _hearIndexSelectedBeforExtraSkin);
    } 

    /*
    public void Select(string name, int index)
    {
        if(_name.Equals(name))
        {
            if (_isNotNeedUpdate)
            {

            }
            else
            {
                _hearIndexSelectedBeforExtraSkin = index;
                _isNotNeedUpdate = true;
            }
        }
    }*/
}
