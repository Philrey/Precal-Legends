using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom Includes
using TMPro;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
using DG.Tweening;

public class ui_manager_main_menu : MonoBehaviour
{
    //Panels
    private GameObject[] panels;
    private GameObject startPanel;
    private GameObject difficultyPanel;
    private GameObject summaryPanel;
    private GameObject settingsPanel;
    
    //Fields
    private TMP_InputField tfIpAddress;
    private IEnumerator loadTabThread;

    int tabSelected = 0;
    // Start is called before the first frame update
    void Start()
    {
        initialize_ui();
        scene_loader.doneLoading = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            backBtnPressed();
        }
    }
    void initialize_ui() {
        startPanel = GameObject.Find("start_panel");
        difficultyPanel = GameObject.Find("difficulty_panel");
        summaryPanel = GameObject.Find("summary_panel");

        settingsPanel = GameObject.Find("settings_panel");
        tfIpAddress = GameObject.Find("tfIpAddressSelected").GetComponent<TMP_InputField>();

        panels = new GameObject[] { 
            startPanel,difficultyPanel,summaryPanel
        };

        toggleSettings(false);
        selectTab(0);
        loadSavedIpAddress();
    }
    #region Functions
    private void backBtnPressed() {
        if (settingsPanel.activeSelf) {
            toggleSettings();
        } else {
            if(tabSelected != 0) {
                playUi("cancel");
                selectTab(tabSelected-1);
                return;
            }
        }
    }
    public void selectTab(int index) {
        tabSelected = index;
        for (int n = 0,len = panels.Length; n < len; n++) {
            if (n == index && !panels[n].activeSelf) {
                panels[n].SetActive(true);
            }else if (n != index) {
                panels[n].SetActive(false);
            }
        }
    }

    public void loadSavedIpAddress() {
        string currIp = save_preferences.getSavedIpAddress();
        constant_variables.setIpAddress(currIp);
        tfIpAddress.text = currIp;
    }

    public void log(string toDebug) {
        Debug.Log(toDebug);
    }
    private void playBgm(string name) {
        playSound(name, 0);
    }
    private void playUi(string name) {
        playSound(name, 1);
    }
    private void playSfx(string name) {
        playSound(name, 2);
    }
    private void playSound(string name, int soundType) {
        switch (soundType) {
            case 0: {
                GameObject.FindObjectOfType<sound_manager>().playBgSound(name);
                break;
            }
            case 1: {
                GameObject.FindObjectOfType<sound_manager>().playUiSound(name);
                break;
            }
            case 2: {
                GameObject.FindObjectOfType<sound_manager>().playSfxSound(name);
                break;
            }
        }
    }
    #endregion

    #region Button Functions
    public void SaveIpAddress() {
        save_preferences.SaveIPAddress(tfIpAddress.text);
        constant_variables.setIpAddress(tfIpAddress.text);
        toggleSettings();
    }

    public void play() {
        playUi("click");
        selectTab(1);
    }

    public void startGame() {
        playUi("click");
        scene_loader.loasdScene(2);
    }

    public void toggleSettings(bool playSound = true) {
        if (!settingsPanel.activeSelf) {
            loadSavedIpAddress();
            if (playSound) { playUi("click"); }            
        } else {
            if (playSound) { playUi("cancel"); }
        }
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void setDifficulty(int index) {
        switch (index) {
            case 0: {
                //lb_difficulty.text = "Easy";
                constant_variables.difficultySelected = index;
                break;
            }
            case 1: {
                //lb_difficulty.text = "Medium";
                constant_variables.difficultySelected = index;
                break;
            }
            case 2: {
                //lb_difficulty.text = "Hard";
                constant_variables.difficultySelected = index;
                break;
            }
        }
        playUi("click");
        selectTab(2);
    }
    #endregion
    #region Threads

    private IEnumerator query_db_and_execute_command(string select, string from, string where, int functionIndex) {


        yield return null;
    }
    #endregion
}
