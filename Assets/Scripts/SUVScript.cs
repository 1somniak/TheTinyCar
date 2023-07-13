using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SUVScript : MonoBehaviour
{
    public ParticleSystem babParticles;
    public ParticleSystem triParticles;
    public GameObject car;
    // public GameObject f150;
    public GameObject wayPoint0;
    public GameObject wayPoint1;
    public GameObject wayPoint2;
    public GameObject wayPoint3;
    public GameObject wayPoint4;
    public GameObject aiScript;
    public GameObject cubeReference150;
    private Rigidbody _aiScript;
    public GameObject f150;
    public List<WheelCollider> wheelColliders;


    private void Start()
    {
        _aiScript = aiScript.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        triParticles = babParticles;
        var position = car.transform.position;
        wayPoint2.transform.position = position;
        wayPoint3.transform.position = position + new Vector3(0, 0, 100);
        wayPoint4.transform.position = position + new Vector3(0, 0, 150);
        if (cubeReference150.transform.position.z > 110)
        {
            print("wtf1   " + cubeReference150.transform.rotation.y + "        F150 speed: " + _aiScript.velocity.magnitude);
            if (cubeReference150.transform.rotation.y > 30)
            {
                aiScript.transform.rotation = Quaternion.Euler(0, 30, 0);
                print("hey");
            }
            if (cubeReference150.transform.rotation.y < -30)
            {
                aiScript.transform.rotation = Quaternion.Euler(0, -30, 0);
                print("hey");
            }

            if (_aiScript.velocity.magnitude < 1)
            {
                f150.transform.rotation = Quaternion.Euler(0, 0, 0);
                foreach (WheelCollider wheelCollider in wheelColliders)
                    wheelCollider.steerAngle = 0f;
            }
            
        }
    }
    
    private double Norm(Vector3 v) => Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}