using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antiroll : MonoBehaviour
{
    public float antirollValue = 5000.0f;
    public WheelCollider wheelLeftFront;
    public WheelCollider wheelRightFront;
    public WheelCollider wheelLeftBack;
    public WheelCollider wheelRightBack;
    public GameObject antirollBar;
    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = antirollBar.transform.localPosition;
    }

    void GroundWheels(WheelCollider wheelLeft, WheelCollider wheelRight)
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        bool groundedLeft = wheelLeft.GetGroundHit(out hit);
        if (groundedLeft)
            travelLeft = (-wheelLeft.transform.InverseTransformPoint(hit.point).y - wheelLeft.radius) / wheelLeft.suspensionDistance;

        bool groundedRight = wheelRight.GetGroundHit(out hit);
        if (groundedRight)
            travelRight = (-wheelRight.transform.InverseTransformPoint(hit.point).y - wheelRight.radius) / wheelRight.suspensionDistance;

        float antiRollForce = (travelLeft - travelRight) * antirollValue;

        if (groundedLeft)
            rigidbody.AddForceAtPosition(wheelLeft.transform.up * -antiRollForce, wheelLeft.transform.position);

        if (groundedRight)
            rigidbody.AddForceAtPosition(wheelRight.transform.up * antiRollForce, wheelRight.transform.position);
    }

    void FixedUpdate()
    {
        GroundWheels(wheelLeftFront, wheelRightFront);
        GroundWheels(wheelLeftBack, wheelRightBack);
    }
}
