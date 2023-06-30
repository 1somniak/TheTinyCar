using System;
using System.Linq;
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

    public int CoverWay(int index)
    {
        return transform.position.x switch
        {
            <= 501.1f => 0,
            <= 504.6f => 1,
            <= 509.1f => 2,
            _ => 3
        };
    }
    
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

    private static int GetColumn(float a) => a switch { < 501.5f => 0, > 510.5f => 2, _ => 1 };

    private float GetPositionX(float x)
    {
        return GetColumn(x) switch
        {
            0 => 501.5f,
            1 => x,
            _ => 510.5f
        };
    } 
    
    private void GetInput()
    {
        verticalInput = 1;
        isBreaking = Input.GetKey(KeyCode.Space);
        horizontalInput = (GetPositionX(target.transform.position.x) - transform.position.x) / 9f; // - transform.rotation.y / 30f;

        var vectorC = target.transform.position - transform.position;
        horizontalInput = (float)MyAtan(vectorC.x / vectorC.z);
//        print(horizontalInput + "  " + transform.rotation.y+ "  vitesse: " + this.gameObject.GetComponent<Rigidbody>().velocity);

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
        steerAngle = maxSteeringAngle * horizontalInput;
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
        Speed = (float)Norme(transform.position - previousPosition) * (1f / Time.deltaTime) * 3.6f;
        previousPosition = transform.position;
        // speedTMP.text = $"F150 Speed: {(int)Speed}     way: {target.GetComponent<CarController>().WhichWay} ";
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsBuilding(other.gameObject.name))
        {
            // Destroy(other.gameObject.GetComponent<Collider>());
            transform.Translate(transform.position.x < 505.5f ? 1 : -1 * Time.deltaTime,0f, 0f);
        }
        
        // if (IsOfType(other, out CarController carController))
        // {
        //     carController.IsDestroyed = true;
        //     Debug.Log("car destroyed");
        // }
    }

    private void OnCollisionStay(Collision other)
    {
        print("hehey");
        if (IsBuilding(other.gameObject.name))
        {
            transform.Translate(transform.position.x < 505.5f ? 1 : -1 * Time.deltaTime, 0f, 0f);
        }
    }

    private static bool IsBuilding(string s)
    {
        // print("collider -" + s + "-");
        return s == "collider";
        // return s.Length >= 4 && s.Substring(0, 4) == "coll";
    }

    public static bool IsOfType<T>(Collision obj, out T k)
    {
        try
        {
            k = obj.gameObject.GetComponent<T>();
            return true;
        }
        catch (Exception)
        {
            k = default;
            return false;
        }
    }
    private float MyApprox(float x) => (int)(x * 100f) / 100f;
    private double MyTan(double a) => Math.Tan(Math.PI * a / 180d);
    private double MyAtan(double a) => Math.Atan(a) * 180d / Math.PI;
    private double MyAcos(double a) => Math.Acos(a * Math.PI / 180d) * 180d / Math.PI;
    private string ToStringVector3(Vector3 vector1) => $"X: {MyApprox(vector1.x)}; Y: {MyApprox(vector1.y)}; Z: {MyApprox(vector1.z)};";
    private double Norme(Vector3 v) => Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}
