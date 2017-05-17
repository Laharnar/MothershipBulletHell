using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class TurretSetup : MonoBehaviour {

    public MonoBehaviour getType;
    public List<string> types;
    public List<ScriptSetup> scriptSetups;

    const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public |
            BindingFlags.Instance | BindingFlags.Static;

    public bool reload;
    public bool saveValues = false;
    public bool locked=false;

	// Use this for initialization
	void Start ()
    {
        //AddMissingComponents(scriptSetups);
        reload = false;

        //DebugProperties(scriptSetups[0]);
    }

   /* private static void DebugProperties(ScriptSetup obj)
    {
        FieldInfo[] fields = obj.GetType().GetFields(flags);
        foreach (FieldInfo fieldInfo in fields)
        {
            Debug.Log("Obj: " + obj + ", Field: " + fieldInfo.Name);
        }
        PropertyInfo[] properties = obj.GetType().GetProperties(flags);
        foreach (PropertyInfo propertyInfo in properties)
        {
            Debug.Log("Obj: " + obj + ", Property: " + propertyInfo.Name);
        }
    }
    /*
    private void AddMissingComponents(ScriptSetup[] addScripts)
    {
        if (addScripts != null)
        for (int i = 0; i < addScripts.Length; i++)
        {
            if (addScripts[i].script && !transform.GetComponent(addScripts[i].script.GetType()))
            {
                transform.gameObject.AddComponent(addScripts[i].script.GetType());
            }
        }
    }*/

    // Update is called once per frame
    void Update () {
        if (locked)
        {
            return;
        }

        if (getType != null && !types.Contains(getType.GetType().Name))
        {
            types.Add(getType.GetType().Name);

            TrimSize();
            LoadPropertiesForSetups();
        }
        if (reload)
        {
            //DebugProperties();
            //AddMissingComponents(scriptSetups);
            LoadPropertiesForSetups();
            reload = false;
        }
	}

    private void LoadPropertiesForSetups()
    {
        for (int i = 0; i < scriptSetups.Count; i++)
        {
            ScriptSetup scsetup = scriptSetups[i];
            GetFieldsAndProperties(scsetup, GetComponent(types[i]));

        }
    }

    private void GetFieldsAndProperties(ScriptSetup scsetup, Component target)
    {
        Component obj = target;
        FieldInfo[] fields = obj.GetType().GetFields(flags);
        scsetup.fields = new ScriptValue[fields.Length];
        for (int j = 0; j < scsetup.fields.Length; j++)
        {
            scsetup.fields[j] = new ScriptValue(fields[j].Name);
            if (saveValues)
            {
                scsetup.fields[j].SetValue(fields[j].GetValue(obj));
            }
        }
        return;
        PropertyInfo[] properties = obj.GetType().GetProperties(flags);
        scsetup.properties = new ScriptValue[properties.Length];
        for (int j = 0; j < scsetup.properties.Length; j++)
        {
            scsetup.properties[j] = new ScriptValue(properties[j].Name);
            if (saveValues)
            {
                scsetup.properties[j].SetValue(properties[j].GetValue(obj, null));
            }
        }
    }

    private void TrimSize()
    {
        while (types.Count > scriptSetups.Count)
        {
            scriptSetups.Add(new ScriptSetup());
        }
        while (types.Count < scriptSetups.Count)
        {
            scriptSetups.RemoveAt(scriptSetups.Count - 1);
        }
    }
}

[System.Serializable]
public class ScriptSetup
{
    
    public ScriptValue[] fields;
    public ScriptValue[] properties;
}

[System.Serializable]
public class ScriptValue
{
    public string name;

    public bool copyValue = true;
    public string strValue;
    public MonoBehaviour compValue;

    public ScriptValue(string name)
    {
        this.name = name;
    }

    internal void SetValue(object v)
    {
        Type t = v.GetType();
        if (t == typeof(string))
        {
            strValue = (string)v;
        }
        if (t == typeof(MonoBehaviour))
        {
            compValue = (MonoBehaviour)v;
        }
        else
        {
            compValue = (MonoBehaviour)v;

        }

        if (!copyValue)
        {
            strValue = "";
            compValue = null;
        }
    }
}