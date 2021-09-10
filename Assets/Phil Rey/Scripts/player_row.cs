using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class player_row : MonoBehaviour
{
    TMP_Text[] contents;

    public void setValues(string rank,string name,string time) {
        contents = gameObject.GetComponentsInChildren<TMP_Text>();

        contents[0].SetText(rank);
        contents[1].SetText(name);
        contents[2].SetText(time);
    }
}
