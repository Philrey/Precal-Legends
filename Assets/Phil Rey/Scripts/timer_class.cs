using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class timer_class : MonoBehaviour
{
    int minutes;
    int seconds;

    TMP_Text lbTimer;
    IEnumerator counter;

    void Awake(){
        lbTimer = GameObject.Find("lbTimer").GetComponent<TMP_Text>();
        counter = startTimerCounter();
    }

    public void startTimer() {
        StartCoroutine(counter);
    }

    public void stopTimer() {
        StopCoroutine(counter);
    }

    private string addZeroes(int value) {
        if(value < 10) {
            return "0" + value.ToString();
        }
        return value.ToString();
    }

    private string getTimeText() {
        return addZeroes(minutes) + ":" + addZeroes(seconds);
    }

    IEnumerator startTimerCounter() {
        minutes = 0;
        seconds = 0;
        
        while (true) {
            lbTimer.SetText(getTimeText());
            yield return new WaitForSecondsRealtime(1f);
            if (seconds + 1 >= 60) {
                seconds = 0;
                minutes++;
            }
            seconds++;            
        }
    }

}
