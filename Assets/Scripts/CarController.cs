using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;
    private bool Cam;

    public Camera Cam1;
    public Camera Cam2;

    public Vector3 PreviousPosition;
    public float Speed;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    public float maxSteeringAngle = 30f;
    public float motorForce = 50f;
    public float brakeForce = 0f;

    public TMP_Text Dbug;
    private List<string> strings = new(new[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }); // 30

    private void Start()
    {
        Time.fixedDeltaTime = 1f / 60;
        Cam = true;
        Cam1.gameObject.SetActive(true);
        Cam2.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        Speed = 50f * (float)Math.Sqrt(
            (PreviousPosition.x - transform.position.x) * (PreviousPosition.x - transform.position.x) +
            (PreviousPosition.y - transform.position.y) * (PreviousPosition.y - transform.position.y) +
            (PreviousPosition.z - transform.position.z) * (PreviousPosition.z - transform.position.z));

        strings[10] = $"                       {Speed}";
        
        strings[3] = $"FPS: {1f / Time.deltaTime}";
        Dbug.text = $"00- {strings[0]}\n01- {strings[1]}\n02- {strings[2]}\n03- {strings[3]}\n04- {strings[4]}\n" +
                    $"05- {strings[5]}\n06- {strings[6]}\n07- {strings[7]}\n08- {strings[8]}\n09- {strings[9]}\n" +
                    $"10- {strings[10]}\n11- {strings[11]}\n12- {strings[12]}\n13- {strings[13]}\n14- {strings[14]}\n" +
                    $"15- {strings[15]}\n16- {strings[16]}\n17- {strings[17]}\n18- {strings[18]}\n19- {strings[19]}\n" +
                    $"20- {strings[20]}\n21- {strings[21]}\n22- {strings[22]}\n23- {strings[23]}\n24- {strings[24]}\n" +
                    $"25- {strings[25]}\n26- {strings[26]}\n27- {strings[27]}\n28- {strings[28]}\n29- {strings[29]}\n";
        
        strings[2] = $"Vertical: {MyApprox(verticalInput)}; Horizontal: {MyApprox(horizontalInput)}, IsBreaking: {isBreaking};";
        PreviousPosition = transform.position;
    }

    
    private void SwitchCameras()
    {
        Cam = !Cam;
        Cam1.gameObject.SetActive(Cam);
        Cam2.gameObject.SetActive(!Cam);
    }
    
    private void Update()
    {
        strings[4] = $"FPS: {1f / Time.deltaTime}";
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("hey");
            SwitchCameras();
        }
    }


    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleSteering()
    {
        steerAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
        strings[6] = $"frontLeftWheelCollider.steerAngle: {(frontLeftWheelCollider.steerAngle)}";
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        brakeForce = isBreaking ? 3000f : 0f;
        strings[7] = $"frontLeftWheelCollider.motorTorque: {MyApprox(verticalInput * motorForce)} frontRightWheelCollider.motorTorque: {MyApprox(verticalInput * motorForce)}; brakeForce: {MyApprox(brakeForce)}";
        
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }

    private void UpdateWheels()
    {
        UpdateWheelPos(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPos(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPos(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelPos(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheelPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        // trans.rotation = Quaternion.Euler( rot.x, rot.y, rot.z);
        strings[1] = $"X:{MyApprox(rot.x)}; Y:{MyApprox(rot.y)}; Z:{MyApprox(rot.z)}";
        trans.position = pos;
    }

    private float MyApprox(float x) => (int)(x * 100f) / 100f;
}