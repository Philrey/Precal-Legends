using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class save_preferences
{
    public static bool SaveIPAddress(string ipAddress) {
        PlayerPrefs.SetString("ipAddress", ipAddress);
        PlayerPrefs.Save();
        return true;
    }

    public static bool SavePlayerName(string playerName) {
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.Save();
        return true;
    }

    public static string getSavedIpAddress() {
        return PlayerPrefs.GetString("ipAddress", "localhost"); //Key Name, Default Value if empty
    }

    public static string getSavedPlayerName() {
        return PlayerPrefs.GetString("playerName", "Player"); //Key Name, Default Value if empty
    }
}
