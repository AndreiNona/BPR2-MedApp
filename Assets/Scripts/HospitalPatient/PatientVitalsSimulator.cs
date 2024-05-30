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

    // Use this for initialization
    void Start()
    {
        GenerateNewVitals();
    }

    //TODO: Test
    // Method to simulate generating new vital signs
    public int[] GenerateNewVitals()
    {
        // Randomly generate systolic blood pressure (SBP)
        systolicPressure = Random.Range(90, maxSystolic);

        // Generate diastolic blood pressure (DBP) based on SBP and an average correlation (r)
        float correlationCoefficient = 0.74f; // Average correlation coefficient from your data
        float stdDev = 0.14f; // Standard deviation in the correlation
        float randomCorrelation = Random.Range(correlationCoefficient - stdDev, correlationCoefficient + stdDev);
        
        // Calculating the diastolic pressure using the relationship observed
        int expectedDiastolic = Mathf.RoundToInt((diastolicPressure + randomCorrelation * (systolicPressure - diastolicPressure)));
        diastolicPressure = Mathf.Clamp(expectedDiastolic, 60, maxDiastolic); // Clamping to ensure within expected range

        // Randomly generate heart rate
        heartRate = Random.Range(60, maxHeartRate);

        return new int[] { systolicPressure, diastolicPressure, heartRate };
    }

    public int[] GetCurrentVitals()
    {
        return new int[] {systolicPressure, diastolicPressure, heartRate};
    }
    // Methods to get the current vital signs
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
    // Methods to get the current vital signs
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
