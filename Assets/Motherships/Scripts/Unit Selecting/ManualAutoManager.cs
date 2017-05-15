using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// interface for switching between manual and auto aim.
/// </summary>
public class ManualAutoManager : MonoBehaviour {

    public Button buttonAutoOn; // these 2 buttons should call functions AutoOn and ManualOn
    public Button buttonManualOn;

    UnitSelector selection;

    bool areButtonsVisible = false;

    void Start() {
        selection = GetComponent<UnitSelector>();

        if (!areButtonsVisible) {
            buttonAutoOn.gameObject.SetActive(false);
            buttonManualOn.gameObject.SetActive(false);
        } else {
            buttonAutoOn.gameObject.SetActive(true);
            buttonManualOn.gameObject.SetActive(true);
        }
    }

    void Update() {
        // THIS HAPPENS ONCE PER SELECTION CHANGE, NOT EVERY FRAME
        // Gets guns and shows manual and auto buttons
        if (selection.areUnitsSelected && selection.selectionChanged) {
            Gun[] guns = selection.GetGuns;

            bool buttonActiveAuto = false,
                buttonActiveManual = false;
            for (int i = 0; i < guns.Length; i++) {
                // show manual aim button if gun is in non manual mode
                if (!buttonActiveManual && (
                    guns[i].AimState == Gun.AimingState.autoAim ||
                    guns[i].AimState == Gun.AimingState.targetAim ||
                    guns[i].AimState == Gun.AimingState.directionAim)) {
                    buttonActiveManual = true;
                    buttonManualOn.gameObject.SetActive(true);
                }
                // show auto aim if gun is in manual mode
                if (!buttonActiveAuto && guns[i].AimState == Gun.AimingState.manualAim) {
                    buttonAutoOn.gameObject.SetActive(true);
                    buttonActiveAuto = true;
                }
                
            }

            LaserGun[] lasers = selection.GetLasers; // add ifs for lasers too
            for (int i = 0; i < lasers.Length; i++) {
                // show manual aim button if gun is in non manual mode
                if (!buttonActiveManual && (
                    lasers[i].AimState == Gun.AimingState.autoAim ||
                    lasers[i].AimState == Gun.AimingState.targetAim ||
                    lasers[i].AimState == Gun.AimingState.directionAim)) {
                    buttonActiveManual = true;
                    buttonManualOn.gameObject.SetActive(true);
                }
                // show auto aim if gun is in manual mode
                if (!buttonActiveAuto && lasers[i].AimState == Gun.AimingState.manualAim) {
                    buttonAutoOn.gameObject.SetActive(true);
                    buttonActiveAuto = true;
                }

            }
            areButtonsVisible = buttonActiveAuto || buttonActiveManual;
        }
        // if all units were just deselected, 
        if (areButtonsVisible && !selection.areUnitsSelected && selection.selectionChanged) {
            buttonAutoOn.gameObject.SetActive(false);
            buttonManualOn.gameObject.SetActive(false);
            areButtonsVisible = false;
        }
    }

    #region Aim interface
    /// <summary>
    /// Call this function from first button, or ai
    /// </summary>
    public void AutoOn() {
        ChoseOption(true, false);
    }

    /// <summary>
    /// Call this function from other button, or ai
    /// </summary>
    public void ManualOn() {
        ChoseOption(false, true);
    }
    #endregion


    #region Helper methods
    void ChoseOption(bool auto, bool manual) {
        if (selection.areUnitsSelected) {
            Gun[] guns = selection.GetGuns;
            for (int i = 0; i < guns.Length; i++) {
                if (manual)
                    guns[i].StartManualAim();
                else if (auto)
                    guns[i].StartAutoAim();                    
            }

            LaserGun[] lasers = selection.GetLasers;
            for (int i = 0; i < lasers.Length; i++) {
                if (manual)
                    lasers[i].StartManualAim();
                else if (auto)
                    lasers[i].StartAutoAim();
            }
        }
    } 
    #endregion
}
