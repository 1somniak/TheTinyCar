using System;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Vector3 speed;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, speed.z >= 0f ? 180f : 0f, 0f);
    }
    
    
    void Update()
    {
        transform.Translate(Math.Abs(speed.x * Time.deltaTime / 3.6f), Math.Abs(speed.y * Time.deltaTime / 3.6f), Math.Abs(speed.z * Time.deltaTime / 3.6f));
    }

    private float Norm(Vector3 v) => (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}