using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Drive: MonoBehaviour
{
    public WheelCollider[] wheelColliders;
    public GameObject[] wheels;
    public float torque = 200;
    public float maxSteerAngle = 30;
    public float maxBreakTorque = 500;

    public AudioSource skidSound;
    public AudioSource highAccel;

    public Transform skidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public ParticleSystem smokePrefab;
    ParticleSystem[] skidSmoke = new ParticleSystem[4];

    public Rigidbody rigidbody;
    public float gearLength = 3;
    public float currentSpeed {  get { return rigidbody.velocity.magnitude * gearLength; } }
    public float lowPitch = 1f;
    public float hightPitch = 6f;
    public int numGears = 5;
    public float maxSpeed = 200f;
    float rpm;
    int currentGear = 1;
    float currentGearPercentage;

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
            skidTrails[i] = Instantiate(skidTrailPrefab);

        skidTrails[i].parent = wheelColliders[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * wheelColliders[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null)
            return;

        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }

    void Start()
    {
        // Initialize particle system
        for (int i = 0; i < 4; i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
        // TODO Turn off the brake lights
        
    }

    public void CalculateEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);        
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));

        currentGearPercentage = Mathf.Lerp(currentGearPercentage, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPercentage);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;        

        if (currentGear > 0 && speedPercentage < downGearMax)
            currentGear--;

        if (speedPercentage > upperGearMax && (currentGear < (numGears - 1)))
            currentGear++;

        float pitch = Mathf.Lerp(lowPitch, hightPitch, rpm);
        highAccel.pitch = Mathf.Min(hightPitch, pitch) * 0.25f;
    }

    public void Go(float acceleration, float steer, float brake)
    {
        acceleration = Mathf.Clamp(acceleration, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBreakTorque;

        float thrustTorque = 0;
        if(currentSpeed < maxSpeed)
            thrustTorque = acceleration * torque;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = thrustTorque;

            if (i < 2)
                wheelColliders[i].steerAngle = steer;
            else
                wheelColliders[i].brakeTorque = brake;

            Quaternion quaternion;
            Vector3 position;
            wheelColliders[i].GetWorldPose(out position, out quaternion);
            wheels[i].transform.position = position;
            wheels[i].transform.rotation = quaternion;
        }        
    }

    public void CheckForSkid()
    {
        int numSkidding = 0;
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            wheelColliders[i].GetGroundHit(out wheelHit);

            // Get slip values
            if(Mathf.Abs(wheelHit.forwardSlip) >= 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.4f)
            {
                numSkidding += 1;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();                    
                }
                // StartSkidTrail(i);
                skidSmoke[i].transform.position = wheelColliders[i].transform.position - wheelColliders[i].transform.up * wheelColliders[i].radius;
                skidSmoke[i].Emit(1);
            }
            else
            {
                // EndSkidTrail(i);
            }
        }

        if(numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }
}
