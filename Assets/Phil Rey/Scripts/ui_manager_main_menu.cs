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
    
    
    public ModalWindowManager loading_modal;
    public TMP_Text lb_difficulty;
    [Header("Tab Selection")]
    private bool leftRightDirection;
    public HorizontalSelector tab_selector;
    public GameObject btn_previous;
    public GameObject btn_next;
    public Transform[] tab_panels;

    private IEnumerator loadTabThread;
    // Start is called before the first frame update
    void Start()
    {
        initialize_ui();
        scene_loader.doneLoading = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void initialize_ui() {
        selectTab(true);
    }

    public void selectTab(bool direction) {
        if (isLoadingTab) {
            return;
        }
        leftRightDirection = direction;
        StartCoroutine(loadTabs(tab_selector.index));
    }
    public void play() {
        btn_next.GetComponent<Button>().onClick.Invoke();
    }
    public void setDifficulty(int index) {
        switch (index) {
            case 0: {
                lb_difficulty.text = "Easy";
                break;
            }
            case 1: {
                lb_difficulty.text = "Medium";
                break;
            }
            case 2: {
                lb_difficulty.text = "Hard";
                break;
            }
        }
        btn_next.GetComponent<Button>().onClick.Invoke();
    }

    public void log(string toDebug) {
        Debug.Log(toDebug);
    }
    #region Threads

    private bool isLoadingTab;
    private IEnumerator loadTabs(int index) {
        isLoadingTab = true;

        if(index == 0 && leftRightDirection && tab_panels[index].gameObject.activeSelf) {
            yield break;
        } 
        if (index == 2 && !leftRightDirection && tab_panels[index].gameObject.activeSelf) {
            yield break;
        }

        //Enable/Disable Buttons
        btn_next.SetActive(index == 2 || index == 0? false : true);
        btn_previous.SetActive(index == 0 ? false : true);

        RectTransform previous = tab_panels[leftRightDirection ? index + 1 : index - 1].GetComponent<RectTransform>();
        RectTransform next = tab_panels[index].GetComponent<RectTransform>();

        //Activate and move to center
        next.gameObject.SetActive(true);
        next.DOAnchorPosX(0, 0.5f, true);

        previous.DOAnchorPosX(leftRightDirection? 1000 : -1000, 0.5f, true);
        while (DOTween.IsTweening(next)) {
            yield return null;
        }
        previous.gameObject.SetActive(false);

        isLoadingTab = false;
        yield return null;
    }

    private IEnumerator query_db_and_execute_command(string select, string from, string where, int functionIndex) {


        yield return null;
    }
    #endregion
}
