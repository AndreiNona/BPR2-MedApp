using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    private struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    private TransformData _originalTransform;
    private Dictionary<Transform, TransformData> _originalChildTransforms;
    private Rigidbody _rigidbody;
    private List<Rigidbody> _childRigidbodies;

    void Start()
    {
        // Store the original transform 
        _originalTransform.Position = transform.position;
        _originalTransform.Rotation = transform.rotation;
        
        // Initialize the dictionary to store original transforms of children
        _originalChildTransforms = new Dictionary<Transform, TransformData>();
        _childRigidbodies = new List<Rigidbody>();

        // Get all child objects and store their original transform
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child != transform) // Skip the parent object
            {
                _originalChildTransforms[child] = new TransformData
                {
                    Position = child.position,
                    Rotation = child.rotation
                };
                var childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    _childRigidbodies.Add(childRigidbody);
                }
            }
        }

        // Get the Rigidbody component of the main object
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            Debug.Log("Collided with room");
            ToOriginal();
        }
    }

    public void ToOriginal()
    {
        Debug.Log("Resetting to original position");

        // Reset the main 
        transform.position = _originalTransform.Position;
        transform.rotation = _originalTransform.Rotation;

        // Reset the main object Rigidbody
        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("Rigidbody component is missing. Cannot reset momentum and rotational speed.");
        }

        // Reset each child
        foreach (var kvp in _originalChildTransforms)
        {
            kvp.Key.position = kvp.Value.Position;
            kvp.Key.rotation = kvp.Value.Rotation;
        }

        // Reset each child object Rigidbody
        foreach (var childRigidbody in _childRigidbodies)
        {
            childRigidbody.velocity = Vector3.zero;
            childRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
