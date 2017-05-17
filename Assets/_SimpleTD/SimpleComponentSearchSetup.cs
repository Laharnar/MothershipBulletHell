using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using UnityEditor;
using System.Collections.Generic;

// use it to set missing refernces to script in the objects.
// set type manualy
public class SimpleComponentSearchSetup : EditorWindow {

    private GameObject limitedSearch;
    public MonoBehaviour basescript;
    public string varname;
    public Component setToType;
    public bool canSearchFromParent = true;

    public bool finished = false;

    int updateNum = 0;
    private bool manualSetupVisible;
    private bool automaticSetupVisible;

    List<List<bool[]>> toggleGroups = new List<List<bool[]>>();// go's>mono's>fields
    private Vector2 autoGroupsScroll;

    [MenuItem("Utility/Component nullMono setup")]
	static void Init() {
        SimpleComponentSearchSetup target = (SimpleComponentSearchSetup)GetWindow<SimpleComponentSearchSetup>();
        target.toggleGroups = new List<List<bool[]>>();
        target.Show();
    }

    // Update is called once per frame
    void OnGUI ()
    {

        // - MANUAL -
        manualSetupVisible = EditorGUILayout.BeginToggleGroup("manual setup", manualSetupVisible);
        limitedSearch = (GameObject)EditorGUILayout.ObjectField("tree start(null:unlimited)", limitedSearch, typeof(GameObject));

        basescript = (MonoBehaviour)EditorGUILayout.ObjectField("base script", basescript, typeof(MonoBehaviour));
        varname = EditorGUILayout.TextField("field name", varname);
        setToType = (Component)EditorGUILayout.ObjectField("set to type", setToType, typeof(Component));
        canSearchFromParent = EditorGUILayout.Toggle("search from parent", canSearchFromParent);

        if (GUILayout.Button("reload"))
        {
            SetValues();
        }
        EditorGUILayout.LabelField("[" + (updateNum + 1) + ". reload]");

        // - AUTOMATIC -
        // finds all null refrences in scene and asks you which you want to fix
        EditorGUILayout.EndToggleGroup();

        autoGroupsScroll = EditorGUILayout.BeginScrollView(autoGroupsScroll);
        automaticSetupVisible = EditorGUILayout.BeginToggleGroup("automatic setup", automaticSetupVisible);
        
        List<FieldData> fields = GetAndDrawNullFields();
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Update"))
        {
            for (int i = 0; i < fields.Count; i++)
            {
                SetValues(fields[i].targetGo, fields[i].script, fields[i].field.Name, fields[i].field.FieldType);
            }
        }

    }

    private List<FieldData> GetAndDrawNullFields()
    {
        List<FieldData> finalFields = new List<FieldData>();
        // get objects
        GameObject[] allGos = GameObject.FindObjectsOfType<GameObject>();
     
        for (int i = 0; i < allGos.Length; i++)
        {
            GameObject go = allGos[i];
            // get scrits on object
            MonoBehaviour[] scripts = go.GetComponents<MonoBehaviour>();
            if (toggleGroups.Count < allGos.Length)
                toggleGroups.Add(new List<bool[]>());
            for (int ii = 0; ii < scripts.Length; ii++)
            {
                MonoBehaviour script = scripts[ii];
                // get fields on script
                FieldInfo[] fields = script.GetType().GetFields();
                if (toggleGroups[i].Count < scripts.Length)
                    toggleGroups[i].Add(new bool[fields.Length]);

                for (int iii = 0; iii < fields.Length; iii++)
                {
                    FieldInfo field = fields[iii];
                    if (field.GetValue(script) == null)
                    {
                        toggleGroups[i][ii][iii] = EditorGUILayout.BeginToggleGroup("null", toggleGroups[i][ii][iii]);

                        EditorGUILayout.ObjectField("tree start(null:unlimited)", go, typeof(GameObject));

                        EditorGUILayout.ObjectField("base script", script, typeof(MonoBehaviour));
                        //field.SetValue(script, EditorGUILayout.TextField("field name", field.GetValue(script)));
                        EditorGUILayout.TextField("field name", field.Name);
                        //EditorGUILayout.ObjectField("value", (UnityEngine.Object)field.GetValue(script), field.GetType());
                        EditorGUILayout.LabelField("set to type"+ field.FieldType.ToString());
                        EditorGUILayout.LabelField("search from parent = true");

                        finalFields.Add(new FieldData(go, script, field, toggleGroups[i][ii][iii]));
                        EditorGUILayout.EndToggleGroup();
                    }
                }
            }
        }
        return finalFields;
    }

    struct FieldData
    {
        public GameObject targetGo;
        public MonoBehaviour script;
        public FieldInfo field;

        public bool useField;

        public FieldData(GameObject go, MonoBehaviour script, FieldInfo field, bool useField) : this()
        {
            this.targetGo = go;
            this.script = script;
            this.field = field;
            this.useField = useField;
        }
    }

    void SetValues()
    {
        finished = false;
        SetValues(limitedSearch, basescript, varname, setToType.GetType());
        updateNum++;
        finished = true;
    }

    private void SetValues(GameObject limitedSearch, MonoBehaviour basescript, string varname, Type setToType)
    {
        UnityEngine.Object[] gos; 
        if (limitedSearch)
        {
            gos = limitedSearch.GetComponentsInChildren(basescript.GetType());
        }
        else
        {
            gos = GameObject.FindObjectsOfType(basescript.GetType());
        }

        for (int i = 0; i < gos.Length; i++)
        {
            FieldInfo nullvar = gos[i].GetType().GetField(varname);
            Component result = null;
            // FIX 1: script assumes some values are non null and exist
            if (setToType == null)
            {
                Debug.Log("Null setting "+setToType);
                continue;
            }
            if (((Component)gos[i]).transform == null)
            {
                Debug.Log("Null script "+gos[i]);
                continue;
            }

            if (canSearchFromParent)
            {
                if (((Component)gos[i]).transform.parent)
                    result = ((Component)gos[i]).transform.parent.GetComponentInChildren(setToType);
            }
            else
                result = ((Component)gos[i]).transform.GetComponentInChildren(setToType);
            nullvar.SetValue(gos[i], result);

        }
    }
}
