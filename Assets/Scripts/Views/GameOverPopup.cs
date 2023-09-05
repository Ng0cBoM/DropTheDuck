using Amas.Core.UI;
using Core.Framework;
using EgdFoundation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : PopupScreen
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highestScoreText;
    [SerializeField] private Button continueButtonWithoutAds;
    [SerializeField] private Button continueButtonWithAds;
    [SerializeField] private Button backToHomeButton;

    private int score;

    private void Awake()
    {
        continueButtonWithoutAds.onClick.AddListener(ContinueWithoutAds);
        continueButtonWithAds.onClick.AddListener(ContinueWithAds);
        backToHomeButton.onClick.AddListener(BackHome);
    }

    private void ContinueWithoutAds()
    {
        UiManager.I.Pop();
        SignalBus.I.FireSignal<ContinuePlay>(new ContinuePlay());
    }

    private void ContinueWithAds()
    {
        SignalBus.I.FireSignal<ContinuePlay>(new ContinuePlay());
        UiManager.I.Pop();
    }

    private void BackHome()
    {
        UiManager.I.Pop();
        ScenesChanger.ChangeScene("Home");
    }

    public override void ReSetup()
    {
    }

    protected override IEnumerator OnPopScreen()
    {
        yield return null;
    }

    protected override IEnumerator OnPushScreen(ScreenData screenData = null)
    {
        yield return null;
    }

    protected override IEnumerator Setup(ScreenData screenData = null)
    {
        score = (int)screenData.screenData["score"];
        highestScoreText.text = "High Score: " + DataManager.I.UserData.HighestScore;
        scoreText.text = "Score: " + score;
        yield return null;
    }
}