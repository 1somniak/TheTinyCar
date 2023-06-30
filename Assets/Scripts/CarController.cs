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
    public bool IsDestroyed { get; set; }
    public JoyScript JoyScript;
    

    private bool IsGoingForward
    {
        get
        {
            var myPosition = transform.position;
            var CPosition = cube.transform.position;
            var AB = Norme(myPosition - previousPosition); // vecteur qui bouge tout le temps
            var AC = Norme(myPosition - CPosition); // vecteur fixe
            var BC = Norme(previousPosition - CPosition); // hypotenuse
            strings[16] = $"AB: {MyApprox((float)AB)}; AC: {MyApprox((float)AC)}; BC: {MyApprox((float)BC)};";
            return BC * BC >= AB * AB + AC * AC;
        }
    }
    public bool isGoingForward;

    public int WhichWay
    {
        get
        {
            return transform.position.x switch
            {
                <= 501.1f => 0,
                <= 505.1f => 1,
                <= 510.1f => 2,
                _         => 3
            };
        }
    }

    public Vector3 previousPosition;
    public float Speed { get; private set; }
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
    public GameObject carVisual;

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
        transform.rotation = Quaternion.Euler(0, 0f, 0);
        previousPosition = transform.position;
        ChargeBattery();
        IsDestroyed = false;
    }

    private void FixedUpdate()
    {
        isGoingForward = IsGoingForward;
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        var position = transform.position;
        GetSpeed(position);
        ChangeStrings();
        previousPosition = position;
        DestroyVehicle();
    }

    private void ChargeBattery()
    {
        Battery = 1000000f; // 1 Million
    }
    
    private void ChangeStrings()
    {
        strings[10] = $"                       {MyApprox(Speed)}";
        
        strings[3] = $"FPS: {1f / Time.deltaTime}";
        // Dbug.text = $"00- {strings[0]}\n01- {strings[1]}\n02- {strings[2]}\n03- {strings[3]}\n04- {strings[4]}\n" +
        //             $"05- {strings[5]}\n06- {strings[6]}\n07- {strings[7]}\n08- {strings[8]}\n09- {strings[9]}\n" +
        //             $"10- {strings[10]}\n11- {strings[11]}\n12- {strings[12]}\n13- {strings[13]}\n14- {strings[14]}\n" +
        //             $"15- {strings[15]}\n16- {strings[16]}\n17- {strings[17]}\n18- {strings[18]}\n19- {strings[19]}\n" +
        //             $"20- {strings[20]}\n21- {strings[21]}\n22- {strings[22]}\n23- {strings[23]}\n24- {strings[24]}\n" +
        //             $"25- {strings[25]}\n26- {strings[26]}\n27- {strings[27]}\n28- {strings[28]}\n29- {strings[29]}\n";
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
        Speed = 3.6f * (1f / Time.deltaTime) * (float)Norme(previousPosition - position);
        // Cadrant.sprite = Sprites[0];
        if (Speed < 1f && Speed > 0.5f)
            speedText.text = $"1";
        else
            speedText.text = ((int)Speed).ToString();
        var n = (int)Speed / (isGoingForward ? 3 : 1);
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
        horizontalInput = Input.GetAxis("Horizontal") + JoyScript.Horizontal;
        if (horizontalInput > 1)
            horizontalInput = 1;
        if (horizontalInput < -1)
            horizontalInput = -1;
        verticalInput = Input.GetAxis("Vertical") + JoyScript.Vertical;
        if (verticalInput > 1)
            verticalInput = 1;
        if (verticalInput < -1)
            verticalInput = -1;
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
        var b = verticalInput > 0.1f && Speed > 45.9f && isGoingForward || verticalInput < 0.1f && Speed > 15.5f && !isGoingForward; // si on dépasse 45 en avançant ou 15 en reculant, la voiture s'arrête
        var motorTorque = (b ? 0 : verticalInput * motorForce) - 0.2f * Speed * (Speed + 10f) * (isGoingForward ? 1 : -1);
        frontLeftWheelCollider.motorTorque = motorTorque * 0.5f;
        frontRightWheelCollider.motorTorque = motorTorque * 0.5f;
        rearLeftWheelCollider.motorTorque = motorTorque;
        rearRightWheelCollider.motorTorque = motorTorque;
        
        if (Speed is >= -0.1f and <= 0.5f && verticalInput == 0f)
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
        wheelCollider.GetWorldPose(out var pos, out var rot);
        trans.rotation = rot;
        strings[1] = $"X:{MyApprox(rot.x)}; Y:{MyApprox(rot.y)}; Z:{MyApprox(rot.z)}";
        trans.position = pos;
    }

    private void DestroyVehicle()
    {
        if (!IsDestroyed) return;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
        foreach (var boxCollider in this.GetComponents<BoxCollider>())
            boxCollider.enabled = false;
        carVisual.transform.Find("Cube0").gameObject.GetComponent<BoxCollider>().enabled = false;
        carVisual.transform.Find("Cube0").gameObject.GetComponent<BoxCollider>().enabled = false;
        carVisual.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        isBreaking = true;
        var localScale = carVisual.transform.localScale;
        if (localScale.y > 0.01f)
            carVisual.transform.localScale = new Vector3(localScale.x, 
                                             localScale.y - 2 * Time.deltaTime,
                                               localScale.z);
        localScale = carVisual.transform.localScale;
        if (localScale.y <= 0f)
            carVisual.transform.localScale = new Vector3(1, 0.01f, 1);
    }


    private static float MyApprox(float x) => (int)(x * 100f) / 100f;
    private static double MyTan(double a) => Math.Tan(Math.PI * a / 180d);
    private static double MyAtan(double a) => Math.Atan(a) * 180d / Math.PI;
    private static double MyAcos(double a) => Math.Acos(a * Math.PI / 180d) * 180d / Math.PI;
    private static string ToStringVector3(Vector3 vector1) => $"X: {MyApprox(vector1.x)}; Y: {MyApprox(vector1.y)}; Z: {MyApprox(vector1.z)};";
    private static double Norme(Vector3 v) => Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}