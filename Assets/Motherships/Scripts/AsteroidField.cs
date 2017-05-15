using UnityEngine;
using System.Collections;

public class AsteroidField : MonoBehaviour {

    public Vector2 fieldSize;
    public int density = 4; // up to num of asteroids per field

    public int curFieldSize;
    Vector3[] grid;

    public float maxSizeRelativeToField = 1;
    public float minSizeRelativeToField = 0.5f;

    public Transform asteroidPref;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < curFieldSize; i++) {
            for (int j = 0; j < curFieldSize; j++) {
                int rand = Random.Range(0, density);
                Vector3 gridPos = new Vector2(i * fieldSize.x, j * fieldSize.y)
                    - new Vector2(curFieldSize * fieldSize.x / 2, curFieldSize * fieldSize.y / 2);

                for (int r = 0; r< rand; r++) {
                    Vector3 randInField = new Vector2(Random.Range(0, fieldSize.x), Random.Range(0, fieldSize.y));
                    Vector3 asteroidPos = gridPos + randInField;
                    Transform asteroid = Instantiate(asteroidPref, asteroidPos, Quaternion.Euler(asteroidPos.x, asteroidPos.y, 0)) as Transform;
                    Vector2 randScale = new Vector2(Random.Range(minSizeRelativeToField, maxSizeRelativeToField), Random.Range(minSizeRelativeToField, maxSizeRelativeToField));
                    asteroid.localScale = new Vector3(randScale.x, randScale.y, 1);
                }
            }
            
        }
    }
}
