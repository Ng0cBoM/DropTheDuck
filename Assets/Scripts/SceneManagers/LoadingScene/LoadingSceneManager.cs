using Core.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneManager : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
        var userData = DataManager.I.UserData;
        ScenesChanger.ChangeScene("Home");
    }
}