using System;
using TMPro;
using UnityEngine;

public class F150Script : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    private float steerAngle;
    private bool isBreaking;

    public GameObject target;
    public Vector3 previousPosition;
    public float Speed { get; set; }
    
    
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

    public double carWidth;
    public double carHeight;

    public TMP_Text speedTMP;
    
    // Start is called before the first frame update
    void Start()
    {
        carWidth = Norme(frontLeftWheelTransform.position - frontRightWheelTransform.position);
        carHeight = Norme(frontLeftWheelTransform.position - rearLeftWheelTransform.position);
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        UpdateSpeed();
    }
    
    private void GetInput()
    {
        horizontalInput = 0;
        verticalInput = 1;
        isBreaking = Input.GetKey(KeyCode.Space);
        // isBreaking = false;
    }
    
    
    private void HandleMotor() // avancer / reculer
    {
        var motorTorque = 10000 * (isBreaking ? -1 : 1);
        
        frontLeftWheelCollider.motorTorque = motorTorque * 0.5f;
        frontRightWheelCollider.motorTorque = motorTorque * 0.5f;
        rearLeftWheelCollider.motorTorque = motorTorque;
        rearRightWheelCollider.motorTorque = motorTorque;
        
        // if (speed is >= -0.1f and <= 0.5f && verticalInput == 0f)
        //     isBreaking = true;
        frontLeftWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
        frontRightWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
        rearLeftWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
        rearRightWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
    }
    
    private void HandleSteering() // tourner
    {
        steerAngle = maxSteeringAngle * horizontalInput; // + horizontalInput * 0.6f * Speed;
        switch (horizontalInput)
        {
            case < -0.1f:
                frontLeftWheelCollider.steerAngle = -90f + (float)MyAtan((MyTan(90d + steerAngle) * carHeight - carWidth) / carHeight);
                frontRightWheelCollider.steerAngle = steerAngle;
                break;
            case > 0.1f:
                frontLeftWheelCollider.steerAngle = steerAngle;
                frontRightWheelCollider.steerAngle = 90f - (float)MyAtan((MyTan(90d - steerAngle) * carHeight - carWidth) / carHeight);
                break;
            default:
                frontLeftWheelCollider.steerAngle = steerAngle;
                frontRightWheelCollider.steerAngle = steerAngle;
                break;
        }
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
        wheelCollider.GetWorldPose(out var pos, out var rot);
        trans.rotation = rot;
        trans.position = pos;
    }

    private void UpdateSpeed()
    {
        Speed = (float)Norme(transform.position - previousPosition) * Time.deltaTime;
        previousPosition = transform.position;
        speedTMP.text = $"F150 Speed: {MyApprox(Speed)}";
    }
    
    
    
    
    private float MyApprox(float x) => (int)(x * 100f) / 100f;
    private double MyTan(double a) => Math.Tan(Math.PI * a / 180d);
    private double MyAtan(double a) => Math.Atan(a) * 180d / Math.PI;
    private double MyAcos(double a) => Math.Acos(a * Math.PI / 180d) * 180d / Math.PI;
    private string ToStringVector3(Vector3 vector1) => $"X: {MyApprox(vector1.x)}; Y: {MyApprox(vector1.y)}; Z: {MyApprox(vector1.z)};";
    private double Norme(Vector3 v) => Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}
