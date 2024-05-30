using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticInteractables : MonoBehaviour
{
public static HapticInteractables instance { get; private set; }
    public Haptic hapticOnActivated;
    public XRBaseController leftController;
    public XRBaseController rightController;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Optional: if you want this instance to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    void Start()
    {
        /*XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        interactable.activated.AddListener(hapticOnActivated.TriggerHaptic);*/
    }
    public void TriggerLeftHaptics()
    {
        if (leftController != null)
        {
            hapticOnActivated.TriggerHaptic(leftController);
        }
        else
        {
            Debug.LogWarning("Left controller is not set.");
        }
    }

    // Method to trigger haptic feedback on the right controller
    public void TriggerRightHaptics()
    {
        if (rightController != null)
        {
            hapticOnActivated.TriggerHaptic(rightController);
        }
        else
        {
            Debug.LogWarning("Right controller is not set.");
        }
    }

    // Method to trigger haptic feedback on both controllers
    public void TriggerBothHaptics()
    {
        if (leftController != null)
        {
            hapticOnActivated.TriggerHaptic(leftController);
        }
        else
        {
            Debug.LogWarning("Left controller is not set.");
        }

        if (rightController != null)
        {
            hapticOnActivated.TriggerHaptic(rightController);
        }
        else
        {
            Debug.LogWarning("Right controller is not set.");
        }
    }
}
