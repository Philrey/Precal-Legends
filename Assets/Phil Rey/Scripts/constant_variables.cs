using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class constant_variables
{
    public string VERSTION = "1.0";
    protected static string IP_ADDRESS = "localhost";
    public static string PLAYER_NAME = "Player";

    //Game Settings Set
    public static int difficultySelected { get; set; }


    private static List<question_class> selectedQuestions;

    public static question_class getQuestionDetails(int index) {
        return selectedQuestions[index];
    }
    public static string getDifficultyName() {
        switch (difficultySelected) {
            case 0:
                return "Easy";
            case 1:
                return "Medium";
            case 2:
                return "Hard";
            default:
                return null;
        }
    }
    public static void setIpAddress(string ipAddress) {
        IP_ADDRESS = ipAddress; //localhost
    }

    public static void setPlayerName(string playerName) {
        PLAYER_NAME = playerName;
    }

    public static int[] getTimerLimit() {
        switch (difficultySelected) {
            case 0:
                return new int[] { 5, 0 };
            case 1:
                return new int[] { 10, 0 };
            case 2:
                return new int[] { 15, 0 };
            default:
                return new int[] { 5, 0 };
        }
    }

    public static string getTimeText(int [] time) {
        return addZeroes(time[0]) + ":" + addZeroes(time[1]);
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

    private static string addZeroes(int value) {
        if (value < 10) {
            return "0" + value.ToString();
        }
        return value.ToString();
    }
}
