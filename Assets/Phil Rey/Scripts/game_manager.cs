using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

using UnityEngine.Networking;
using SimpleJSON;

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

    //Questions
    List<question_class> questionsList;

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
        resumeGame();
        toggleAttackModal();
        toggleQuestionPanel(false);
        //attackModal.SetActive(false);
        //scene_loader.doneLoading = true;

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region Functions
    private void StartGame() {
        StartCoroutine(
            queryDatabase(
                "*", "questions", 
                "WHERE difficulty='" + constant_variables.difficultySelected + "'",
                0
            )
        );
    }

    private void mountNextQuestionToUI(bool popFirstIndex = false) {
        if (popFirstIndex && questionsList.Count > 0) {
            Debug.Log("Popping Question");
            questionsList.RemoveAt(0);
        }

        if (questionsList.Count > 0) {
            //Put values to UI
            lbTitleText.SetText(
                "Question (" + questionsList[0].getQuestionNumber() +
                "/" + rSize + ")"
            );  //Title

            lbQuestion.SetText(questionsList[0].getQuestion()); //Question

            lbChoiceA.SetText(questionsList[0].getChoices()[0]);//A
            lbChoiceB.SetText(questionsList[0].getChoices()[1]);//B
            lbChoiceC.SetText(questionsList[0].getChoices()[2]);//C
            lbChoiceD.SetText(questionsList[0].getChoices()[3]);//D
        } else {
            //End the game
            Debug.Log("No Questions Left");
        }
    }
    #endregion
    #region IEnumerators
    int rSize;
    IEnumerator loadQuestions(JSONArray result) {
        if(scene_loader.getInstance()) {
            scene_loader.getInstance().setLoadingMsg("Loading Questions...");
        }
        if(result.Count == 0) {
            lbTitleText.SetText("Question (0/0)");  //Title
            btnTitleBar.SetActive(false);
        } else {
            btnTitleBar.SetActive(true);
        }

        questionsList = new List<question_class>();

        //Add questions to list in reverse order
        rSize = result.Count;

        for (int n=0; n < rSize; n++) {
            if (scene_loader.getInstance()) {
                scene_loader.getInstance().setLoadingMsg("Loading Question " + (n + 1) + " of " + rSize);
            }
            questionsList.Add(new question_class(result[n], n + 1));
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        if (scene_loader.getInstance()) {
            scene_loader.getInstance().setLoadingMsg("Mounting Questions");
        }
        mountNextQuestionToUI();
        scene_loader.doneLoading = true;

        yield return null;
    }
    
    IEnumerator queryDatabase(string select, string from, string where, int processIndex) {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("select=*&from=users&where="));

        string postData = "select=" + select + "&from=" + from + "&where=" + where;

        string uri = constant_variables.getIpAddress() + "returnValues.php?" + postData;

        Debug.Log(uri);

        UnityWebRequest httpRequest = UnityWebRequest.Get(uri);
        httpRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return httpRequest.SendWebRequest();

        if (httpRequest.result == UnityWebRequest.Result.ConnectionError
            || httpRequest.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log("Error Found..." + httpRequest.error);

        } else {
            string response = httpRequest.downloadHandler.text;
            JSONNode result = SimpleJSON.JSON.Parse(response);

            Debug.Log("Server Responded: " + result["result"] + " Rows: " + result["result"].Count + " Columns: " + result["result"][0].Count);

            JSONArray jsonArray = result["result"].AsArray;


            switch (processIndex) {
                case 0: {
                    StartCoroutine(loadQuestions(jsonArray));
                    break;
                }
                case 1: {
                    //StartCoroutine(loadRoomsThread(jsonArray));
                    break;
                }
            }

        }
        yield return null;
    }

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
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void resumeGame() {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
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
