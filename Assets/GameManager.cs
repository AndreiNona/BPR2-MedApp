using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    [SerializeField] private DropdownManager dropdownManager;
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private HospitalPatientController patientController;  
    
    private List<TieToObjectPosition> tieScripts = new List<TieToObjectPosition>();
    
    
    private bool _isToolPrep= false;
    private bool _isPatientPrep= false;
    
    //Don't need it if we use the patient controller
    private bool _isRested= false;
    
    private bool _areToolsClean= true;

    //TODO: Replace with new room lock system
    public bool isRoomLocked = false;
    
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        StartCoroutine(WaitForDialogManager());
    }
    private void CheckPatient()
    {
        Debug.Log("CheckPatient called");
        if (!patientController._isUp && patientController._hasGoodPosture)
        {
            _isPatientPrep = true;
            Debug.Log($"_isPatientPrep: {_isPatientPrep}");
        }
    }

    private void CheckTools()
{
    RoomData currentRoomData = dropdownManager.currentRoomData;
    Dictionary<string, int> toolCounts = dropdownManager.GetRequiredToolCounts();
    _areToolsClean = true; 
    
    foreach (GameObject requiredTool in currentRoomData.requiredTools)
    {
        string toolName = requiredTool.name; // Use prefab name as key
        if (!toolCounts.ContainsKey(toolName) || toolCounts[toolName] < 1)
        {
            _areToolsClean = false;
            Debug.Log($"Required tool {toolName} has not been instantiated.");
            dialogManager.ShowDialogNode(11);
            return; // Exit early
        }
    }
    
    // Check the cleanliness of each required tool
    foreach (GameObject requiredTool in currentRoomData.requiredTools)
    {
        string toolName = requiredTool.name;
        if (toolCounts.ContainsKey(toolName) && toolCounts[toolName] > 0)
        {
            GameObject toolInstance = FindToolInstanceByName(toolName);
            if (toolInstance != null)
            {
                CleanableObject cleanable = toolInstance.GetComponent<CleanableObject>();
                if (cleanable != null && !cleanable.isClean)
                {
                    _areToolsClean = false;
                    break;
                }
            }
        }
    }
    if (_areToolsClean)
    {
        _isToolPrep = true;
        Debug.Log("All required tools are clean and ready.");
    }
    else
    {
        dialogManager.ShowDialogNode(3); // Show dialog indicating some tools are not clean.
        Debug.Log("Some required tools are not clean.");
    }

    Debug.Log($"_areToolsClean: {_areToolsClean}");
}

    // Helper method to find an instance of a tool by name
    private GameObject FindToolInstanceByName(string toolName)
{
    GameObject[] allTools = GameObject.FindGameObjectsWithTag("Tool"); // Assuming all tools are tagged "Tool"
    foreach (var tool in allTools)
    {
        if (tool.name == toolName)
            return tool;
    }
    return null;
}
    private void HandleDialogBasedOnPreparation()
{
    switch (_isPatientPrep)
    {
        case false when !_isToolPrep:
            Debug.Log("HandleDialogBasedOnPreparation: do nothing");
            break;
        case true when _isToolPrep:
            dialogManager.ShowDialogNode(7);
            Debug.Log("Both preparations complete. Moving to node 7.");
            break;
        case false:
            dialogManager.ShowDialogNode(5);
            Debug.Log("Patient is not ready. Moving to node 5.");
            break;
        default:
        {
            if (!_isToolPrep)
            {
                dialogManager.ShowDialogNode(6);
                Debug.Log("Tools are not ready. Moving to node 6.");
            }
            break;
        }
    }
}

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator WaitForDialogManager() {

        while (FindFirstObjectByType<DialogManager>() == null) {
            yield return null; 
        }
        dialogManager = FindFirstObjectByType<DialogManager>();
        dialogManager.InitializeDialogNodes();
        dialogManager.ShowDialogNode(0); 
    }
    public void GeneratePatientVitals(string[] attachmentNames)
    {
        patientController.UpdateVitalsAndSliders(attachmentNames);
    }
    public void UpdateTieToObjectPositions(List<TieToObjectPosition> tiePositions)
    {
        tieScripts = tiePositions;
        Debug.Log($"Updated GameManager with {tiePositions.Count} tie positions.");
    }

    public void PreparePatient(string actionName)
    {
        Debug.Log($"PreparePatient called with: {actionName}");
        switch (actionName)
        {
            case "PreparePatient":
                CheckPatient();
                break;
            case "PrepTools":
                CheckTools();
                break;
        }
        HandleDialogBasedOnPreparation();
    }
    
    public void ManageAttachable(string objectName, bool desiredState, int targetNode, int failNode, int badPosNode)
    {
        bool isCuffPlaced = false;
        bool isCorrectlyPositioned = false;

        // Find the object with the desired name from the list
        foreach (var script in tieScripts)
        {
            if (script.publicEditorName == objectName)
            {
                Debug.Log("ManageAttachable for: " + script.publicEditorName);
                isCuffPlaced = script.isTied;
                isCorrectlyPositioned = script.IsObjectCorrectlyPositioned();

                if (isCorrectlyPositioned)
                    script.LockObject(); 
                break;
            }
        }

        if (!isCorrectlyPositioned)
        {
            Debug.Log("isCorrectlyPositioned: is false");
            dialogManager.ShowDialogNode(badPosNode); // Show default node when the position is incorrect
        }
        else
            dialogManager.ShowDialogNode(isCuffPlaced == desiredState ? targetNode : failNode);
    }

    public void UnlockAllAttachable()
    {
        foreach (var script in tieScripts)
        {
            script.UnlockObject();
        }
    }
    public void PatientPosture(string postureName)
    {
        switch (postureName)
        {
            case "Sit":
                patientController.SetPostureState(PatientAnimationController.PostureState.Sitting);
                break;
            case "StandUp":
                patientController.SetPostureState(PatientAnimationController.PostureState.Standing);
                break;
            case "GoodPosture":
                patientController.SetHasGoodPosture(true);
                break;
            case "BadPosture":
                patientController.SetHasGoodPosture(false);
                break;
            case "CrossLegged":
                if (patientController.animationController.IsSitting())
                    patientController.SetLegState(PatientAnimationController.LegState.CrossLegged);
                break;
            case "LegsNormal":
                if (patientController.animationController.IsSitting())
                    patientController.SetLegState(PatientAnimationController.LegState.Normal);
                break;
            case "ArmsUp":
                if (patientController.animationController.IsSitting())
                    patientController.SetArmState(PatientAnimationController.ArmState.Up);
                break;
            case "ArmsDown":
                if (patientController.animationController.IsSitting())
                    patientController.SetArmState(PatientAnimationController.ArmState.Down);
                break;
            case "ResetPosture":
                patientController.ResetToDefaultPosture();
                break;
            case "ResetIdle":
                patientController.ResetToDefaultIdleState();
                break;
            default:
                Debug.LogWarning("Unknown posture: " + postureName);
                break;
        }
    }
}
