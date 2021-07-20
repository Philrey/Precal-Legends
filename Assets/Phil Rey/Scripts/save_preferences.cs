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

    public static string getSavedIpAddress() {
        return PlayerPrefs.GetString("ipAddress", "localhost"); //Key Name, Default Value if empty
    }
}
