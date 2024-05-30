using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
  
    [SerializeField] 
    [Tooltip("Angle for object spin")]
    private Vector3 angle;

    private int _constant;

    private bool _isSpinning;

    private void Start()
    {
        _isSpinning = true;
        _constant = 2;
    }

    void Update()
    {
        if(_isSpinning)
            transform.Rotate(angle.x * Time.deltaTime, angle.y * Time.deltaTime, angle.z * Time.deltaTime, Space.Self);
    }

    public void stopSpin()
    {
        _isSpinning = false;
    }

    public void startSpin()
    {
        _isSpinning = true;
    }

    public void ChangeSpeed(int denominator)
    {
    
            switch (denominator)
            {
                case 0:
                    angle =  Vector3.zero;
                    break;
                case -1:
                    angle.x -= _constant;
                    break;
                case -2:
                    angle.y -= _constant;
                    break;
                case -3:
                    angle.z -= _constant;
                    break;
                case 1:
                    angle.x += _constant;
                    break;
                case 2:
                    angle.y += _constant;
                    break;
                case 3:
                    angle.z += _constant;
                    break;
            }
            Debug.Log("Speed is: X:"+ angle.x+ " Y: " +angle.y + " Z: "+ angle.z);
            
    }
}
