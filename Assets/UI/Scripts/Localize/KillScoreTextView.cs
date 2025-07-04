using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SocialPlatforms.Impl;

public class KillScoreTextView : MonoBehaviour
{
    public string Score = "10";
    public string ScoreCurrent = "10";
    public LocalizeStringEvent localizeString;

    void Start()
    {
        gameObject.SetActive(false);
        //WaitingScreenModel.Instance.OnGameReady.AddListener(() => gameObject.SetActive(false));
    }

    public void StartNewGame()
    {
        gameObject.SetActive(true);
        localizeString.RefreshString();
    }

    public void ScoreUpdated(int score)
    {
        Score = score.ToString();
        localizeString.RefreshString();
    }

    public void ScoreCurrentUpdated(int score)
    {
        ScoreCurrent = score.ToString();
        localizeString.RefreshString();
    }
}
