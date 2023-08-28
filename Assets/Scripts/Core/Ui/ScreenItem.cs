using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityExtensions.Tween;


    public abstract class ScreenItem : MonoBehaviour {
        protected ScreenData _screenData;
        public Action OnPushSuccess { get; set; }
        public Action OnPopSuccess { get; set; }
        public bool KeepScreenWork;
        public bool KeepOldScreenWork;
        [HideInInspector] public string prefabName;
        [HideInInspector] public Canvas canvasRoot;
        public TweenPlayer tween;
#pragma warning disable 612, 618
        [Obsolete("DONT CALL THIS METHOD, Call CloseScreen() instead", false)]
        public void Back() {
            this.StartCoroutine(IE_PoppingScreen());
        }
        public void Close() {
            UiManager.I.Pop();
        }
        public void Push(ScreenData screenData = null) {
            _screenData = screenData;
            canvasRoot = GetComponent<Canvas>();
            this.StartCoroutine(IE_PushingScreen());

        }
        protected abstract IEnumerator Setup(ScreenData screenData = null);
        public abstract void ReSetup();
        protected abstract IEnumerator OnPushScreen(ScreenData screenData = null);
        protected abstract IEnumerator OnPopScreen();
        protected IEnumerator IE_PushingScreen() {
            yield return Setup(_screenData);
            tween.normalizedTime = 0;
            yield return OnPushScreen(_screenData);
            OnPushSuccess?.Invoke();
        }
        protected IEnumerator IE_PoppingScreen() {
            tween.normalizedTime = 1;
            yield return OnPopScreen();
            OnPopSuccess?.Invoke();
            tween.SetBackDirectionAndEnabled();
            yield return new WaitForSeconds(tween.duration);
            GameObject.Destroy(gameObject);
        }
    }
    public class ScreenData {
        public Dictionary<string, object> screenData;
        public Dictionary<string, Action> screenAction;
        public ScreenData() {
            screenData = new Dictionary<string, object>();
            screenAction = new Dictionary<string, Action>();
        }
    }
