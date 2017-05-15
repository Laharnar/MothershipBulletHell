using UnityEngine;
using System.Collections;

public class EndLevel : MonoBehaviour {

    static bool playerDown;


    public int winScene;
    public int loseScene;
    public FadeEffect fading;

    public GoalsDisplay[] requiredForWin;

	// Use this for initialization
	void Start () {
        StartCoroutine(LevelLoop());
	}
	
	// Update is called once per frame
	IEnumerator LevelLoop () {
        while (true) {
            bool end = true;
            for (int i = 0; i < requiredForWin.Length; i++) {
                if (!requiredForWin[i].goalReached) {
                    end = false;
                    break;
                }
            }
            // all goals were reached
            if (end) {
                FadeWinLoseScreen(winScene);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
	}

    private void FadeWinLoseScreen(int sceneId) {
        fading.levelId = sceneId;
        StartCoroutine(fading.EndScene());
    }

    public void PlayerDownLevel() {
        playerDown = true;

        FadeWinLoseScreen(loseScene);
    }
}
