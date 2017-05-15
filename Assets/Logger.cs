using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Collective logger
/// </summary>
public class Logger : MonoBehaviour {

    public static Logger logs;

    static string log;

    public static List<MonoBehaviour> targets = new List<MonoBehaviour>();

    bool logDone = false;

    public static void AddLog(string txt, MonoBehaviour target) {
        targets.Add(target);
        log += "["+ target.name + "]["+target.GetType().Name+"]"+txt+"\n";
    }

    public void Awake() {
        logs = this;
    }

	// Update is called once per frame
	void Update () {
        if (logDone) return;
        if (Time.time > 5) {
            logDone = true;
            Debug.Log(log);
            log = "";
            targets.Clear();
        }
	}
}
