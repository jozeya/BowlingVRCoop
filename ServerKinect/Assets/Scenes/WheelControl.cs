using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float MotorForce, SteerForce, BrakeForce;
    public WheelCollider Wheel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical") * MotorForce;
        float h = Input.GetAxis("Horizontal") * SteerForce;

        
        Wheel.motorTorque = v;
        Wheel.steerAngle = h;

        if (Input.GetAxis("Vertical") == 0)
        {
            Wheel.brakeTorque = BrakeForce;
            Wheel.brakeTorque = BrakeForce;
        }
        else
        {
            Wheel.brakeTorque = 0;
            Wheel.brakeTorque = 0;
        }
    }
}
