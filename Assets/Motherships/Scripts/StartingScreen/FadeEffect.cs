using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour {

    public float fadeSpeed = 1.5f;
    public Color col = Color.black;

    public Image image;

    public int levelId = 0;

    public bool levelStartFade = false;

    void Awake() {
            
        StartCoroutine(StartScene());
    }


    void FadeToClear() {
        // Lerp the colour of the texture between itself and transparent.
        image.color = Color.Lerp(image.color, Color.clear, fadeSpeed * Time.deltaTime);
    }


    void FadeToBlack() {
        // Lerp the colour of the texture between itself and black.
        image.color = Color.Lerp(image.color, col, fadeSpeed * Time.deltaTime);
    }


    IEnumerator StartScene() {
        bool sceneStarting = levelStartFade;
        if (!sceneStarting) {
            image.color = Color.clear;
            yield break;
        }

        image.color = col;
        while (true) {
            // Fade the texture to clear.
            FadeToClear();

            // If the texture is almost clear...
            if (image.color.a <= 0.01f) {
                // ... set the colour to clear and disable the GUITexture.
                image.color = Color.clear;
                image.enabled = false;

                // The scene is no longer starting.
                sceneStarting = false;//[obsolete]
                break;
            }
            yield return null;
        }
        
    }


    public IEnumerator EndScene() {

        bool fading = true;//[obsolete]
        while (fading) {
            // Make sure the texture is enabled.
            image.enabled = true;

            // Start fading towards black.
            FadeToBlack();

            // If the screen is almost black...
            if (image.color.a >= 0.99f) {
                // ... reload the level.
                Application.LoadLevel(levelId);
                fading = false;//[obsolete]
                break;
            }
            yield return null;
        }

    }
}
