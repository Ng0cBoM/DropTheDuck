using Core.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button addEnergyButton;
    [SerializeField] private Button addCoinButton;
    [SerializeField] private TextMeshProUGUI highestScoretext;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        playButton.onClick.AddListener(PlayGame);
        shopButton.onClick.AddListener(OpenShop);
        inventoryButton.onClick.AddListener(OpenInventory);
        settingButton.onClick.AddListener(OpenSetting);
        addEnergyButton.onClick.AddListener(AddEnergy);
        addCoinButton.onClick.AddListener(AddCoin);
    }

    private void Start()
    {
        highestScoretext.text = DataManager.I.UserData.HighestScore.ToString();
        scoreText.text = DataManager.I.UserData.Coin.ToString();
    }

    private void PlayGame()
    {
        ScenesChanger.ChangeScene("MainGame");
    }

    private void OpenShop()
    { }

    private void OpenInventory()
    { }

    private void OpenSetting()
    { }

    private void AddEnergy()
    { }

    private void AddCoin()
    { }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        shopButton.onClick.RemoveAllListeners();
        inventoryButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        addEnergyButton.onClick.RemoveAllListeners();
        addCoinButton.onClick.RemoveAllListeners();
    }
}