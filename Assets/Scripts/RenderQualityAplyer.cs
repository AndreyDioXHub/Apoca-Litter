using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RenderQualityAplyer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _renderQualityScoreText;
    private int _renderQualityScore;
    private string _sceneNameMenu = "MenuNetwork";

    void Start()
    {
        LateStart();
    }

    public async void LateStart()
    {
        await UniTask.WaitForSeconds(0.1f);

        _renderQualityScore = PlayerPrefs.GetInt(PlayerPrefsConsts.renderqualityscore, 4);
        _renderQualityScoreText.text = _renderQualityScore.ToString();

        int baseResolution = 1920;

        switch (_renderQualityScore)
        {
            case 1:
                baseResolution = 256;
                break;
            case 2:
                baseResolution = 512;
                break;
            case 3:
                baseResolution = 1024;
                break;
            case 4:
                baseResolution = 1920;
                break;
        }

        Screen.SetResolution(baseResolution, baseResolution * Screen.height / Screen.width, true);
    }

    void Update()
    {
        
    }

    public void UpdateRenderQuality()
    {
        _renderQualityScore--;
        _renderQualityScore = _renderQualityScore < 1 ? 4 : _renderQualityScore;
        _renderQualityScoreText.text = _renderQualityScore.ToString();
        PlayerPrefs.SetInt(PlayerPrefsConsts.renderqualityscore, _renderQualityScore);

        int baseResolution = 1920;

        switch (_renderQualityScore)
        {
            case 1:
                baseResolution = 256;
                break;
            case 2:
                baseResolution = 512;
                break;
            case 3:
                baseResolution = 1024;
                break;
            case 4:
                baseResolution = 1920;
                break;
        }

        Screen.SetResolution(baseResolution, baseResolution * Screen.height / Screen.width, true);
    }
}
