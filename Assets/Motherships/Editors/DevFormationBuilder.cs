using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

public class RefreshableWindow : EditorWindow {
    protected virtual void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh")) {
            RefreshableWindow window = (RefreshableWindow)EditorWindow.GetWindow(typeof(RefreshableWindow));
            window.Init();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected virtual void Init() {
        Show();
    }
}

public class DevFormationBuilder : RefreshableWindow {

    public int selectedBtn { get; set; }
    public Vector2 gridSize { get; set; }

    public FormationTemplate[] templateShips;
    int selectedShipType = 0;
    SerializedObject so;
    bool[,] shipPlaces;// formation placement
    GameObject[,] formation;
    
    private Vector2 leaderPos;

    private string formationName = "";

    //FormationTemplateFix testSo;

	[MenuItem ("Window/Formation builder")]
    static void ShowWin () {
        // Get existing open window or if none, make a new one:
        DevFormationBuilder window = (DevFormationBuilder)EditorWindow.GetWindow(typeof(DevFormationBuilder));

        
        window.Show();
        window.Init();
        
    }

    protected override void Init() {
        base.Init();
        
        //testSo = CreateInstance<FormationTemplateFix>();
        //testSo = FormationTemplateFix.CreateInstance<FormationTemplateFix>();

        //testSo = ResourceScriptableObject.SaveSO<FormationTemplateFix>(testSo, "testing");
        //Selection.activeObject = testSo;      

        

        //Debug.Log(ResourceScriptableObject.LoadSO<FormationTemplateFix>("testing"));
        //Debug.Log(((FormationTemplateFix)ResourceScriptableObject.LoadSO<FormationTemplateFix>("testing")).name);

    }

    protected override void OnGUI()
    {
        if (shipPlaces == null)
        {
            shipPlaces = new bool[(int)gridSize.x, (int)gridSize.y];
        }

        try
        {
            EditorGUILayout.BeginHorizontal();
            
            /// do this in later stage instead
            //ChoseShipType();

            // set grid size
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            Vector2 temp = gridSize;
            gridSize = EditorGUILayout.Vector2Field("Grid", gridSize);
            gridSize = new Vector2((int)gridSize.x, (int)gridSize.y);
            if ((temp.x != gridSize.x || temp.y != gridSize.y))
            {
                shipPlaces = new bool[(int)gridSize.x, (int)gridSize.y];
            }
            EditorGUILayout.EndHorizontal();
            // create grid with toggle values
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < gridSize.x; x++)
            {
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(15));
                for (int y = 0; y < gridSize.y; y++)
                {
                    shipPlaces[x, y] = GUILayout.Toggle(shipPlaces[x, y], "");
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            //selectedBtn = GUILayout.SelectionGrid(selectedBtn, new string[0], 5);
            EditorGUILayout.EndVertical();
            // GUILayout.BeginVertical();
            //GUILayout.EndVertical();

            //generate prefab from grid
            
            formationName = GUILayout.TextField(formationName);
            if (GUILayout.Button("Create", GUILayout.MaxWidth(150)))
            {
                GameObject formationParent = new GameObject();
                formation = new GameObject[(int)gridSize.y, (int)gridSize.y];
                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        if (shipPlaces[x, y])
                        {
                            GameObject partOfFormation = new GameObject();
                            partOfFormation.transform.parent = formationParent.transform;
                            partOfFormation.transform.position = new Vector3(x, y, 0);
                            partOfFormation.transform.name = "FormationUnit_"+x+"_"+y;
                            partOfFormation.AddComponent<FormationTemplate>().OnCreateEmpty(templateShips[selectedShipType]);

                            formation [x, y] = partOfFormation;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (gridSize.x > 0 && gridSize.y > 0) {
                EditorGUILayout.BeginHorizontal();
                leaderPos = EditorGUILayout.Vector2Field("Leader", leaderPos);
                leaderPos = new Vector2((int)leaderPos.x, (int)leaderPos.y);
                if (leaderPos.x < gridSize.x && leaderPos.y < gridSize.y
                    && GUILayout.Button("Apply leader", GUILayout.MaxWidth(150))) {
                    //formation[(int)leaderPos.x, (int)leaderPos.y].GetComponent<FormationUnit>().isLeader = true;
                        
                    int x = (int)leaderPos.x;
                    int y = (int)leaderPos.y;

                    for (int i = 0; i < formation.GetLength(0); i++) {
                        for (int j = 0; j < formation.GetLength(1); j++) {
                            if (i != x && j != y) {
                                formation[i, j].GetComponent<FormationUnit>().SetLeader(formation[(int)leaderPos.x, (int)leaderPos.y].GetComponent<FormationUnit>());
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            

        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        base.OnGUI();

        AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
	}

    private void ChoseShipType() {
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(50));
        if (so == null) {

            so = new SerializedObject(this);
        }
        if (so != null) {
            SerializedProperty templateShipsProperty = so.FindProperty("templateShips");
            EditorGUILayout.PropertyField(templateShipsProperty, true);

            so.ApplyModifiedProperties();
        }


        selectedShipType = EditorGUILayout.IntField(selectedShipType);
        EditorGUILayout.EndVertical();
    }
}
