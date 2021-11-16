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
    private GameObject gameOverPanel;
    private GameObject attackModal;

    //Other Text Containers
    private TMP_Text lbTitleText;

    private TMP_Text lbTimer;
    private timer_class timer;

    private int lives;
    private Image[] livesImages;
    private Image[] stars;
    private bool[] starAchievements;

    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite filledStar;

    [SerializeField]private Sprite [] titleBarImages;

    //Question Containers
    private TMP_Text lbQuestion;
    private TMP_Text lbChoiceA;
    private TMP_Text lbChoiceB;
    private TMP_Text lbChoiceC;
    private TMP_Text lbChoiceD;

    //Prefabs
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private GameObject listParent;

    //Buttons
    GameObject btnTitleBar;

    //Cameras
    GameObject cmCamera;

    //Questions
    List<question_class> questionsList;

    //Variables
    int choiceSelected;
    [SerializeField] private player_controller player;
    [SerializeField] private enemy_controller enemy;

    private void Awake() {
        questionsPanel = GameObject.Find("ui_question_main_panel");
        settingsPanel = GameObject.Find("settingsPanel");
        gameOverPanel = GameObject.Find("gameOverPanel");
        attackModal = GameObject.Find("attackModal");

        lbTitleText = GameObject.Find("text_title_bar").GetComponent<TMP_Text>();
        lbQuestion = GameObject.Find("question_text").GetComponent<TMP_Text>();

        lbChoiceA = GameObject.Find("lbAnswerA").GetComponent<TMP_Text>();
        lbChoiceB = GameObject.Find("lbAnswerB").GetComponent<TMP_Text>();
        lbChoiceC = GameObject.Find("lbAnswerC").GetComponent<TMP_Text>();
        lbChoiceD = GameObject.Find("lbAnswerD").GetComponent<TMP_Text>();

        lbTimer = GameObject.Find("lbTimer").GetComponent<TMP_Text>();
        timer = gameObject.AddComponent<timer_class>();

        livesImages = GameObject.Find("lives").GetComponentsInChildren<Image>();
        lives = livesImages.Length;

        player.GetComponent<player_class>().setStats(lives);    //Set Player HP

        btnTitleBar = GameObject.Find("titleBarBtn");
        cmCamera = GameObject.Find("cmMainCamera");
    }

    // Start is called before the first frame update
    void Start()
    {
        resumeGame();
        toggleAttackModal();
        toggleQuestionPanel(false);
        toggleGameOverScreen();
        //attackModal.SetActive(false);
        //scene_loader.doneLoading = true;

        StartGame();
    }
    #region Functions
    private void StartGame() {
        starAchievements = new bool[] { true, true, true };
        StartCoroutine(
            queryDatabase(
                "*", "v_random_questions", 
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
            timer.stopTimer();
            StartCoroutine(gameOverSequence(3f,true));
            Debug.Log("No Questions Left");
        }
    }

    private void updateHpBar() {
        for (int n = 0, length = livesImages.Length; n < length; n++) {
            if(n+1 > lives) {
                livesImages[n].sprite = emptyHeart;
            }
        }
    }

    private void loadRankings(JSONArray result) {
        if(result.Count <= 0) {
            Debug.Log("No Rankings for this difficulty");
            return;
        }

        for(int n = 0, len = result.Count; n < len; n++) {
            GameObject obj = GameObject.Instantiate(rowPrefab,listParent.transform);
            obj.GetComponent<player_row>().setValues(
                (n + 1).ToString(),                                                     //Rank N
                result[n]["name"],                                                      //Name
                addZeroes(result[n]["minutes"]) + ":"+ addZeroes(result[n]["seconds"])  //Time
            );
        }
    }
    private string addZeroes(int value) {
        if (value < 10) {
            return "0" + value.ToString();
        }
        return value.ToString();
    }
    //Unicode Characters s
    char squareRoot = '\u221a';
    private string parseHtmlText(string value) {
        string temp = value.Replace("%E2%88%9A", squareRoot.ToString());
        return temp;
    }
    #endregion
    #region IEnumerators
    int rSize;
    bool isAttacking = false;
    bool insertingData = false;
    IEnumerator gameOverSequence(float waitTime,bool winLose) {
        starAchievements[0] = winLose;
        int [] timeSpent = timer.getTime();
        starAchievements[2] = timeSpent[0] < 5 && winLose;

        toggleTitleBarBtn();
        if (winLose) {
            playUi("winner");
        } else {
            playUi("gameover");
        }
        yield return new WaitForSecondsRealtime(waitTime);
        //yield return new WaitForSeconds(waitTime);
        toggleGameOverScreen();

        //Get Textfields
        TMP_Text titleText = GameObject.Find("txt_gameOver").GetComponent<TMP_Text>();
        TMP_Text rankingText = GameObject.Find("lb_HS").GetComponent<TMP_Text>();
        TMP_Text timerText = GameObject.Find("lbDetails3").GetComponent<TMP_Text>();

        stars = GameObject.Find("stars").GetComponentsInChildren<Image>();

        //Load Achievements
        titleText.SetText(winLose? "Mission Complete" : "Game Over");
        rankingText.SetText("Rankings (" + constant_variables.getDifficultyName() + ")");

        int[] timerLimit  = constant_variables.getTimerLimit();
        timerText.SetText("- Complete under "+constant_variables.getTimeText(timerLimit)+" minutes (" + timer.getTimeText() + ")");

        //Insert Results to DB
        string name = constant_variables.PLAYER_NAME;
        int diff = constant_variables.difficultySelected;
        int[] time = timer.getTime();
        bool addToHighScores = true;
        //Load stars
        yield return new WaitForSeconds(1f);
        for(int n = 0; n < 3; n++) {
            if (starAchievements[n]) {
                stars[n].sprite = filledStar;
            } else {
                addToHighScores = false;
            }
            playUi("click");
            yield return new WaitForSeconds(0.5f);
        }

        if (addToHighScores) {
            IEnumerator task = insertValues("highscores", "id,name,difficulty,minutes,seconds", "NULL,'" + name + "'," + diff + "," + time[0] + "," + time[1]);
            StartCoroutine(task);

            yield return new WaitUntil(() => insertingData == false);
        }
        //Load Rankings
        StartCoroutine(
            queryDatabase("*","highscores","WHERE difficulty="+constant_variables.difficultySelected+" ORDER BY minutes ASC, seconds ASC",1)
        );
        Debug.Log("Game Over");
        yield return null;
    }

    IEnumerator attackSequence() {
        isAttacking = true;
        yield return new WaitForSeconds(1f);
        //1. Check answer
        if (questionsList[0].isAnswerCorrect(choiceSelected)) {
            player.focusCamToThis();
            player.GetComponent<player_class>().attack();
            mountNextQuestionToUI(true);
        } else {
            enemy.focusCamToThis();
            enemy.GetComponent<player_class>().attack();
            lives--;

            starAchievements[1] = false;
        }
        yield return new WaitForSeconds(1f);
        updateHpBar();
        while (!cmCamera.activeSelf) {
            yield return null;
        }
        toggleTitleBarBtn();
        isAttacking = false;
        if(lives <= 0) {
            timer.stopTimer();
            StartCoroutine(gameOverSequence(3f,false));
        }
        yield return null;
    }

    IEnumerator loadQuestions(JSONArray result) {
        if(scene_loader.getInstance()) {
            scene_loader.getInstance().setLoadingMsg("Loading Questions...");
        }
        if(result.Count == 0) {
            lbTitleText.SetText("Question (0/0)");  //Title
            btnTitleBar.SetActive(false);
            yield break;
        } else {
            btnTitleBar.SetActive(true);
        }

        questionsList = new List<question_class>();

        //Add questions to list in reverse order
        rSize = result.Count;
        enemy.GetComponent<player_class>().setStats(rSize);

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
        timer.startTimer();
        yield return null;
    }
    
    IEnumerator insertValues(string table, string columnNames, string values) {
        insertingData = true;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("select=*&from=users&where="));

        string postData = "tName=" + table + "&cNames=" + columnNames + "&cValues=" + values;

        string uri = constant_variables.getIpAddress() + "insertValues.php?" + postData;
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

            JSONArray jsonArray = result["queryResult"].AsArray;

            if (jsonArray[0]["result"] == "true") {

            } else {
                Debug.Log("Insert Failed");
            }
        }
        insertingData = false;
        yield return null;
    }
    IEnumerator queryDatabase(string select, string from, string where, int processIndex) {
        cmCamera.SetActive(false);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

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
            response = parseHtmlText(response);
            JSONNode result = SimpleJSON.JSON.Parse(response);

            Debug.Log("Server Responded: " + result["result"] + " Rows: " + result["result"].Count + " Columns: " + result["result"][0].Count);

            JSONArray jsonArray = result["result"].AsArray;


            switch (processIndex) {
                case 0: {
                    StartCoroutine(loadQuestions(jsonArray));
                    break;
                }
                case 1: {
                    loadRankings(jsonArray);
                    break;
                }
            }

        }
        cmCamera.SetActive(true);
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
    public void selectAnswer(int choice) {
        playUi("click");
        toggleAttackModal();
        choiceSelected = choice;
    }
    public void attackWithAnswer() {
        if (isAttacking) {
            Debug.Log("Still attacking. Please wait");
            return;
        }
        Debug.Log("Attacking With choice " + choiceSelected);
        toggleQuestionPanel(false);
        toggleTitleBarBtn();
        toggleAttackModal();
        StartCoroutine(attackSequence());
    }
    public void pauseGame() {
        if (scene_loader.doneLoading) {
            playUi("click");
        }
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void resumeGame() {
        if (scene_loader.doneLoading) {
            playUi("cancel");
        }
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void returnToMainMenu() {
        playUi("click");
        Time.timeScale = 1f;
        scene_loader.loasdScene(1);
    }
    public void toggleGameOverScreen() {
        gameOverPanel.SetActive(!gameOverPanel.activeSelf);
    }
    public void toggleAttackModal() {
        attackModal.SetActive(!attackModal.activeSelf);
    }

    public void toggleTitleBarBtn() {
        btnTitleBar.SetActive(!btnTitleBar.activeSelf);
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
    #region Sound Manager Controls
    public void log(string toDebug) {
        Debug.Log(toDebug);
    }
    private void playBgm(string name) {
        playSound(name, 0);
    }
    public void playUi(string name) {
        if(name == "click") {
            playSound(name, 1, 1);
        } else {
            playSound(name, 1);
        }
    }
    private void playSfx(string name) {
        playSound(name, 2);
    }
    private void playSound(string name, int soundType, float pitch = 1f) {
        switch (soundType) {
            case 0: {
                GameObject.FindObjectOfType<sound_manager>().playBgSound(name);
                break;
            }
            case 1: {
                GameObject.FindObjectOfType<sound_manager>().playUiSound(name,false,pitch);
                break;
            }
            case 2: {
                GameObject.FindObjectOfType<sound_manager>().playSfxSound(name,false,pitch);
                break;
            }
        }
    }
    #endregion
}
