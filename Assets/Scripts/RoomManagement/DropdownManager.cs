using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{ 
    [SerializeField] private Transform location;
    
    public TMP_Dropdown dropdown;
    public List<RoomData> roomDatas;  // List of all room data scriptable objects

    
    public RoomData currentRoomData{ get; private set; }
    private string _currentRoomName;
    
    private Dictionary<string, int> requiredToolCounts = new Dictionary<string, int>();
    
    [SerializeField] 
    [Tooltip("Only one object per room")] private bool isMono;
    
    // List to keep track of instantiated objects
    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private void Start()
    {

        if (RoomSelector.instance == null)
        {
            Debug.LogError("RoomSelector instance is not set.");
            return;
        }

        _currentRoomName = RoomSelector.instance.CurrentRoomName;
        UpdateDropdown(_currentRoomName);
        dropdown.onValueChanged.AddListener(HandleDropdownChange);
        ToggleDropdownVisibility();
    }

    //TODO: Replace with something more efficient
    private void Update()
    {
        if (_currentRoomName != RoomSelector.instance.CurrentRoomName)
        {
            _currentRoomName = RoomSelector.instance.CurrentRoomName;
            ClearInstantiatedObjects();  // Clear objects when room changes
            UpdateDropdown(_currentRoomName);
            ToggleDropdownVisibility();
        }
    }

    private void HandleDropdownChange(int index)
    {
        int prefabIndex = index - 1;
        if (prefabIndex < 0 || prefabIndex >= currentRoomData.roomPrefabs.Length)
        {
            if (prefabIndex != -1)  // Assuming -1 is the "Select an item..." index
            {
                Debug.LogError("Selected index is out of range or not selectable.");
                return;
            }
        }

        if (isMono)
            ClearCurrentRoomObjects();

        GameObject prefab = currentRoomData.roomPrefabs[prefabIndex];
        GameObject newObject = Instantiate(prefab, location.position, location.rotation);
        newObject.name = prefab.name;

        // Update dictionary with prefab name instead of GameObject
        string prefabName = prefab.name;
        if (currentRoomData.requiredTools.Contains(prefab))
        {
            if (!requiredToolCounts.ContainsKey(prefabName))
            {
                requiredToolCounts[prefabName] = 0;
            }
            requiredToolCounts[prefabName]++;
        }
        instantiatedObjects.Add(newObject);
        Debug.Log("Created: " + newObject.name);
        StartCoroutine(ResetDropdown());
    }

    private IEnumerator ResetDropdown()
    {
        yield return null; // Wait for the end of the frame
        dropdown.value = 0; // Reset to non-selectable prompt
        dropdown.RefreshShownValue();
    }

    public void DecreaseToolCount(GameObject prefab)
    {
        string prefabName = prefab.name; // Assuming this is unique
        Debug.Log($"Try to decrease count of {prefabName}, instance ID: {prefab.GetInstanceID()}");

        if (requiredToolCounts.ContainsKey(prefabName) && requiredToolCounts[prefabName] > 0)
        {
            requiredToolCounts[prefabName]--;
            Debug.Log($"Decreased count of {prefabName} to {requiredToolCounts[prefabName]}");
        }
        else
        {
            Debug.Log("Prefab not found in dictionary or count is zero.");
            Debug.Log("Current keys in dictionary:");
            foreach (var key in requiredToolCounts.Keys)
            {
                Debug.Log($"Key: {key}");
            }
        }
    }
    public Dictionary<string, int> GetRequiredToolCounts()
    {
        return requiredToolCounts;
    }

    private void ClearCurrentRoomObjects()
    {
        foreach (GameObject prefab in currentRoomData.roomPrefabs)
        {
            string prefabName = prefab.name;
            if (requiredToolCounts.ContainsKey(prefabName))
            {
                requiredToolCounts[prefabName] = 0;
            }
        }
    }
    private void ClearInstantiatedObjects()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();
    }
    private void UpdateDropdown(string newRoomName)
    {
        dropdown.options.Clear(); // Clear current options
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        // Add a non-selectable prompt to the dropdown
        newOptions.Add(new TMP_Dropdown.OptionData("Select an item..."));

        currentRoomData = roomDatas.Find(room => room.roomName == newRoomName);
        if (currentRoomData != null)
        {
            foreach (var item in currentRoomData.dropdownItems)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(item));
            }
        }
        dropdown.AddOptions(newOptions);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }
    private void ToggleDropdownVisibility()
    {
        // Set active state based on whether there are any options
        dropdown.gameObject.SetActive(dropdown.options.Count > 0);
    }
}
