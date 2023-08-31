using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using EgdFoundation;
using System;

namespace Core.Framework
{
    public class DataManager : PersistentSingleton<DataManager>
    {
        private UserData _userData;
        private const string USER_DATA_PATH = "UserData.json";

        public UserData UserData
        {
            get
            {
                if (_userData == null)
                {
                    _userData = LoadDataFromJson<UserData>(USER_DATA_PATH);
                }
                return _userData;
            }
        }

        public void UpdateCoin(int coinAmount)
        {
            _userData.Coin += coinAmount;
            SavePlayerData();
            SignalBus.I.FireSignal<UpdateCoin>(new UpdateCoin());
        }

        public void UpdateHighestScore(int score)
        {
            _userData.HighestScore = score;
            SavePlayerData();
        }

        public void SavePlayerData()
        {
            SaveDataToJson(USER_DATA_PATH, _userData);
        }

        private void SaveDataToJson(string jsonFilePath, object dataObject)
        {
            jsonFilePath = Application.persistentDataPath + jsonFilePath;
            string jsonData = JsonUtility.ToJson(dataObject);
            File.WriteAllText(jsonFilePath, jsonData);
        }

        private void OnApplicationQuit()
        {
            SavePlayerData();
        }

        private T LoadDataFromJson<T>(string jsonFilePath)
            where T : class, new()
        {
            jsonFilePath = Application.persistentDataPath + jsonFilePath;
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                if (string.IsNullOrEmpty(jsonData))
                {
                    Debug.LogWarning("Could not load data from " + jsonFilePath);
                }
                return JsonUtility.FromJson<T>(jsonData);
            }
            else
            {
                Debug.LogWarning("JSON file does not exist.");
                return new T();
            }
        }
    }
}