using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RemoveObjects : MonoBehaviour
{
    [SerializeField] private TMP_Text prompt;
    [SerializeField] private bool isPrompt;
    [SerializeField] private GameObject teleportTarget;
    [SerializeField] private DropdownManager dropdownManager;
    [SerializeField] private List<string> feedbackPositiveList = new List<string> {
        "Great job!", "Keeping the planet safe", "Doing great work",
        "Keep on recycling", "That's the spirit", "Thank you!",
        "Thank you for your work!", "Solid work!"
    };

    [SerializeField] private List<string> tagsToRemove = new List<string> { "Destroyable","Tool" };
    [SerializeField] private List<string> tagsToRelocate = new List<string> { "Required" };

    private void OnTriggerEnter(Collider other)
    {
        GameObject triggerObject = other.gameObject;

        if (ShouldRemove(triggerObject))
        {
            RemoveObject(triggerObject);
        }
        else if (ShouldRelocate(triggerObject))
        {
            RelocateObject(triggerObject);
        }
    }

    private bool ShouldRemove(GameObject obj)
    {
        foreach (var tag in tagsToRemove)
        {
            if (obj.CompareTag(tag))
                return true;
        }
        return false;
    }

    private bool ShouldRelocate(GameObject obj)
    {
        foreach (var tag in tagsToRelocate)
        {
            if (obj.CompareTag(tag))
                return true;
        }
        return false;
    }

    private void RemoveObject(GameObject obj)
    {
        if(isPrompt)
            prompt.text = PickRandomString(feedbackPositiveList);

        // Get the root parent of the GameObject
        GameObject rootObject = obj.transform.root.gameObject;

        if (dropdownManager != null)
        {
 
            dropdownManager.DecreaseToolCount(rootObject);
        }

        // Destroy the root object 
        Destroy(rootObject);
        Debug.Log($"Destroyed: {rootObject.name}");
    }

    private void RelocateObject(GameObject obj)
    {
        prompt.text = "PLACEHOLDER_UNKNOWN_OBJECT";
        if (dropdownManager != null)
        {
            dropdownManager.DecreaseToolCount(obj);
        }
        obj.transform.position = teleportTarget.transform.position;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
        Debug.Log($"Relocated: {obj.name}");
    }

    private string PickRandomString(List<string> list)
    {
        var random = new System.Random();
        int index = random.Next(list.Count);
        return list[index];
    }
}