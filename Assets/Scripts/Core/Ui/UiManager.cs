using System.Collections.Generic;
using UnityEngine;
using EgdFoundation;

    public class UiManager : Singleton<UiManager> {
        private List<ScreenItem> _screens = new List<ScreenItem>();
        private List<QueueScreenItem> _queuePush = new List<QueueScreenItem>();
        private List<ScreenItem> _queuePop = new List<ScreenItem>();
        private enum UiState { free, pushing, popping }
        private UiState currentState = UiState.free;
        private class QueueScreenItem {
            public string screenName;
            public ScreenData screenData;

            public QueueScreenItem(string screenName, ScreenData data) {
                this.screenName = screenName;
                this.screenData = data;
            }
        }
        private void Update() {
            if (currentState != UiState.free) {
                return;
            }
            if (_queuePop.Count > 0) {
                if (_screens.Count > 0 && _screens[0] == _queuePop[0]) {
                    currentState = UiState.popping;
                    _screens[0].OnPopSuccess = PopScreenSuccess;
#pragma warning disable 612, 618
                    _screens[0].Back();
                    return;
                }
                else {
                    _queuePop.RemoveAt(0);
                    return;
                }
            }
            if (_queuePush.Count > 0) {
                QueueScreenItem pushQueue = _queuePush[0];
                GameObject pfbScreen = GetPrefab(pushQueue.screenName);
                if (pfbScreen == null) {
                    _queuePush.RemoveAt(0);
                    return;
                }
                GameObject objScreen = GameObject.Instantiate(pfbScreen);
                ScreenItem screenItem = objScreen.GetComponent<ScreenItem>();
                if (screenItem == null) {
                    GameObject.Destroy(objScreen);
                    _queuePush.RemoveAt(0);
                    currentState = UiState.free;
                }
                else {
                    currentState = UiState.pushing;
                    screenItem.prefabName = pushQueue.screenName;
                    screenItem.OnPushSuccess = PushScreenSuccess;
                    Debug.Log($"====== screen item pushed with data {pushQueue.screenData != null}");
                    screenItem.Push(pushQueue.screenData);

                    if (_screens.Count > 0 && _screens[0].KeepScreenWork == false && screenItem.KeepOldScreenWork == false) {
                        _screens[0].tween.SetBackDirectionAndEnabled();
                    }

                    _screens.Insert(0, screenItem);
                    _screens[0].canvasRoot.sortingOrder = _screens.Count * 10;
                }
            }
        }
        private void PushScreenSuccess() {
            currentState = UiState.free;
            _queuePush.RemoveAt(0);
            _screens[0].tween.SetForwardDirectionAndEnabled();
            if (_screens.Count > 1) {
                for (int i = 1; i < _screens.Count; i++) {
                    if (_screens[i] != null && _screens[0].KeepScreenWork != false && _screens[i].KeepOldScreenWork != false) {
                        _screens[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        private void PopScreenSuccess() {
            currentState = UiState.free;
            _queuePop.RemoveAt(0);
            _screens.RemoveAt(0);
            if (_screens.Count > 0) {
                _screens[0].gameObject.SetActive(true);
                _screens[0].tween.SetForwardDirectionAndEnabled();
                _screens[0].ReSetup();
            }
        }
        private GameObject GetPrefab(string sceneName) {
            return Resources.Load<GameObject>($"Prefabs/UI/{sceneName}");
        }
        public void Push(string screenName, ScreenData screenData = null) {
            QueueScreenItem queueItem = new QueueScreenItem(screenName, screenData);
            Debug.Log($"========== Screen data is null: {screenData == null} =============");
            _queuePush.Add(queueItem);
        }
        public void Pop() {
            if (_screens.Count <= 0) {
                return;
            }
            _queuePop.Insert(0, _screens[0]);
        }
        public void PopToScreen<T>() where T : ScreenItem {
            if (GetScreen<T>() == null) {
                return;
            }
            for (int i = 0; i < _screens.Count; i++) {
                if (!_screens[i] is T) {
                    _queuePop.Add(_screens[i]);
                }
                else {
                    break;
                }
            }
        }
        public void PopToScreen(ScreenItem item) {
            if (!_screens.Contains(item)) return;
            for (int i = 0; i < _screens.Count; i++) {
                if (!_screens[i].Equals(item)) {
                    _queuePop.Add(_screens[i]);
                }
                else {
                    break;
                }
            }
        }
        public T GetTop<T>() where T : ScreenItem {
            if (_screens.Count > 0 && _screens[0] is T) {
                return _screens[0] as T;
            }
            return null;
        }
        public T GetScreen<T>() where T : ScreenItem {
            if (_screens.Count <= 0) return null;
            for (int i = 0; i < _screens.Count; i++) {
                if (_screens[i] is T)
                    return _screens[i] as T;
            }
            return null;
        }
        public List<ScreenItem> GetCurrentScreen() {
            return _screens;
        }
    }
