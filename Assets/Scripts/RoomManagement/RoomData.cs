using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Room/Room Data", order = 1)]
public class RoomData : ScriptableObject
{
    public string roomName;
    public List<string> dropdownItems;
    public GameObject[] roomPrefabs; // Array of GameObjects to spawn for the room
    public List<GameObject> requiredTools;
}
