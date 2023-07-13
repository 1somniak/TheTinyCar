using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed;
    public int way;
    public List<Transform> wheelTransforms;
    public List<WheelCollider> wheelColliders;
    public Rigidbody myRigidBody;
    private float ActualSpeed => myRigidBody.velocity.magnitude * 3.6f;
    public int raycastDistance = 100;
    private float _motorForce = 10000;

    private void Start()
    {
        // transform.rotation = Quaternion.Euler(0f, speed >= 0f ? 180f : 0f, 0f);
        _motorForce = gameObject.transform.name.Length >= 4 && gameObject.transform.name.Substring(0, 4) is "Bus_"
            ? 3000
            : 10000;
    }
    
    
    private void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
        {
            Debug.Log("raycast est touché mdr");
        }
        UpdateWheelsPos();
    }

    private void HandleMotor()
    {
        if (ActualSpeed < speed && IsMovingObjectForward())
        {
            
        } // 453.49 - 96.39 = 357.1
    }
    
    
    private void UpdateWheelsPos()
    {
        for (var i = 0; i < 4; i++)
        {
            wheelColliders[i].GetWorldPose(out var pos, out var rot);
            wheelTransforms[i].rotation = rot;
            wheelTransforms[i].position = pos;
        }
    }

    private bool IsMovingObjectForward()
    {
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, raycastDistance))
            return false;
        else
            return hit.transform.name.Length >= 4 &&
                   hit.transform.name.Substring(0, 4) is "Bus_" or "Car_" or "Poli" or "Taxi";
    }

    private bool IsBusCarTaxiPolice(string s) => s.Length >= 4 && s.Substring(0, 4) is "Bus_" or "Car_" or "Poli" or "Taxi";

    private float Norm(Vector3 v) => (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}