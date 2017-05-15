using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour {

    public Text timeText;

    public void SetTime(int minutes, int seconds) {
        string min = minutes.ToString(), sec = seconds.ToString();
        if (seconds < 10) {
            sec = "0" + sec;
        }
        timeText.text = min + ":" + sec;
    }

    internal void SetColor(Color color) {
        timeText.color = color;
    }

    internal void TimeOut(Color color) {
        SetColor(color);

        GameObject.FindObjectOfType<EndLevel>().PlayerDownLevel();

        StartCoroutine(Blink(0.5f));
    }

    IEnumerator Blink(float time) {
        float t = Time.time +time;
        float blinkRate = 1;

        bool visible = true;
        string text = timeText.text;
        while (true) {
            if (Time.time >t) {
                break;
            }
            // keep blinked text updated[probably obsolete]
            if (visible) {
                text = timeText.text;
            }
            timeText.text = visible ? text : "";
            yield return new WaitForSeconds(blinkRate);
        }
    }
}
