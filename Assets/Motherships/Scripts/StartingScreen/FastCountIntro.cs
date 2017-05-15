using UnityEngine;
using System.Collections;

/// <summary>
/// Handles first scene, where counter goes up to 2 minutes, before mission starts.
/// </summary>
[@RequireComponent(typeof(TimeDisplay))]
[@RequireComponent(typeof(FadeEffect))]
public class FastCountIntro : MonoBehaviour {
    
    //how much to count to
    public int maxSeconds;
    TimeDisplay countDisplay;
    public float displayRate = 0.1f;// how much to wait between update

    FadeEffect sceneLoader;

    void Start() {
        sceneLoader = GetComponent<FadeEffect>();
        countDisplay = GetComponent<TimeDisplay>();
        StartCoroutine(Count());
    }

    private IEnumerator Count() {
        yield return new  WaitForSeconds(2);
        int counter = 0;
        while (counter < maxSeconds) {
            counter++;
            countDisplay.SetTime((int)(counter/60), counter%60);
            yield return new WaitForSeconds(displayRate);
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(sceneLoader.EndScene());
    }
}
