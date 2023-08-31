using Core.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneManager : MonoBehaviour
{
    private void Start()
    {
        var userData = DataManager.I.UserData;
        ScenesChanger.ChangeScene("Home");
    }
}