using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSceneManager : MonoBehaviour
{
    
    void Start()
    {
        ScreenData dummySceneData = new ScreenData() {
            screenData = new Dictionary<string, object>(),
            screenAction = new Dictionary<string, System.Action>()
        };
        dummySceneData.screenData["message"] = "Hello world! This is a dummy screen";
        dummySceneData.screenAction["ShowLog"] = () => { UiManager.I.Push("DummyPopup"); };
        UiManager.I.Push("Dummy", dummySceneData);
    }

}
