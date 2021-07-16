using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class constant_variables
{
    public string VERSTION = "1.0";
    protected static string IP_ADDRESS = "http://localhost/precal";

    //Game Settings Set
    public static int difficultySelected { get; set; }


    private static List<question_class> selectedQuestions;

    public static question_class getQuestionDetails(int index) {
        return selectedQuestions[index];
    }

    public static int getQSize() {
        return selectedQuestions.Count;
    }


    protected string getCustomIpAddress(string hostName) {
        return $"http://{hostName}/precal";
    }
}
