using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SUVScript : MonoBehaviour
{
    public ParticleSystem babParticles;
    public ParticleSystem triParticles;
    public GameObject wayPoint1;
    public GameObject wayPoint2;
    public GameObject wayPoint3;
    
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        triParticles = babParticles;
    }
}
