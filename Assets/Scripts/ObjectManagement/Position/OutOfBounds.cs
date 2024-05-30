using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class uses "other.gameObject" game possible improvement: Look into behaviour when not calling ".gameObject"
public class OutOfBounds : MonoBehaviour
{
    
    
    [SerializeField] private GameObject TeleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.CompareTag("Required"))
        {
            Debug.Log("Out of bounds, relocating: " + other.gameObject.name);
            
            try
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            catch (MissingComponentException e)
            {
                Console.WriteLine(e);
            }
            other.gameObject.transform.position = TeleportTarget.transform.position;
        }
        else
        {
            Debug.Log("Out of bounds, destroying: " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
}
