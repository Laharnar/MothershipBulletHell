using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GroupControl : MonoBehaviour {

    protected List<GroupUnitControl> members;
    public bool toggleOnRegister = false;
    public bool disableOnRegister = false;

    internal void Register(GroupUnitControl groupHpControl) {
        if (members == null) {
            members = new List<GroupUnitControl>();
        }
        members.Add(groupHpControl);
        if (toggleOnRegister)
            ToggleMember(members.Count - 1);
        if (disableOnRegister)
            EnableMember(members.Count - 1, false);
    }

    public void UnRegister(int i) {
        members.RemoveAt(i);
    }

    internal void ToggleAll() {
        for (int i = 0; i < members.Count; i++) {
            ToggleMember(i);
        }
    }

    internal void SetEnabledAll(bool enabled) {
        for (int i = 0; i < members.Count; i++) {
            EnableMember(i, enabled);
        }
    }

    internal void EnableMember(int i, bool enabled) {
        for (int j = 0; j < members[i].managed.Length; j++)
            members[i].managed[j].enabled = enabled;
    }

    internal void ToggleMember(int i) {
        for (int j = 0; j < members[i].managed.Length; j++)
            members[i].managed[j].enabled = !members[i].managed[j].enabled;
    }

    /// <summary>
    /// respond to some member getting zero hp.
    /// </summary>
    public virtual void OnMemberDestroyed(GroupUnitControl destroyedMember) { }
    //protected virtual void OnMemberSpawned(GroupUnitControl spawnedMember) { }

}
