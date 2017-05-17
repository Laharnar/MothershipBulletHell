using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UnitInfo : MonoBehaviour {

    /// <summary>
    /// Additional objects it should save components from.
    /// For saving sprite renderer on subobjects, etc.
    /// </summary>
    public Transform[] additionalObjects;

    // dont put duplicates, search only returns first item
    // You could sort them based on how much they are used
    MonoBehaviour[] infos;
    Component[] components;

    // save last for faster access
    MonoBehaviour last;
    Component lastCompo;

    protected void Awake() {
        List<MonoBehaviour> infos = new List<MonoBehaviour>();
        List<Component> components = new List<Component>();
        infos.AddRange(GetComponents<MonoBehaviour>());
        components.AddRange(GetComponents<Component>());

        for (int i = 0; i < additionalObjects.Length; i++) {
            infos.AddRange(additionalObjects[i].GetComponents<MonoBehaviour>());
            components.AddRange(additionalObjects[i].GetComponents<Component>());
        }
        this.infos = infos.ToArray();
        this.components = components.ToArray();
    }

    public T Get<T>(params Type[] secondaryTypes) where T : MonoBehaviour{
        if (infos == null) Debug.Log("No behaviours.");
        for (int i = 0; i < infos.Length; i++) {
            if (infos[i] == null) Debug.Log("Remove null mono behaviour.", this);

            /*List<Type> results = new List<Type>();
            GetAllDerivedTypesRecursively(new Type[1] { typeof(T) }, infos[i].GetType(), ref results);
            for (int j = 0; j < results.Count; j++) {
                if (results[j] == typeof(T)) {
                    Debug.Log(infos[j].GetType() + " "+ (typeof(T).ToString()) + " " + results[j], infos[i].transform);
                    last = infos[j];
                    return (T)infos[j];
                }
            } //*/

            if (infos[i].GetType() == typeof(T)) {
                last = infos[i];
                return (T)infos[i];
            } else {
                for (int j = 0; j < secondaryTypes.Length; j++) {
                    if (infos[i].GetType() == secondaryTypes[j]) {
                        last = infos[i];
                        return (T)infos[i];
                    }
                }
            }
        }
        return null;
    }

 

    public T[] GetAll<T>() where T : MonoBehaviour {
        List<T> results = new List<T>();
        for (int i = 0; i < infos.Length; i++) {
            if (infos[i] == null) Debug.Log("Remove null mono behaviour.", this);
            if (infos[i].GetType() == typeof(T)) {
                last = infos[i];
                results.Add((T)infos[i]);
            }
        }
        return results.ToArray();
    }

    internal T GetCompt<T>() where T: Component {
        for (int i = 0; i < components.Length; i++) {
            if (components[i] == null) Debug.Log("Remove null component behaviour.", this);
            if (components[i].GetType() == typeof(T)) {
                lastCompo = components[i];
                return (T)components[i];
            }
        }
        return null;
    }

    public T[] GetAllCompts<T>() where T : Component {
        List<T> results = new List<T>();
        for (int i = 0; i < components.Length; i++) {
            if (components[i] == null) Debug.Log("Remove null component behaviour.", this);
            if (components[i].GetType() == typeof(T)) {
                lastCompo = components[i];
                results.Add((T)components[i]);
            }
        }
        return results.ToArray();
    }

    public T GetLast<T>() where T : MonoBehaviour{
        return (T)last;
    }

    public T GetLastCompt<T>() where T: Component {
        return (T)lastCompo;
    }

    private static void GetAllDerivedTypesRecursively(Type[] types, Type type1, ref List<Type> results) {
        if (type1.IsGenericType) {
            GetDerivedFromGeneric(types, type1, ref results);
        } else {
            GetDerivedFromNonGeneric(types, type1, ref results);
        }
    }

    private static void GetDerivedFromGeneric(Type[] types, Type type, ref List<Type> results) {
        var derivedTypes = types
            .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == type).ToList();
        results.AddRange(derivedTypes);
        foreach (Type derivedType in derivedTypes) {
            GetAllDerivedTypesRecursively(types, derivedType, ref results);
        }
    }


    public static void GetDerivedFromNonGeneric(Type[] types, Type type, ref List<Type> results) {
        var derivedTypes = types.Where(t => t != type && type.IsAssignableFrom(t)).ToList();

        results.AddRange(derivedTypes);
        foreach (Type derivedType in derivedTypes) {
            GetAllDerivedTypesRecursively(types, derivedType, ref results);
        }
    }
}
