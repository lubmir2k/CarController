using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive driveScript;

    void Start()
    {
        driveScript = this.GetComponent<Drive>();
    }

    void Update()
    {
        float accelerate = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");

        driveScript.Go(accelerate, steer, brake);

        driveScript.CheckForSkid();
        driveScript.CalculateEngineSound();
    }
}
