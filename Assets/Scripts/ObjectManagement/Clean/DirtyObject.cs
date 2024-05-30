using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyObject : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("If object can get dirty")]
    public bool isDirty = true;  // This object's current dirty state
    [SerializeField] 
    [Tooltip("Contact time required for objects to get dirty")]
    public float timeToMakeDirty = 3.0f;  // Time in seconds another object must be in contact to become dirty

}
