using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Vector3 speed;
    public Quaternion rotation;
    
    void Start()
    {
        transform.rotation = rotation;
    }
    
    
    void Update()
    {
        transform.Translate(speed.x * Time.deltaTime, speed.y * Time.deltaTime, speed.z * Time.deltaTime);
    }
}
