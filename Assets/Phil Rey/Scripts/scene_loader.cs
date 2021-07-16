using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Michsky.UI.ModernUIPack;
using TMPro;

public class scene_loader : MonoBehaviour
{
    public static bool doneLoading;
    private static bool doneTransition;

    private static scene_loader instance;

    public Image fader;
    public ProgressBar progressBar;
    public TMP_Text lbProgressText;

    private Color faderColor;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        firstRun();
    }
    public void setLoadingMsg(string txt) {
        lbProgressText.SetText(txt);
    }
    public static scene_loader getInstance() {
        return instance;
    }

    private void firstRun() {
        fader.gameObject.SetActive(true);
        Color curColor = fader.color;
        curColor.a = 1;
        fader.color = curColor;
        doneTransition = true;
        StartCoroutine(loadScene(1, true));
    }

    private void fadeIn() {
        if (!DOTween.IsTweening(fader)) {
            StartCoroutine(fade(true));
        }
    }

    private void fadeOut() {
        if (!DOTween.IsTweening(fader)) {
            StartCoroutine(fade(false));
        }
    }

    private IEnumerator fade(bool inOut) {
        doneTransition = false;

        Color curColor = fader.color;
        Color targetColor = fader.color;
        targetColor.a = inOut ? 1 : 0;

        fader.gameObject.SetActive(true);
        fader.DOColor(targetColor, 1f);

        while (DOTween.IsTweening(fader)) {
            yield return null;
        }
        fader.gameObject.SetActive(inOut);
        doneTransition = true;

        yield return null;
    }

    public IEnumerator loadScene(int index, bool firstRun = false) {
        doneLoading = false;

        if (!firstRun) {
            fadeIn();
        }
        while (!doneTransition) {
            yield return null;
        }

        switch (index) {
            case 2: {
                Screen.orientation = ScreenOrientation.Landscape; break;
            }default: {
                Screen.orientation = ScreenOrientation.Portrait; break;
            }
        }

        progressBar.gameObject.SetActive(true);
        progressBar.currentPercent = 0;

        AsyncOperation ao = SceneManager.LoadSceneAsync(index);
        ao.allowSceneActivation = false;
        
        while (!ao.isDone) {
            progressBar.currentPercent = ao.progress * 100;

            if(ao.progress >= 0.9f) {
                progressBar.currentPercent = 100;
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
        //Must be set true by main scenes by calling "scene_loader.doneLoading = true;" after initialization
        while (!doneLoading) {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        fadeOut();
        progressBar.gameObject.SetActive(false);

        yield return null;
    }

    public static void loasdScene(int sceneIndex) {
        instance.StartCoroutine(instance.loadScene(sceneIndex));
    }
}
