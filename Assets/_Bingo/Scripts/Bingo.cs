using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Bingo : MonoBehaviour {

    int player = 0;

    Color[] colors = new Color[2] { Color.red, Color.blue };

    Transform[,] grid;

    public Transform prefab;

    public Text winText;

	// Use this for initialization
	void Start () {
        grid = new Transform[10, 8];
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 8; j++) {
                grid[i, j] = Instantiate(prefab, transform) as Transform;
                grid[i, j].position = new Vector3(i*1.01f, j*1.01f);
                grid[i, j].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        StartCoroutine(Game());
	}
	

	// Update is called once per frame
	IEnumerator Game () {
        while (true) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                DropIn(GetColumn(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                int count = CheckEndGame(grid, 4);
                if (count >= 4) {
                    break;
                }
                player =  ++player % 2;
            }
            yield return true;
        }
        WinScreen(player);
	}

    private void WinScreen(int player) {
        winText.text = "Winner is PLAYER " + (player+1) + "!";
    }

    private int CheckEndGame(Transform[,] grid, int nInRow) {
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 8; j++) {
                int count = 1;
                Color col = grid[i, j].GetComponent<SpriteRenderer>().color;
                if (col == Color.white) {
                    continue;
                }
                // vertical. go over colors up and count them
                for (int k = j+1; k < 8; k++) {
                    if (grid[i, k].GetComponent<SpriteRenderer>().color == col) {
                        count++;
                    } else {
                        break;
                    }
                }
                if (count >= nInRow) {
                    return count;
                }
                count = 0;
                // horizontal
                for (int k = i + 1; k < 10; k++) {
                    if (grid[k, j].GetComponent<SpriteRenderer>().color == col) {
                        count++;
                    } else {
                        break;
                    }
                }
                if (count >= nInRow) {
                    return count;
                }
                count = 0;
                //diagonal /
                for (int k = 0; i+k < 10 && j+k < 8; k++) {
                    if (grid[i+k, j+k].GetComponent<SpriteRenderer>().color == col) {
                        count++;
                    } else {
                        break;
                    }
                }
                if (count >= nInRow) {
                    return count;
                }
                count = 0;
                //diagonal \
                for (int k = 0; j+k <8 && i-k >= 0; k++) {
                    if (grid[i - k, j + k].GetComponent<SpriteRenderer>().color == col) {
                        count++;
                    } else {
                        break;
                    }
                }
                if (count >= nInRow) {
                    return count;
                }
            }
        }
        return 0;
    }

    private int GetColumn(Vector3 mousePosition) {
        int column = -1;
        float min = 100000;
        for (int i = 0; i < 10; i++) {
            float diff = Mathf.Abs(grid[i, 0].position.x - mousePosition.x);
            Debug.Log(diff);
            if (diff < min) {
                min = diff;
                column = i;
            }
        }
        return column;
    }

    public void DropIn(int column) {
        for (int i = 0; i < 8; i++) {
            if (grid[column, i].GetComponent<SpriteRenderer>().color == Color.white) {
                grid[column, Mathf.Clamp(i, 0, 8 - 1)].GetComponent<SpriteRenderer>().color = colors[player];
                break;
            }
        }
    }
}
