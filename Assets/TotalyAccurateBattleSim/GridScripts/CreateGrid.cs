using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CreateGrid : MonoBehaviour {


    public int width = 1;
    public int height = 1;
    public float pieceRadius= 1;
    public Transform gridItemPref;
    public Vector3 offset;
    Transform[,] grid;

    public bool updated = false;
    public bool dontDestroyGridOnSceneLoad = true;

    GameObject go;


    // Update is called once per frame
    void Update () {
        if (updated)
        {
            updated = false;
            MakeGrid(width, height);

        }
    }

    private void MakeGrid(int width, int height)
    {
        if (go)
        {
            DestroyImmediate(go);
        }

        go = new GameObject();
        go.name = "grid";

        grid = new Transform[height, width];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Transform item = Instantiate(gridItemPref) as Transform;
                grid[j, i] = item;
                grid[j, i].parent = go.transform;

                float stepx = 2 * pieceRadius * i + pieceRadius - pieceRadius * width + offset.x;// with offset by half grid
                float stepy = 2 * pieceRadius * j + pieceRadius - pieceRadius * height + offset.y;// with offset by half grid

                item.position = new Vector3(stepx, stepy, 0);

                IncludeAlliance(item);
            }
        }
        if (dontDestroyGridOnSceneLoad)
        {
            go.AddComponent<KeepBetweenScenes>();
        }
    }

    private static void IncludeAlliance(Transform item)
    {
        item.gameObject.AddComponent<Alliance>();
    }
}
