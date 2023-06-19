using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    public GameObject car;
    private Vector3 distance;
    // Start is called before the first frame update
    void Start()
    {
        // Time.fixedDeltaTime = 1f;
        distance = car.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log($"distance {distance}");
        transform.position = car.transform.position - distance;
    }
}
