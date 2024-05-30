using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningAgent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CleanableObject cleanableObject = other.GetComponent<CleanableObject>();
        if (cleanableObject != null && !cleanableObject.isClean)
        {
            cleanableObject.Clean();
        }
    }
}
