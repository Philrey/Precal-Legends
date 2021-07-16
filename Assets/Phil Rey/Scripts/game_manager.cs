using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class game_manager: MonoBehaviour {
    //Panels
    private GameObject questionsPanel;
    private GameObject settingsPanel;
    private GameObject attackModal;

    //Other Text Containers
    private TMP_Text lbTitleText;
    [SerializeField]private Sprite [] titleBarImages;

    //Question Containers
    private TMP_Text lbQuestion;
    private TMP_Text lbChoiceA;
    private TMP_Text lbChoiceB;
    private TMP_Text lbChoiceC;
    private TMP_Text lbChoiceD;

    //Buttons
    GameObject btnTitleBar;

    private void Awake() {
        questionsPanel = GameObject.Find("ui_question_main_panel");
        settingsPanel = GameObject.Find("settingsPanel");
        attackModal = GameObject.Find("attackModal");

        lbTitleText = GameObject.Find("text_title_bar").GetComponent<TMP_Text>();
        lbQuestion = GameObject.Find("question_text").GetComponent<TMP_Text>();

        lbChoiceA = GameObject.Find("lbAnswerA").GetComponent<TMP_Text>();
        lbChoiceB = GameObject.Find("lbAnswerB").GetComponent<TMP_Text>();
        lbChoiceC = GameObject.Find("lbAnswerC").GetComponent<TMP_Text>();
        lbChoiceD = GameObject.Find("lbAnswerD").GetComponent<TMP_Text>();

        btnTitleBar = GameObject.Find("titleBarBtn");
    }

    // Start is called before the first frame update
    void Start()
    {
        toggleAttackModal();
        toggleQuestionPanel(false);
        //attackModal.SetActive(false);
        scene_loader.doneLoading = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region IEnumerators
    IEnumerator moveQuestionPanel(float endValue, float titleBarEndValue) {
        RectTransform rect = questionsPanel.GetComponent<RectTransform>();
        RectTransform rect2 = btnTitleBar.GetComponent<RectTransform>();

        rect.DOAnchorPosY(endValue, 0.5f, true);
        rect2.DOAnchorPosY(titleBarEndValue, 0.45f, true);

        while (DOTween.IsTweening(rect)) {
            yield return null;
        }

        yield return null;
    }

    #endregion
    #region Button Functions
    public void pauseGame() {

    }

    public void resumeGame() {

    }

    public void toggleAttackModal() {
        attackModal.SetActive(!attackModal.activeSelf);
    }

    public void toggleQuestionPanel(bool showHide) {
        if (!DOTween.IsTweening(questionsPanel.GetComponent<RectTransform>())) {
            if (showHide) {
                StartCoroutine(moveQuestionPanel(360f,665f));
                btnTitleBar.GetComponent<Button>().interactable = false;
                btnTitleBar.GetComponent<Image>().sprite = titleBarImages[0];
            } else {
                StartCoroutine(moveQuestionPanel(-370f, 54f));
                btnTitleBar.GetComponent<Button>().interactable = true;
                btnTitleBar.GetComponent<Image>().sprite = titleBarImages[1];
            }
        }
    }
    #endregion
}
