using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorOpenController : MonoBehaviour
{
    [SerializeField] private Animator targetDoor = null;


    [SerializeField] private string openAnimation;
    [SerializeField] private string closeAnimation;
    
    private XRGrabInteractable grabInteractable;
     private bool _isOpen = false;

     public void OnPlayerInteract()
     {
         Debug.Log("Called OnPlayerInteract");
         if (!_isOpen)
         {
             targetDoor.Play(openAnimation,0,0.0f);
             _isOpen = true;
         }
         else
         {
             targetDoor.Play(closeAnimation,0,0.0f);
             _isOpen = false;
         }
         
     }

     public bool DoorState()
     {
         return _isOpen;
     }

}
