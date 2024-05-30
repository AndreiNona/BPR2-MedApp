using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelector : MonoBehaviour
{
    // Singleton instance
    public static RoomSelector instance;
    private GameManager gameManager;
    public TMP_Dropdown dropdownDefault;
    public TMP_Dropdown dropdownTheory;
    public TMP_Dropdown dropdownPractice;

    public TMP_Text subtitle;

    [SerializeField]
    private GameObject[] defaultRooms; // Main, Theory, Practice rooms
    [SerializeField] 
    [Tooltip("Theory Rooms to load ")]
    private GameObject[] theoryRooms;
    [SerializeField] 
    [Tooltip("Practice Rooms to load ")]
    private GameObject[] practiceRooms;


    public string CurrentRoomName { get; private set; }
    
    private GameObject _currentRoom;
    
    private  List<int> _roomHistory;
    
    
    void Awake()
    {
        Debug.Log("RoomSelector Awake called.");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("RoomSelector instance set and marked as DontDestroyOnLoad.");
        }
        else if (instance != this)
        {
            Debug.Log("Duplicate RoomSelector instance found. Destroying.");
            Destroy(gameObject);
        }
    }
    void Start()
    { 
        gameManager = GameManager.instance;
        InitializeDropdown(dropdownDefault, new List<string> { "Select a Room", "Main Room", "Theory Room", "Practice Room" });
        InitializeDropdown(dropdownTheory, ConvertToDropdownList(theoryRooms));
        InitializeDropdown(dropdownPractice, ConvertToDropdownList(practiceRooms));

        // Set event listeners
        dropdownDefault.onValueChanged.AddListener(index => ChangeRoom(dropdownDefault, defaultRooms, index));
        dropdownTheory.onValueChanged.AddListener(index => ChangeRoom(dropdownTheory, theoryRooms, index));
        dropdownPractice.onValueChanged.AddListener(index => ChangeRoom(dropdownPractice, practiceRooms, index));

        // Initialize to load main room on start
        ActivateRoom(defaultRooms[0]); // Assuming main room is the first in the array
    }
    List<string> ConvertToDropdownList(GameObject[] rooms)
    {
        List<string> roomNames = new List<string> { "Select a Room" };
        foreach (var room in rooms)
        {
            roomNames.Add(room.name);
        }
        return roomNames;
    }
    void InitializeDropdown(TMP_Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }
    
    private void ChangeRoom(TMP_Dropdown dropdown, GameObject[] roomArray, int index)
    {
        if (gameManager.isRoomLocked)
        {
            Debug.Log("Room change is locked.");
            subtitle.text = "Room is locked, please complete the current session";
            return;
        }
        if (index <= 0) return; // Ignore "Select a Room" placeholder

        // Reset other dropdowns
        ResetOtherDropdowns(dropdown);
        
        ActivateRoom(roomArray[index - 1]); // Adjust for placeholder

        // Reset index after room activation to ensure re-selectability
        dropdown.value = 0;
        dropdown.RefreshShownValue();
        subtitle.text = "Room successful changed";
    }
    private void ResetOtherDropdowns(TMP_Dropdown activeDropdown)
    {
        // Disable listeners temporarily to avoid triggering changes
        dropdownDefault.onValueChanged.RemoveListener(index => ChangeRoom(dropdownDefault, defaultRooms, index));
        dropdownTheory.onValueChanged.RemoveListener(index => ChangeRoom(dropdownTheory, theoryRooms, index));
        dropdownPractice.onValueChanged.RemoveListener(index => ChangeRoom(dropdownPractice, practiceRooms, index));

        // Reset dropdowns to default value
        if (activeDropdown != dropdownDefault)
        {
            dropdownDefault.value = 0;
            dropdownDefault.RefreshShownValue();
        }
        if (activeDropdown != dropdownTheory)
        {
            dropdownTheory.value = 0;
            dropdownTheory.RefreshShownValue();
        }
        if (activeDropdown != dropdownPractice)
        {
            dropdownPractice.value = 0;
            dropdownPractice.RefreshShownValue();
        }

        // Re-enable listeners
        dropdownDefault.onValueChanged.AddListener(index => ChangeRoom(dropdownDefault, defaultRooms, index));
        dropdownTheory.onValueChanged.AddListener(index => ChangeRoom(dropdownTheory, theoryRooms, index));
        dropdownPractice.onValueChanged.AddListener(index => ChangeRoom(dropdownPractice, practiceRooms, index));
    }
    private void ActivateRoom(GameObject room)
    {
        if (_currentRoom != null) _currentRoom.SetActive(false);
        _currentRoom = room;
        CurrentRoomName = room.name;
        _currentRoom.SetActive(true);
    }
}

