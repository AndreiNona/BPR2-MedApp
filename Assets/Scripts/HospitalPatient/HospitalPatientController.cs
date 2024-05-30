using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HospitalPatientController : MonoBehaviour
{
    [SerializeField] public PatientAnimationController animationController;
    [SerializeField] private PatientVitalsSimulator vitalsSimulator;
    [SerializeField] private SliderController systolicSliderController;
    [SerializeField] private SliderController diastolicSliderController;
    [SerializeField] private SliderController heartRateSliderController;
    public bool _isUp { get; private set; }
    public bool _hasGoodPosture{ get; private set; }
    private bool isComplaining =true;
    
    private List<TieToObjectPosition> tieToObjectPositions = new List<TieToObjectPosition>();
    
    private bool savedIsUp;
    private bool savedHasGoodPosture;

    private bool savedIsComplaining;


    void Start()
    {
        InitializeTieScripts();
        ResetVitalsAndSliders();  // Ensure initial state is reset
    }
    private void InitializeTieScripts()
    {
        Transform hospitalPatientTransform = transform;
        // Assuming all relevant objects are children of this GameObject
        Debug.Log("Searching for TieToObjectPosition components within children tagged 'Attachment'...");

        // Recursively search for objects with the TieToObjectPosition component under the HospitalPatientController GameObject
        SearchAndAddTieScripts(hospitalPatientTransform);
        // After collecting all scripts, pass them to the patient controller
        UpdateTieToObjectPositions(tieToObjectPositions);
    }
    //TODO: Find a way to avoid recursively searching
    private void SearchAndAddTieScripts(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Check if the child is tagged with "Attachment" and contains the correct component
            if (child.CompareTag("Attachment"))
            {
                TieToObjectPosition script = child.GetComponent<TieToObjectPosition>();
                if (script != null)
                {
                    Debug.Log($"Added object with TieToObjectPosition: {child.gameObject.name}");
                    tieToObjectPositions.Add(script);
                }
            }
            // Recursively search in the current child's children
            SearchAndAddTieScripts(child);
        }
    }
    public void UpdateTieToObjectPositions(List<TieToObjectPosition> tiePositions)
    {
        GameManager.instance.UpdateTieToObjectPositions(tiePositions);
    }
    public void SetHasGoodPosture(bool value)
    {
        _hasGoodPosture = value;
        UpdatePostureBasedOnState();
    }

    public bool GetHasGoodPosture()
    {
        return _hasGoodPosture;
    }
    public void SetPostureState(PatientAnimationController.PostureState state)
    {
        animationController.SetPostureState(state);
    }

    public void SetLegState(PatientAnimationController.LegState state)
    {
        animationController.SetLegState(state);
    }

    public void SetArmState(PatientAnimationController.ArmState state)
    {
        animationController.SetArmState(state);
    }
    public void ToggleSit()
    {
        animationController.SitDown();
        UpdatePostureBasedOnState();
        _isUp = false;
    }

    public void ToggleStandUp()
    {
        animationController.StandUp();
        animationController.ToggleCrossLegged(false); 
        animationController.ToggleArmsUp(false);
        _isUp = true;
    }

    //TODO: Fix
    public void ToggleGoodPosition()
    {
        if (isComplaining)
            isComplaining = !isComplaining;
        else
            _hasGoodPosture = !_hasGoodPosture;
        
        UpdatePostureBasedOnState();
    }
    public void ResetToDefaultIdleState()
    {
        SetPostureState(PatientAnimationController.PostureState.Standing);
        SetLegState(PatientAnimationController.LegState.Normal);
        SetArmState(PatientAnimationController.ArmState.Down);
    }
    public void ResetToDefaultPosture()
    {
        animationController.ResetPosture();
        // Animation state is reset to idle
        ResetToDefaultIdleState();
    }

    public void UpdatePostureBasedOnState()
    {
        if (animationController.IsSitting()) 
        {
            if (_hasGoodPosture)
            {
                SetArmState(PatientAnimationController.ArmState.Up);
                SetLegState(PatientAnimationController.LegState.Normal);
            }
            else if (isComplaining)
            {
                SetLegState(PatientAnimationController.LegState.CrossLegged);
                SetArmState(PatientAnimationController.ArmState.Down);
            }
            else
            {
                SetLegState(PatientAnimationController.LegState.Normal);
                SetArmState(PatientAnimationController.ArmState.Down);
            }
        }
    }
    private bool HasSpecificAttachment(string[] attachmentNames)
    {
        // Track the number of matches found
        int matchCount = 0;

        foreach (string attachmentName in attachmentNames)
        {
            bool foundMatchForCurrentName = false;

            foreach (TieToObjectPosition tie in tieToObjectPositions)
            {
                Debug.Log($"Comparing to: {tie.publicEditorName}, isTied: {tie.isTied}, Target Name: {attachmentName}");

                if (tie.publicEditorName == attachmentName && tie.isTied)
                {
                    Debug.Log($"Match found: {attachmentName} is tied.");
                    foundMatchForCurrentName = true;
                    break; // Stop checking once a match is found for the current name
                }
            }

            if (foundMatchForCurrentName)
                matchCount++;  
            else
            {
               
                Debug.Log($"No tied attachment found with name: {attachmentName}.");
                return false; 
            }
        }
        // Check if the number of matches found equals the number of names provided
        if (matchCount == attachmentNames.Length)
            return true; 
        return false; // Ideally never be hit due to earlier checks

    }

    private int[] GetVitals(string[] attachmentNames)
    {
        Debug.Log($"Attempting to get vitals with attachment: {attachmentNames}, Up: {_isUp}, Good Posture: {_hasGoodPosture}, Has Specific Attachment: {HasSpecificAttachment(attachmentNames)}");
        // Check the conditions
        if (!_isUp && _hasGoodPosture && HasSpecificAttachment(attachmentNames))
        {
            Debug.Log($"Conditions met for {attachmentNames}: Up: {_isUp} (expected False), Good Posture: {_hasGoodPosture} (expected True)");
            return vitalsSimulator.GenerateNewVitals() ;
        }
        Debug.Log($"Conditions not met for {attachmentNames}: Up: {_isUp}, Good Posture: {_hasGoodPosture}");
        return null;  // Return null if conditions are not met
    }
    public void UpdateVitalsAndSliders(string[] attachmentNames)
    {
        Debug.Log($"Updating vitals and sliders for attachment: {attachmentNames}");
        if (!_isUp && _hasGoodPosture)
        {
            int[] vitals = GetVitals(attachmentNames);
            if (vitals != null)
            {
                Debug.Log($"Vitals received for {attachmentNames}: Systolic: {vitals[0]}, Diastolic: {vitals[1]}, Heart Rate: {vitals[2]}");
                
                // Enable sliders
                systolicSliderController.gameObject.SetActive(vitals[0] > 0);
                diastolicSliderController.gameObject.SetActive(vitals[1] > 0);
                heartRateSliderController.gameObject.SetActive(vitals[2] > 0);
                
                // Update sliders
                systolicSliderController.AnimateSlider(vitals[0] / (float)vitalsSimulator.GetMaxSystolicPressure(), vitals[0]);
                diastolicSliderController.AnimateSlider(vitals[1] / (float)vitalsSimulator.GetMaxDiastolicPressure(), vitals[1]);
                heartRateSliderController.AnimateSlider(vitals[2] / (float)vitalsSimulator.GetMaxHeartRate(), vitals[2]);
            }
            else
                Debug.Log($"Vitals not updated for {attachmentNames} due to null values.");
        }
        else
        {
            Debug.Log($"Failed to update vitals for {attachmentNames}: Up: {_isUp}, Good Posture: {_hasGoodPosture}");
        }
    }

    private void ResetVitalsAndSliders()
    {
        // Update sliders to zero and disable them
        systolicSliderController.AnimateSlider(0, 0);
        diastolicSliderController.AnimateSlider(0, 0);
        heartRateSliderController.AnimateSlider(0, 0);

        systolicSliderController.gameObject.SetActive(false);
        diastolicSliderController.gameObject.SetActive(false);
        heartRateSliderController.gameObject.SetActive(false);
    }

    public int GetCurrentHeartRate()
    {
        return vitalsSimulator.GetCurrentHeartRate();
    }
    
    private void SaveState()
    {
        savedIsUp = _isUp;
        savedHasGoodPosture = _hasGoodPosture;
        savedIsComplaining = isComplaining;
    }
    private void OnDisable()
    {
        SaveState();
    }
}
