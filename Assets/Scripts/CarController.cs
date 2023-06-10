using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class CarController : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    private float steerAngle;
    private bool isBreaking;
    public float Battery;

    private bool _isGoingForward
    {
        get
        {
            var myPosition = transform.position;
            var CPosition = cube.transform.position;
            var AB = Norme(myPosition - previousPosition); // vecteur qui bouge tout le temps
            var AC = Norme(myPosition - CPosition); // vecteur fixe
            var BC = Norme(previousPosition - CPosition); // hypotenuse
            strings[16] = $"AB: {MyApprox((float)AB)}; AC: {MyApprox((float)AC)}; BC: {MyApprox((float)BC)};";
            // var theta = (MyAcos((-BC * BC + AB * AB + AC * AC) / (2 * AB * AC)) + 360) % 360;
            return BC * BC >= AB * AB + AC * AC;
        }
    }
    public bool isGoingForward;

    public Vector3 previousPosition;
    public float speed;
    public double carWidth = 130d;
    public double carHeight = 185d;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;
    public GameObject cube;

    public float maxSteeringAngle = 30f;
    public float motorForce = 50f;
    public float brakeForce = 0f;

    public Sprite[] Sprites;
    public Image Cadrant;
    public Image StopImage;

    public TMP_Text Dbug;
    private List<string> strings = new(new[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }); // 30
    public TMP_Text speedText;
    
    private void Start()
    {
        Time.fixedDeltaTime = 1f / 60;
        transform.position = new Vector3(500f, 0f, 500f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        previousPosition = new Vector3(500f, 0f, 500f);
        ChargeBattery();
    }

    private void FixedUpdate()
    {
        isGoingForward = _isGoingForward;
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        var position = transform.position;
        GetSpeed(position);

        ChangeStrings();
        
        previousPosition = position;
    }

    private void ChargeBattery()
    {
        Battery = 1000000f; // 1m
    }
    
    private void ChangeStrings()
    {
        strings[10] = $"                       {MyApprox(speed)}";
        
        strings[3] = $"FPS: {1f / Time.deltaTime}";
        Dbug.text = $"00- {strings[0]}\n01- {strings[1]}\n02- {strings[2]}\n03- {strings[3]}\n04- {strings[4]}\n" +
                    $"05- {strings[5]}\n06- {strings[6]}\n07- {strings[7]}\n08- {strings[8]}\n09- {strings[9]}\n" +
                    $"10- {strings[10]}\n11- {strings[11]}\n12- {strings[12]}\n13- {strings[13]}\n14- {strings[14]}\n" +
                    $"15- {strings[15]}\n16- {strings[16]}\n17- {strings[17]}\n18- {strings[18]}\n19- {strings[19]}\n" +
                    $"20- {strings[20]}\n21- {strings[21]}\n22- {strings[22]}\n23- {strings[23]}\n24- {strings[24]}\n" +
                    $"25- {strings[25]}\n26- {strings[26]}\n27- {strings[27]}\n28- {strings[28]}\n29- {strings[29]}\n";
        strings[2] = $"Vertical: {MyApprox(verticalInput)}; Horizontal: {MyApprox(horizontalInput)}, IsBreaking: {isBreaking};";
        strings[11] = $"PreviousPosition: {ToStringVector3(previousPosition)}";
        strings[12] = $"MyPosition: {ToStringVector3(transform.position)}";
        strings[13] = $"Distance: {ToStringVector3(previousPosition - transform.position)}";
        strings[8] = $"IsGoingForward: {isGoingForward}";
    }
    
    private void Update()
    {
        strings[4] = $"FPS: {MyApprox(1f / Time.deltaTime)}";
        if (Input.GetKeyDown(KeyCode.Delete))
            Start();
    }

    private void GetSpeed(Vector3 position)
    {
        speed = 3.6f * (1f / Time.deltaTime) * (float)Math.Sqrt(
            (previousPosition.x - position.x) * (previousPosition.x - position.x) +
            (previousPosition.y - position.y) * (previousPosition.y - position.y) +
            (previousPosition.z - position.z) * (previousPosition.z - position.z));
        // Cadrant.sprite = Sprites[0];
        if (speed < 1f && speed > 0.5f)
            speedText.text = $"1";
        else
            speedText.text = ((int)speed).ToString();
        var n = (int)speed / (isGoingForward ? 3 : 1);
        Cadrant.sprite = Sprites[n >= 16 ? 15 : n];

        var colorA = StopImage.color.a;
        if (isBreaking)
        {
            if (colorA < 1f)
                StopImage.color = new Color(255, 255, 255, colorA + 5 * Time.deltaTime);
        }
        else
        {
            if (colorA > 0f)
                StopImage.color = new Color(255, 255, 255, colorA - 5 * Time.deltaTime);
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        // isBreaking = Input.GetKey(KeyCode.Space);
        isBreaking = verticalInput < -0.1f && isGoingForward || verticalInput > 0.1f && !isGoingForward;
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
        strings[6] = $"Roue left: {MyApprox(frontLeftWheelCollider.steerAngle > 90 ? 180 - frontLeftWheelCollider.steerAngle : frontLeftWheelCollider.steerAngle)}; Roue Right {MyApprox(frontRightWheelCollider.steerAngle)}";
        strings[9] = $"Roue left: {MyApprox(frontLeftWheelCollider.steerAngle)}; Roue Right {MyApprox(frontRightWheelCollider.steerAngle)}";
    }

    private void HandleMotor() // avancer / reculer
    {
        var b = verticalInput > 0.1f && speed > 45.9f && isGoingForward || verticalInput < 0.1f && speed > 15.5f && !isGoingForward; // si on dépasse 45 en avançant ou 15 en reculant, la voiture s'arrête
        var motorTorque = (b ? 0 : verticalInput * motorForce) - 0.2f * speed * (speed + 10f) * (isGoingForward ? 1 : -1);
        frontLeftWheelCollider.motorTorque = motorTorque * 0.5f;
        frontRightWheelCollider.motorTorque = motorTorque * 0.5f;
        rearLeftWheelCollider.motorTorque = motorTorque;
        rearRightWheelCollider.motorTorque = motorTorque;
        
        if (speed is >= -0.1f and <= 0.5f && verticalInput == 0f)
            isBreaking = true;
        frontLeftWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
        frontRightWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
        rearLeftWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
        rearRightWheelCollider.brakeTorque = isBreaking ? brakeForce : 0f;
        strings[7] = $"frontLeftWheelCollider.motorTorque: {MyApprox(frontLeftWheelCollider.motorTorque)} frontRightWheelCollider.motorTorque: {MyApprox(frontRightWheelCollider.motorTorque)}; brakeForce: {MyApprox(frontLeftWheelCollider.brakeTorque)}";
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
        strings[1] = $"X:{MyApprox(rot.x)}; Y:{MyApprox(rot.y)}; Z:{MyApprox(rot.z)}";
        trans.position = pos;
    }

    private float MyApprox(float x) => (int)(x * 100f) / 100f;
    private double MyTan(double a) => Math.Tan(Math.PI * a / 180d);
    private double MyAtan(double a) => Math.Atan(a) * 180d / Math.PI;
    private double MyAcos(double a) => Math.Acos(a * Math.PI / 180d) * 180d / Math.PI;
    private string ToStringVector3(Vector3 vector1) => $"X: {MyApprox(vector1.x)}; Y: {MyApprox(vector1.y)}; Z: {MyApprox(vector1.z)};";
    private double Norme(Vector3 v) => Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}