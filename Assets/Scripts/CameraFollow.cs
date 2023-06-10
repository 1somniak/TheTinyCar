using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    [SerializeField] private Vector3 offsetStraight;
    [SerializeField] private Vector3 offsetBackStraight;
    [SerializeField] private Vector3 offsetBackLeft;
    private Vector3 _offsetBackRight;
    
    [SerializeField] private GameObject target;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;
    private CarController _carController;
    

    private void Start()
    {
        Time.fixedDeltaTime = 1f / 60;
        _carController = target.GetComponent<CarController>();
        _offsetBackRight = new Vector3(-offsetBackLeft.x, offsetBackLeft.y, offsetBackLeft.z);
    }

    private void FixedUpdate()
    {
        GetOffset();
        HandleTranslation();
        HandleRotation();
    }

    private void GetOffset()
    {
        if (!_carController.isGoingForward && _carController.speed >= 0.6f)
        {
            switch (_carController.horizontalInput)
            {
                case >= 0.5f:
                    offset = _offsetBackRight;
                    break;
                case <= -0.5f:
                    offset = offsetBackLeft;
                    break;
                default: // quand on recule tout droit
                    offset = offsetBackStraight;
                    break;
            }
        }
        else
            offset = offsetStraight;
        
        
    }
   
    private void HandleTranslation()
    {
        var targetPosition = target.transform.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition + new Vector3(0, 1, 0), translateSpeed * Time.deltaTime);
    }
    
    private void HandleRotation()
    {
        var direction = target.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(direction + new Vector3(0, 1, 0), Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
