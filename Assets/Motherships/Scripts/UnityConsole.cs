
#define USEUCONSOLE
#define USEDEBUGTARGET

using UnityEngine;
using System.Collections.Generic;

public static class UnityConsole {
    public static bool useLogs
#if USEUCONSOLE
    = true;
#else
 = false;
#endif

    public static bool useDebugTarget
#if USEDEBUGTARGET
    = true;
#else
 = false;
#endif

    static List<UConsoleMsg> messages = new List<UConsoleMsg>();

    internal static void WriteWarning(object msg) {
        UnityConsole.WriteWarning(msg, null);
    }

    internal static void WriteWarning(object msg, Object target) {
        if (useLogs) {
            if (useDebugTarget && target != null) {
                messages.Add(new UConsoleMsg(msg, target));
            } else {
                messages.Add(new UConsoleMsg(msg, null));
            }
        }
    }

    public class UConsoleMsg {
        public object msg;
        public Object target;

        public UConsoleMsg(object msg, Object target) {
            this.msg = msg;
            this.target = target;
        }
    }

    internal static void ReleaseBuffer() {
        System.Text.StringBuilder compose = new System.Text.StringBuilder();

        foreach (UConsoleMsg log in messages) {
            compose.Append(log.msg);
            if (log.target != null) {
                compose.Append(" " + log.target.name);
            }
            compose.Append("\n**************************\n");
        }

        Debug.LogWarning(compose);

        messages.Clear();
    }
}