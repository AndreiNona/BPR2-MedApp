using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticArea : MonoBehaviour
{
    public Haptic hapticOnEnter;
    public float bpm; // Beats per minute
    private XRBaseController controllerInArea;
    private float hapticInterval;
    private float lastHapticTime;
    private HospitalPatientController patientController;

    void Start()
    {
        // Find the HospitalPatientController in the parent hierarchy
        patientController = GetComponentInParent<HospitalPatientController>();

        if (patientController == null)
        {
            Debug.LogError("HospitalPatientController not found in the parent hierarchy.");
        }
    }

    void Update()
    {
        // Calculate the interval between haptic pulses based on BPM
        if (controllerInArea != null && bpm > 0)
        {
            hapticInterval = 60f / bpm;

            if (Time.time - lastHapticTime >= hapticInterval)
            {
                Debug.Log("Triggering haptic feedback.");
                hapticOnEnter.TriggerHaptic(controllerInArea);
                lastHapticTime = Time.time;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name);

        // Try to get the XRBaseController from various components in the hierarchy
        XRDirectInteractor directInteractor = other.GetComponentInParent<XRDirectInteractor>();
        XRRayInteractor rayInteractor = other.GetComponentInParent<XRRayInteractor>();

        if (directInteractor != null)
        {
            controllerInArea = directInteractor.xrController;
            Debug.Log("Controller entered (direct interactor): " + controllerInArea.name);
        }
        else if (rayInteractor != null)
        {
            controllerInArea = rayInteractor.xrController;
            Debug.Log("Controller entered (ray interactor): " + controllerInArea.name);
        }
        else
        {
            Debug.LogWarning("No valid XR interactor found on: " + other.gameObject.name);
        }

        if (controllerInArea != null && patientController != null)
        {
            bpm = (float)patientController.GetCurrentHeartRate();
            Debug.Log($"Updated BPM to: {bpm}");
            lastHapticTime = Time.time;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit called with: " + other.gameObject.name);

        XRDirectInteractor directInteractor = other.GetComponentInParent<XRDirectInteractor>();
        XRRayInteractor rayInteractor = other.GetComponentInParent<XRRayInteractor>();

        if ((directInteractor != null && directInteractor.xrController == controllerInArea) ||
            (rayInteractor != null && rayInteractor.xrController == controllerInArea))
        {
            Debug.Log("Controller exited: " + controllerInArea.name);
            controllerInArea = null;
        }
    }
}
