using System.Collections.Generic;
using SimpleJSON;

public class question_class
{
    private string question;
    private string choice_a;
    private string choice_b;
    private string choice_c;
    private string choice_d;

    private int correct_answer;
    private int qNumber;

    public question_class(JSONNode data, int qNumber) {
        setQuestion(data);
        this.qNumber = qNumber;
    }

    private void setQuestion(JSONNode data) {
        question = data["question"];
        
        choice_a = data["choice_a"];
        choice_b = data["choice_b"];
        choice_c = data["choice_c"];
        choice_d = data["choice_d"];

        correct_answer = data["correct_answer"];
    }

    public int getQuestionNumber() {
        return qNumber;
    }

    public string getQuestion() {
        return question;
    }

    public List<string> getChoices() {
        List<string> choices = new List<string>();

        choices.Add(choice_a);
        choices.Add(choice_b);
        choices.Add(choice_c);
        choices.Add(choice_d);

        return choices;
    }

    public bool isAnswerCorrect(int answerIndex) {
        if(correct_answer == answerIndex) {
            return true;
        }
        return false;
    }
}
