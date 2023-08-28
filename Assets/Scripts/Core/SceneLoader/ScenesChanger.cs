using System;
using EgdFoundation;
using UnityEngine.SceneManagement;
using DVAH;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;

public static class ScenesChanger
{
    private static Scene oldScene;
    private static UnityEngine.AsyncOperation loadSceneAsync;
    public enum CONDITON_LOADING
    {
        load_ad_open_done,
        load_native_done
    }
    public static void ChangeScene(string sceneName)
    {
        AdManager.Instant.DestroyBanner();
        LoadingManager.Instant.DoneConditionSelf(
            (int)CONDITON_LOADING.load_ad_open_done,
            () => AdManager.Instant.AdsOpenIsLoaded(0)
        );
        oldScene = SceneManager.GetActiveScene();
        LoadingManager.Instant.Init(1, LoadingCompleteCallback).SetMaxTimeLoading(30);

        loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        // SceneManager.UnloadSceneAsync(oldScene);
    }

    private static void LoadingCompleteCallback(List<bool> doneCondition)
    {
        AdManager.Instant.ShowAdOpen(
            0,
            true,
            (id, state) =>
            {
                // id mean adOpen id
                if (state == OpenAdState.None)
                {
                    //adOpen show fail or something wrong
                }

                if (state == OpenAdState.Open)
                {
                    Debug.Log($"Ads opened");
                }

                if (state == OpenAdState.Click)
                {
                    //trigger callback when user click ad
                }

                if (state == OpenAdState.Closed)
                {
                    //trigger callback when user close ad
                }
                AdManager.Instant.InitializeBannerAdsAsync();
            }
        );
    }

}