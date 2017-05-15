//#define USEUCONSOLE
//#define USEDEBUGTARGET

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ComponentHelper {

    /// <summary>
    /// Checks if variable is set in inspector(is not null).
    /// If not, it prints a warning and source's name.
    /// 
    /// Use for checking if components are set.
    /// </summary>
    public static void InspectorNullComponentWarning(this MonoBehaviour obj, MonoBehaviour target, string warning) {
        if (target == null) {
            UnityConsole.WriteWarning("IsMonoComponentWarning:" + warning + " Source:" + obj.name);
        }
    }

    
}

