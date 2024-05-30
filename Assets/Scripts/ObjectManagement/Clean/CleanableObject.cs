using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanableObject : MonoBehaviour
{
    public bool isClean = true;
    public bool isCleanable = true;
    private float dirtTimer = 0;
    private bool inContactWithDirty = false;
    private DirtyObject _storedDirtyObject;

    void Update()
    {
        
        if (inContactWithDirty && isCleanable)
        {
            dirtTimer += Time.deltaTime;
            if (_storedDirtyObject != null && dirtTimer >= _storedDirtyObject.timeToMakeDirty)
                BecomeDirty();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (isCleanable)
        {
            DirtyObject dirtyObject = collision.gameObject.GetComponent<DirtyObject>();
            if (dirtyObject != null && dirtyObject.isDirty)
            {
                inContactWithDirty = true;
                _storedDirtyObject = dirtyObject;  // Store the reference to the dirty object
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<DirtyObject>())
        {
            inContactWithDirty = false;
            dirtTimer = 0;
            _storedDirtyObject = null;  // Clear the reference
        }
    }
    public void Clean()
    {
        if (isCleanable && !isClean)
        {
            isClean = true;
            isCleanable = false;  
            Debug.Log($"{gameObject.name} has been cleaned and can no longer become dirty.");
        }
    }
    private void BecomeDirty()
    {
        if (isCleanable)
        {
            isClean = false;
            inContactWithDirty = false;
            dirtTimer = 0;
            Debug.Log($"{gameObject.name} has become dirty.");
        }
    }
}
