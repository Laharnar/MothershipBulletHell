using UnityEngine;
using System.Collections;

public class UpgradePlatform : MonoBehaviour {

    public UpgradeText connection;

    void Start() {
        if (connection)
            connection.open = false;
    }

    internal void ShowButton(bool visible) {
        if (!connection)
            return;
        connection.open = visible;
        connection.UpdateDisplay();
    }
}
