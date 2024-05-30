using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientVitalsSimulator : MonoBehaviour
{
    [SerializeField] [Tooltip("Maximum value for Systolic Pressure")]
    private int maxSystolic = 121;
    [SerializeField] [Tooltip("Maximum value for Diastolic Pressure")]
    private int maxDiastolic = 81;
    [SerializeField] [Tooltip("Maximum value for Heart Rate")]
    private int maxHeartRate = 111;

    private int systolicPressure { get; set; }
    private int diastolicPressure { get; set; }
    private int heartRate { get; set; }
    
    void Start()
    {
        GenerateNewVitals();
    }

    //TODO: Test
    public int[] GenerateNewVitals()
    {
        // Generate systolic blood pressure (SBP)
        systolicPressure = Random.Range(90, maxSystolic);

        // Generate diastolic blood pressure (DBP) based on SBP
        float correlationCoefficient = 0.74f; // Average correlation coefficient 
        float stdDev = 0.14f; // Standard deviation 
        float randomCorrelation = Random.Range(correlationCoefficient - stdDev, correlationCoefficient + stdDev);
        
        // Calculating the diastolic pressure 
        int expectedDiastolic = Mathf.RoundToInt((diastolicPressure + randomCorrelation * (systolicPressure - diastolicPressure)));
        diastolicPressure = Mathf.Clamp(expectedDiastolic, 60, maxDiastolic); // Clamping to ensure within expected range

        // Generate heart rate
        heartRate = Random.Range(60, maxHeartRate);

        return new int[] { systolicPressure, diastolicPressure, heartRate };
    }

    public int[] GetCurrentVitals()
    {
        return new int[] {systolicPressure, diastolicPressure, heartRate};
    }
   
    public int GetCurrentSystolicPressure()
    {
        return systolicPressure;
    }

    public int GetCurrentDiastolicPressure()
    {
        return diastolicPressure;
    }

    public int GetCurrentHeartRate()
    {
        return heartRate;
    }

    public int GetMaxSystolicPressure()
    {
        return maxSystolic;
    }

    public int GetMaxDiastolicPressure()
    {
        return maxDiastolic;
    }

    public int GetMaxHeartRate()
    {
        return maxHeartRate;
    }
}
