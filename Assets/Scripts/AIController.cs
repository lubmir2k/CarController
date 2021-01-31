using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    public float brakingSensitivity = 3f;
    Drive driveScript;
    public float steeringSensitivity = 0.01f;
    public float accelerationSensitivity = 0.3f;
    Vector3 target;
    Vector3 nextTarget;
    float totalDistanceToTarget;
    int currentWayPoint = 0;

    void Start()
    {
        driveScript = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWayPoint].transform.position;
        nextTarget = circuit.waypoints[currentWayPoint + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, driveScript.rigidbody.gameObject.transform.position);
    }

    void Update()
    {
        Vector3 localTarget = driveScript.rigidbody.gameObject.transform.InverseTransformPoint(target);
        Vector3 nextLocalTarget = driveScript.rigidbody.gameObject.transform.InverseTransformPoint(nextTarget);
        float distanceToTarget = Vector3.Distance(target, driveScript.rigidbody.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(driveScript.currentSpeed);

        float distanceFactor = distanceToTarget / totalDistanceToTarget;
        float speedFactor = driveScript.currentSpeed / driveScript.maxSpeed;

        float accelerate = Mathf.Lerp(accelerationSensitivity, 1, distanceFactor);
        float brake = Mathf.Lerp((-1 - Mathf.Abs(nextTargetAngle)) * brakingSensitivity, 1 + speedFactor, 1 - distanceFactor);

        if(Mathf.Abs(nextTargetAngle) > 20)
        {
            brake += 0.8f;
            accelerate -= 0.8f;
        }

        /*
        if (distanceToTarget < 5) // braking and reducing gas - curves
        {
            brake = 0.6f;
            accelerate = 0.3f;
        }
        */

        driveScript.Go(accelerate, steer, brake);

        if(distanceToTarget < 4) // make larger is car starts to circle WP
        {
            currentWayPoint++;
            if (currentWayPoint >= circuit.waypoints.Length)
                currentWayPoint = 0;

            target = circuit.waypoints[currentWayPoint].transform.position;
            nextTarget = circuit.waypoints[currentWayPoint + 1].transform.position;

            totalDistanceToTarget = Vector3.Distance(target, driveScript.rigidbody.gameObject.transform.position);
        }

        driveScript.CheckForSkid();
        driveScript.CalculateEngineSound();
    }
}
