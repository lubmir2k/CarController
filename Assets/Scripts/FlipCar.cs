using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCar : MonoBehaviour
{
    Rigidbody rigidbody;
    float lastTimeChecked;
    
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    void RightCar()
    {
        this.transform.position += Vector3.up;
        this.transform.rotation = Quaternion.LookRotation(this.transform.forward);
    }
    
    void Update()
    {
        if(transform.up.y > 0.5f || rigidbody.velocity.magnitude > 1)
        {
            lastTimeChecked = Time.time;
        }

        if(lastTimeChecked > lastTimeChecked + 3)
        {
            RightCar();
        }
    }
}
