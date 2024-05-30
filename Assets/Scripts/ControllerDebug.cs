using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerDebug : MonoBehaviour
{
    public ActionBasedController leftController;
    public InputActionAsset inputActionAsset;
    void Start()
    {
        inputActionAsset.Enable();
    }
    void Update()
    {
        if (leftController.positionAction.action.ReadValue<Vector3>() != Vector3.zero)
        {
            Debug.Log("Left Controller Position: " + leftController.positionAction.action.ReadValue<Vector3>());
        }

        if (leftController.rotationAction.action.ReadValue<Quaternion>() != Quaternion.identity)
        {
            Debug.Log("Left Controller Rotation: " + leftController.rotationAction.action.ReadValue<Quaternion>());
        }

        Debug.Log("Left Controller Is Tracked: " + leftController.isTrackedAction.action.ReadValue<bool>());
    }
}
