using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class Haptic
{
    [Range(0,1)]
    public float intensity;
    public float duration;
    public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
    {
        if(eventArgs.interactableObject is XRBaseControllerInteractor controllerInteractor)
            TriggerHaptic(controllerInteractor.xrController);
    }
    
    public void TriggerHaptic(XRBaseController controller)
    {
        if (intensity > 0)
        {
            Debug.Log($"Sending haptic impulse: Intensity={intensity}, Duration={duration}");
            controller.SendHapticImpulse(intensity, duration);
        }
            
    }
}
