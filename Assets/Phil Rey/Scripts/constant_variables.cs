using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class constant_variables
{
    public string VERSTION = "1.0";
    protected static string IP_ADDRESS = "localhost";

    //Game Settings Set
    public static int difficultySelected { get; set; }


    private static List<question_class> selectedQuestions;

    public static question_class getQuestionDetails(int index) {
        return selectedQuestions[index];
    }

    public static void setIpAddress(string ipAddress) {
        IP_ADDRESS = ipAddress; //localhost
    }

    public static int getQSize() {
        return selectedQuestions.Count;
    }

    public static string getIpAddress(bool hostNameOnly = false) {
        return hostNameOnly ? IP_ADDRESS : getCustomIpAddress(IP_ADDRESS);  // localhost : http://localhost/precal/
    }

    public static string getCustomIpAddress(string hostName) {
        return $"http://{hostName}/precal/";
    }
}
