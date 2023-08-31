using Core.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EgdFoundation;

public class MainGamePage : MonoBehaviour
{
    [SerializeField]
    private Button pauseButton;

    [SerializeField]
    private TextMeshProUGUI coinText;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        coinText.text = DataManager.I.UserData.Coin.ToString();
        scoreText.text = "0";
        pauseButton.onClick.AddListener(OpenPausePopup);
        SignalBus.I.Register<UpdateCoin>(UpdateCoinHandle);
        SignalBus.I.Register<UpdateScore>(UpdateScoreHandle);
    }

    private void UpdateCoinHandle(UpdateCoin signal)
    {
    }

    private void UpdateScoreHandle(UpdateScore signal)
    {
        scoreText.text = signal.score.ToString();
    }

    private void OpenPausePopup()
    {
    }

    private void UpdateScore()
    {
    }
}