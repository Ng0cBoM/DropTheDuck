using Amas.Core.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyPopup : PopupScreen
{
    [SerializeField] private Button _closeBtn; 
    private void Awake()
    {
        _closeBtn.onClick.AddListener(UiManager.I.Pop);
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
        yield return null;
    }
}
