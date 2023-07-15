using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _initialPosition;
    public float speedRoation = 1f;
    private float _time;
    public GameObject rotator;
    private CarController _theTinyCar;
    private bool _onTheMap = true;
    private Vector3 _positionBeforeCharge;

    private void Start()
    {
        _initialPosition = rotator.transform.position;
        _time = 0;
        _theTinyCar = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        _onTheMap = true;
    }

    
    private void Update()
    {
        _time += Time.deltaTime; // rotation et flottement
        rotator.transform.Rotate(
            0f,
            Time.deltaTime * 360f * speedRoation,
            0f);

        rotator.transform.position = _initialPosition + new Vector3(
            0f,
            0.75f + (float)Math.Sin(_time * 3) / 6f - _initialPosition.y,
            0f);
        
        // destruction
        if (Vector3.Distance(transform.position, _theTinyCar.transform.position) < 0.75)
        {
            _onTheMap = false;
            _theTinyCar.ChargeBattery();
            _positionBeforeCharge = transform.position - _theTinyCar.transform.position;
        }
        Destruction();
    }

    public void Build() => Start();

    private void Destruction()
    {
        if (_onTheMap)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else
        {
            transform.position = _theTinyCar.gameObject.transform.position + _positionBeforeCharge;
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(0, 0, 0);
                Destroy(gameObject);
            }
            else
                transform.localScale -= Time.deltaTime * 5 * new Vector3(1f, 1f, 1f);

        }
    }
}
