using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DummyView : ScreenItem
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _debugButton;
    private Action onClickAction;
    private void Awake()
    {
        _debugButton.onClick.AddListener(() => onClickAction.Invoke());
    }
    public override void ReSetup()
    {
        tween.SetForwardDirectionAndEnabled();
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
        if (screenData != null)
        {
            Dictionary<string, object> dataDictionary = screenData.screenData;
            string textMessage = (string)dataDictionary["message"];
            _text.text = textMessage;
            onClickAction = screenData.screenAction["ShowLog"].Invoke;
        }

        yield return null;
    }
}
