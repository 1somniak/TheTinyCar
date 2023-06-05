using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject Car;
    public Vector3 Position = new Vector3(-8f, 6f, 0f);
    public Quaternion Rotation = Quaternion.Euler(35, 90, 0f);
    
    
    // Start is called before the first frame update
    public void Start()
    {
        transform.rotation = Rotation;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        var carPosition = Car.transform.position;
        transform.position = new Vector3(carPosition.x + Position.x, carPosition.y + Position.y, carPosition.z + Position.z);
    }
}
