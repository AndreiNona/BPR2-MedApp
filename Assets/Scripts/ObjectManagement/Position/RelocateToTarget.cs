using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RelocateToTarget : MonoBehaviour
{
    [SerializeField] private Transform location;
   

    private void OnCollisionEnter(Collision collision)
    {
        Relocate(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Relocate(other.gameObject);
    }
    private void Relocate(GameObject target)
    {
        
        if (target.CompareTag("Relocatable"))
        {
            Debug.Log("Found plate: " + target.gameObject.name);

                    Debug.Log("Relocating:" + target.gameObject.name);
                    target.transform.position = location.transform.position;
                    target.transform.rotation = location.rotation;
                   //target.transform.rotation =quaternion.Euler(-20,0,0);
                    //Prevent error when relocating composite objects
                    try
                    {
                        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    }
                    catch (MissingComponentException e)
                    {
                        Console.WriteLine(e);
                    }

        }else
            Debug.Log("Invalid object for relocating:" + target.gameObject.name);

    }
}
