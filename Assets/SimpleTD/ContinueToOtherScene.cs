using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueToOtherScene : MonoBehaviour {

	public void ContinueToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
