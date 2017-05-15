using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Allows functions stacking to make good use of single fixed update
/// </summary>
public class UpdateQueue : MonoBehaviour {

    /// <summary>
    /// functions to be run in this update in order
    /// 0: movement
    /// 1: rotation
    /// 2: copied movement on units
    /// </summary>
    public Action[] updateList = new Action[3];

	void Update () {
        foreach (var fncUpdate in updateList) {
            if (fncUpdate != null) {
                fncUpdate();
            }
        }
	}
}
