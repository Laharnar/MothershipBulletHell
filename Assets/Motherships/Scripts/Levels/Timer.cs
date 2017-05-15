using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

    float timerStart;

    public int countMinutes;
    public int countSeconds;

    private int timerSeconds;

    public bool timerOn = true;

    // When countdown of timer completes,  message for the function will be sent to target object.
    public Transform sendMessageTarget;
    public string sendMessageMethodName;

    public TimeDisplay timerDisplay;

    void Start() {

        StartTimer();
    }

	void StartTimer () {

        timerSeconds = countMinutes * 60 + countSeconds % 60;

        timerStart = Time.time;

        StartCoroutine(Countdown());
	}

    IEnumerator Countdown() {
        float countTo = timerStart + timerSeconds;
        // run while time didnt run out or timer isnt on
        while (true) {
            if (Time.time > countTo) {
                break;
            }
            if (!timerOn) {
                timerDisplay.SetTime(0, -1);
                yield return null;
                continue;
            }

            float timeLeft = countTo - Time.time;
            timerDisplay.SetTime((int)(timeLeft / 60), (int)timeLeft % 60);
            
            yield return null;
        }

        // countdown has succesfuly completed
        if (timerOn) {
            timerDisplay.TimeOut(Color.red);
            OnCountdownComplete();
        }

    }

    private void OnCountdownComplete() {
        Debug.Log("Blast the player to scraps.");
        sendMessageTarget.SendMessage(sendMessageMethodName);
    }
}
