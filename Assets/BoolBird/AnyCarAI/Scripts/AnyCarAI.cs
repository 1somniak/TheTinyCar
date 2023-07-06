using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region CUSTOM DRIVING

[Serializable]
public enum CarDriveTypeAI
{
    FrontWheelDrive,
    RearWheelDrive,
    FourWheelDrive
}

[Serializable]
public enum SpeedTypeAI
{
    MPH,
    KPH
}

#endregion

#region WHEELS SETUP

public enum AxlAI
{
    Front,
    Rear
}
public enum TypeAI
{
    Drive,
    Trailer
}

[Serializable]
public struct CarWheelsAI
{
    public GameObject model;
    public AxlAI axel;
    public TypeAI type;
}

[Serializable]
public struct CarWheelsColsAI
{
    public GameObject collider;
    public AxlAI axel;
    public TypeAI type;
}

#endregion

#region COLLISION

[Serializable]
public struct OptionalMeshesAI
{
    public MeshFilter modelMesh;
    public int loseAftCollisions;
}

#endregion

#region AI ENUMS
public enum BrakeCondition
{
    NeverBrake,
    TargetDirectionDifference,
    TargetDistance,
}

public enum ProgressStyle
{
    SmoothAlongRoute,
    PointToPoint,
}

#endregion

public class AnyCarAI : MonoBehaviour
{
    #region EDITOR

    public int toolbarTab;
    public string currentTab;

    #endregion
    
    #region WHEELS REFERENCES

    public List<CarWheelsAI> extraWheels;
    public List<CarWheelsColsAI> extraWheelsColList = new List<CarWheelsColsAI>();
    public CarWheelsColsAI extraWheelCol;
    public GameObject frontLeft;
    public GameObject frontRight;
    public GameObject backLeft;
    public GameObject backRight;
    public GameObject frontLeftCol;
    public GameObject frontRightCol;
    public GameObject backLeftCol;
    public GameObject backRightCol;

    public float extraWheelRadius;
    private float FLRadius;
    private float FRRadius;
    private float BLRadius;
    private float BRRadius;

    #region WHEELS VALUES

    public float wheelsMass = 20;
    public float forcePoint = 0;
    public float dumpingRate = 0.025f;
    public float suspensionDistance = 0.2f;
    public Vector3 wheelsPosition;
    public Vector3 wheelsRotation;
    [Range(0.1f, 1f)] public float wheelStiffness = 1f;
    public float suspensionSpring = 70000f;
    public float suspensionDamper = 3500f;
    [Range(0.1f, 1)] public float targetPosition = 0.5f;

    [Range(0.5f, 2)] public float wheelsRadius = 1f;

    #endregion

    #endregion

    #region CAR REFERENCES

    public GameObject bodyMesh;
    public GameObject extraBodyCol;

    #endregion

    #region INPUT REFERENCES

    public float AccelInput { get; private set; }
    public float BrakeInput { get; private set; }

    #endregion

    #region CUSTOM CONTROLS

    public AnimationCurve enginePower;

    public float maximumSteerAngle;
    [Range(0, 1)] public float steerHelper; // 0 = raw physics , 1 grip in the facing direction
    [Range(0, 0.5f)] public float tractionControl;
    [Range(0, 0.5f)] public float slipLimit = 0.3f;

    [SerializeField] public CarDriveTypeAI carDriveType = CarDriveTypeAI.FourWheelDrive;
    [SerializeField] public SpeedTypeAI speedType = SpeedTypeAI.KPH;

    public Vector3 centerOfMass;
    public float vehicleMass = 1000f;

    public float motorTorque = 2500f;
    public float brakeTorque = 20000f;
    public float reverseTorque = 500f;
    public float handbrakeTorque = 10000000f;
    public float maxSpeed = 200f;

    public int numberOfGears = 5;
    public float downForce = 300f;

    public bool ABS = true;
    public bool skidMarks = true;

    public bool smokeOn = false;
    private ParticleSystem smokeParticles;

    #endregion

    #region TURBO

    public bool turboON = false;
    public AudioClip turboAudioClip;
    [Range(0, 1f)] public float turboVolume = 0.5f;

    #endregion

    #region EXHAUST

    public Transform exhaustObjectPrefab;
    public Transform exhaustObject;
    public GameObject exhaustObj;
    public bool exhaustFlame;
    public ParticleSystem exhaustVisual;
    public AudioSource exhaustSoundSource;
    public AudioClip exhaustSound;
    [Range(0.01f, 1)] public float exhaustVolume;

    #endregion

    #region UTILITY

    private float oldRotation;
    public Rigidbody rb
    {
        get;
        set;
    }

    public float currentSpeed;
    private float currentTorque;
    private float rpmRange = 1f;
    public int currentGear;
    private float gearFactor;
    public bool reverseGearOn;
    public float RPM { get; private set; }

    public GameObject objToUnpack;

    public GameObject frontLights;
    public GameObject rearLights;

    #endregion

    #region COLLISION SYSTEM

    public bool collisionSystem = false;
    public AudioClip collisionSound;
    [Range(0.01f, 1f)] public float collisionVolume;

    public OptionalMeshesAI[] optionalMeshList;

    [Range(0.01f, 50)] public float demolutionStrenght;
    [Range(0.1f, 500)] public float demolutionRange;
    public bool customMesh = false;
    public bool collisionParticles = false;

    #endregion

    #region AUDIO REFERENCES

    public AudioClip skidSound;
    [Range(0.01f, 1)] public float skidVolume = 0.3f;

    public AudioClip lowAcceleration;
    public AudioClip lowDeceleration;
    public AudioClip highAcceleration;
    public AudioClip highDeceleration;
    [Range(0.01f, 1)] public float engineVolume;



    #endregion

    #region SUSPENSIONS SOUND

    public AudioSource suspensionsSource;
    public AudioSource skidSource;
    public AudioClip suspensionsSound;
    [Range(0, 1)] public float suspensionsVolume;

    #endregion

    #region CREATE COLLIDERS

    public void UnpackPrefab()
    {
        Transform _parent = this.transform.parent;
        objToUnpack = _parent.gameObject;
    }

    public void CreateColliders()
    {
        // Create and Set Wheel Colliders
        frontLeft.AddComponent(typeof(SphereCollider));
        frontRight.AddComponent(typeof(SphereCollider));
        backLeft.AddComponent(typeof(SphereCollider));
        backRight.AddComponent(typeof(SphereCollider));

        // Create Wheel Colliders Objects
        frontLeftCol = new GameObject("FLCOL");
        frontRightCol = new GameObject("FRCOL");
        backLeftCol = new GameObject("BLCOL");
        backRightCol = new GameObject("BRCOL");


        // Front Left Wheel
        frontLeftCol.transform.parent = this.transform.GetChild(0);
        frontLeftCol.transform.position = frontLeft.transform.position;
        frontLeftCol.transform.rotation = frontLeft.transform.rotation;

        // Front Right Wheel
        frontRightCol.transform.parent = this.transform.GetChild(0);
        frontRightCol.transform.position = frontRight.transform.position;
        frontRightCol.transform.rotation = frontRight.transform.rotation;

        // Back Left Wheel
        backLeftCol.transform.parent = this.transform.GetChild(0);
        backLeftCol.transform.position = backLeft.transform.position;
        backLeftCol.transform.rotation = backLeft.transform.rotation;

        // Back Right Wheel
        backRightCol.transform.parent = this.transform.GetChild(0);
        backRightCol.transform.position = backRight.transform.position;
        backRightCol.transform.rotation = backRight.transform.rotation;

        // Add Wheel Colliders
        frontLeftCol.AddComponent(typeof(WheelCollider));
        frontRightCol.AddComponent(typeof(WheelCollider));
        backLeftCol.AddComponent(typeof(WheelCollider));
        backRightCol.AddComponent(typeof(WheelCollider));



        // Add Skid Marks
        frontLeftCol.AddComponent(typeof(WheelsFXAI));
        frontRightCol.AddComponent(typeof(WheelsFXAI));
        backLeftCol.AddComponent(typeof(WheelsFXAI));
        backRightCol.AddComponent(typeof(WheelsFXAI));

        // Get Wheel Radius
        FLRadius = frontLeft.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;
        FRRadius = frontRight.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;
        BLRadius = backLeft.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;
        BRRadius = backRight.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;

        // Set Wheel Radius
        frontLeftCol.GetComponent<WheelCollider>().radius = Mathf.Abs(FLRadius);
        frontRightCol.GetComponent<WheelCollider>().radius = Mathf.Abs(FRRadius);
        backLeftCol.GetComponent<WheelCollider>().radius = Mathf.Abs(BLRadius);
        backRightCol.GetComponent<WheelCollider>().radius = Mathf.Abs(BRRadius);

        // Destroy Sphere Colliders
        DestroyImmediate(frontLeft.GetComponent<SphereCollider>());
        DestroyImmediate(frontRight.GetComponent<SphereCollider>());
        DestroyImmediate(backLeft.GetComponent<SphereCollider>());
        DestroyImmediate(backRight.GetComponent<SphereCollider>());

        // Create Body Convex Mesh Collider
        if (bodyMesh != null)
        {
            bodyMesh.AddComponent(typeof(MeshCollider));
            bodyMesh.AddComponent(typeof(CompetitiveDrivingCheck));
            bodyMesh.GetComponent<MeshCollider>().convex = true;
        }


        // Extra Wheels
        extraWheelsColList.Clear();
        int c = 1;
        foreach (var wheel in extraWheels)
        {
            wheel.model.AddComponent(typeof(SphereCollider));

            extraWheelCol.collider = new GameObject("extraWheel " + c);
            extraWheelCol.axel = wheel.axel;
            extraWheelCol.type = wheel.type;

            extraWheelCol.collider.transform.parent = this.transform.GetChild(0);
            extraWheelCol.collider.transform.position = wheel.model.transform.position;
            extraWheelCol.collider.transform.rotation = wheel.model.transform.rotation;


            extraWheelCol.collider.AddComponent(typeof(WheelCollider));
            extraWheelCol.collider.AddComponent(typeof(WheelsFXAI));

            extraWheelRadius = wheel.model.GetComponent<SphereCollider>().radius * wheel.model.transform.lossyScale.x;
            extraWheelCol.collider.GetComponent<WheelCollider>().radius = Mathf.Abs(extraWheelRadius);

            DestroyImmediate(wheel.model.GetComponent<SphereCollider>());

            extraWheelsColList.Add(extraWheelCol);

            c++;
        }
    }

    public void CreateDebugBodyCol()
    {
        extraBodyCol = Instantiate(Resources.Load("BodyCollider"), this.transform.position, Quaternion.identity) as GameObject;
        extraBodyCol.transform.parent = this.transform;
        extraBodyCol.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        extraBodyCol.transform.rotation = this.transform.rotation;
        bodyMesh = extraBodyCol;
        bodyMesh.AddComponent(typeof(CompetitiveDrivingCheck));
    }

    #endregion

    #region AI CONTROLLER

    public CarAIInputs carAIInputs;
    public CarAIWaipointTracker carAIWaypointTracker;

    #region INPUTS

    [SerializeField] public BrakeCondition brakeCondition = BrakeCondition.TargetDistance;

    [SerializeField] [Range(0, 1)] public float cautiousSpeedFactor = 0.05f;
    [SerializeField] [Range(0, 180)] public float cautiousAngle = 50f;
    [SerializeField] [Range(0, 200)] public float cautiousDistance = 100f;
    [SerializeField] public float cautiousAngularVelocityFactor = 30f;
    [SerializeField] [Range(0, 0.1f)] public float steerSensitivity = 0.05f;
    [SerializeField] [Range(0, 0.1f)] public float accelSensitivity = 0.04f;
    [SerializeField] [Range(0, 1)] public float brakeSensitivity = 1f;
    [SerializeField] [Range(0, 10)] public float lateralWander = 3f;
    [SerializeField] public float lateralWanderSpeed = 0.5f;
    [SerializeField] [Range(0, 1)] public float wanderAmount = 0.1f;
    [SerializeField] public float accelWanderSpeed = 0.1f;

    [SerializeField] public bool isDriving;
    [SerializeField] public Transform carAItarget;
    private GameObject carAItargetObj;
    [SerializeField] public bool stopWhenTargetReached; 
    [SerializeField] public float reachTargetThreshold = 2;

    #region SENSORS

    [SerializeField] [Range(15, 50)] public float sensorsAngle;
    [SerializeField] public float avoidDistance = 10;
    [SerializeField] public float brakeDistance = 6;
    [SerializeField] public float reverseDistance = 3;

    #endregion

    #endregion

    #region PERSUIT AI

    public bool persuitAiOn;
    public GameObject persuitTarget;
    public float persuitDistance;

    #endregion

    #region PROGRESS TRACKER

    [SerializeField] public ProgressStyle progressStyle = ProgressStyle.SmoothAlongRoute;
    [SerializeField] public WaypointsPath AIcircuit;
    [SerializeField] [Range(5,50)]public float lookAheadForTarget = 5;
    [SerializeField] public float lookAheadForTargetFactor = .1f;
    [SerializeField] public float lookAheadForSpeedOffset = 10;
    [SerializeField] public float lookAheadForSpeedFactor = .2f;
    [SerializeField] [Range(1, 10)] public float pointThreshold = 4;
    public Transform AItarget;

    #endregion

    #endregion

    void Start()
    {
        #region INITIALIZE COMPONENTS

        rb = GetComponent<Rigidbody>();
        rb.mass = vehicleMass;
        centerOfMass = rb.centerOfMass;
        currentTorque = motorTorque - (tractionControl * motorTorque);


        #region LIGHTS INITIALIZATION

        rearLights = transform.GetChild(1).GetChild(2).gameObject;
        rearLights.SetActive(false);

        #endregion

        #region SMOKE

        smokeParticles = transform.root.GetComponentInChildren<ParticleSystem>();

        if (!smokeOn)
        {
            smokeParticles.Stop();
        }
        else
        {
            smokeParticles.Play();
        }

        #endregion

        #region FEATURES SCRIPTS

        carAIInputs = gameObject.AddComponent<CarAIInputs>();
        carAIWaypointTracker = gameObject.AddComponent<CarAIWaipointTracker>();
        EngineAudioAI audioScript = gameObject.AddComponent<EngineAudioAI>();
        DamageSystemAI damageScript = gameObject.AddComponent<DamageSystemAI>();

        #endregion

        #region AI REFERENCES

        carAItargetObj = new GameObject("WaypointsTarget");
        carAItargetObj.transform.parent = this.transform.GetChild(1);
        carAItarget = carAItargetObj.transform;

        #endregion

        #region EXHAUST

        if (exhaustFlame)
        {
            if (exhaustObj == null)
            {
                exhaustObj = transform.GetChild(1).Find("ExhaustPipe(Clone)").gameObject;

                exhaustVisual = exhaustObj.GetComponent<ParticleSystem>();

                exhaustSoundSource = exhaustObj.GetComponent<AudioSource>();
                exhaustSoundSource.clip = exhaustSound;
                exhaustSoundSource.volume = exhaustVolume;
            }
        }

        #endregion

        SetWheelsValues();

        #endregion
    }

    private void Update()
    {
        #region CURRENT SPEED

        switch (speedType)
        {
            case SpeedTypeAI.MPH:

                currentSpeed = rb.velocity.magnitude * 2.23693629f;

                break;
            case SpeedTypeAI.KPH:

                currentSpeed = rb.velocity.magnitude * 3.6f;

                break;
        }

        #endregion

        isDrivingDebug();
    }

    public void Move(float steering, float Accel, float footbrake, float handbrake)
    {
        AnimateWheels();

        #region GETINPUTS

        //clamp input values
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = Accel = Mathf.Clamp(Accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        #endregion

        #region GEARS

        CalculateRPM();

        #region TRANSMISSION

        AutoGearSystem();

        #endregion

        #endregion

        #region DRIVING

        Steering(steering);

        SteerHelper();

        ApplyDrive(Accel, footbrake);

        MaxSpeedReached();

        HandBreaking(handbrake);

        #endregion

        #region AERODYNAMICS

        AddDownForce();

        if (skidMarks == true)
        {
            CheckForWheelSpin();
        }

        TractionControl();

        #endregion       
    }

    #region DRIVING

    private void AnimateWheels()
    {
        Quaternion FLRot;
        Vector3 FLPos;
        Quaternion FRRot;
        Vector3 FRPos;
        Quaternion BLRot;
        Vector3 BLPos;
        Quaternion BRRot;
        Vector3 BRPos;

        // Front Left
        frontLeftCol.GetComponent<WheelCollider>().GetWorldPose(out FLPos, out FLRot);
        FLRot = FLRot * Quaternion.Euler(wheelsRotation);
        frontLeft.transform.position = FLPos;
        frontLeft.transform.rotation = FLRot;

        // Front Right
        frontRightCol.GetComponent<WheelCollider>().GetWorldPose(out FRPos, out FRRot);
        FRRot = FRRot * Quaternion.Euler(wheelsRotation);
        frontRight.transform.position = FRPos;
        frontRight.transform.rotation = FRRot;

        // Back Left
        backLeftCol.GetComponent<WheelCollider>().GetWorldPose(out BLPos, out BLRot);
        BLRot = BLRot * Quaternion.Euler(wheelsRotation);
        backLeft.transform.position = BLPos;
        backLeft.transform.rotation = BLRot;

        // Back Right
        backRightCol.GetComponent<WheelCollider>().GetWorldPose(out BRPos, out BRRot);
        BRRot = BRRot * Quaternion.Euler(wheelsRotation);
        backRight.transform.position = BRPos;
        backRight.transform.rotation = BRRot;

        // Extra Wheels
        int i = 0;
        foreach (var wheelCol in extraWheelsColList)
        {
            Quaternion _rot;
            Vector3 _pos;
            wheelCol.collider.GetComponent<WheelCollider>().GetWorldPose(out _pos, out _rot);
            _rot = _rot * Quaternion.Euler(wheelsRotation);
            extraWheels[i].model.transform.position = _pos;
            extraWheels[i].model.transform.rotation = _rot;

            i++;
        }

    }
    private void Steering(float steering)
    {
        var steerAngle = steering * maximumSteerAngle;
        frontLeftCol.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(frontLeftCol.GetComponent<WheelCollider>().steerAngle, steerAngle, 0.5f);
        frontRightCol.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(frontRightCol.GetComponent<WheelCollider>().steerAngle, steerAngle, 0.5f);

        foreach (var wheelCol in extraWheelsColList)
        {
            if (wheelCol.axel == AxlAI.Front)
            {
                wheelCol.collider.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(wheelCol.collider.GetComponent<WheelCollider>().steerAngle, steerAngle, 0.5f);
            }
        }
    }

    private void SteerHelper()
    {
        // wheels don't touch ground
        WheelHit wHFL;
        WheelHit wHFR;
        WheelHit wHBL;
        WheelHit wHBR;

        frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wHFL);
        frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out wHFR);
        backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wHBL);
        backRightCol.GetComponent<WheelCollider>().GetGroundHit(out wHBR);

        if (wHFL.normal == Vector3.zero)
            return;
        if (wHFR.normal == Vector3.zero)
            return;
        if (wHBL.normal == Vector3.zero)
            return;
        if (wHBR.normal == Vector3.zero)
            return;

        // Extra Wheels
        int i = 0;
        foreach (var wheelCol in extraWheelsColList)
        {
            WheelHit wheelhit;
            wheelCol.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
            i++;
        }

        // Shift direction debug
        if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - oldRotation) * steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rb.velocity = velRotation * rb.velocity;
        }
        oldRotation = transform.eulerAngles.y;
    }

    private void ApplyDrive(float Accel, float footbrake)
    {
        float thrustTorque;

        switch (carDriveType)
        {
            case CarDriveTypeAI.FourWheelDrive:

                thrustTorque = Accel * (currentTorque / 4f) * enginePower.Evaluate(1);

                frontLeftCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                frontRightCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                backLeftCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                backRightCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;

                foreach (var wheelCol in extraWheelsColList)
                {
                    if (wheelCol.type == TypeAI.Drive)
                    {
                        wheelCol.collider.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                    }
                }

                break;

            case CarDriveTypeAI.FrontWheelDrive:

                thrustTorque = Accel * (currentTorque / 2f) * enginePower.Evaluate(1);

                frontLeftCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                frontRightCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;

                foreach (var wheelCol in extraWheelsColList)
                {
                    if (wheelCol.type == TypeAI.Drive)
                    {
                        wheelCol.collider.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                    }
                }

                break;

            case CarDriveTypeAI.RearWheelDrive:

                thrustTorque = Accel * (currentTorque / 2f) * enginePower.Evaluate(1);

                backLeftCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                backRightCol.GetComponent<WheelCollider>().motorTorque = thrustTorque;

                foreach (var wheelCol in extraWheelsColList)
                {
                    if (wheelCol.type == TypeAI.Drive)
                    {
                        wheelCol.collider.GetComponent<WheelCollider>().motorTorque = thrustTorque;
                    }
                }

                break;

        }

        #region FOOTBRAKE

        if (footbrake > 0)
        {
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rb.velocity) < 50f)
            {
                reverseGearOn = false;
            }
            else
            {
                reverseGearOn = true;
            }
        }
        else
        {
            reverseGearOn = false;
        }

        if (!ABS)
        {
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rb.velocity) < 50f)
            {
                frontLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
                frontRightCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
                backLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
                backRightCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;

                foreach (var wheelCol in extraWheelsColList)
                {
                    wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
                }

                if (footbrake > 0)
                {
                    rearLights.SetActive(true);
                }
                else
                {
                    rearLights.SetActive(false);
                }
            }
            else if (footbrake > 0)
            {
                rearLights.SetActive(false);

                frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                backRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                frontLeftCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                frontRightCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                backLeftCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                backRightCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;

                foreach (var wheelCol in extraWheelsColList)
                {
                    wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = 0f;
                    wheelCol.collider.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                }
            }
        }
        else
        {
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rb.velocity) < 50f)
            {
                StartCoroutine(ABSCoroutine(footbrake));

                if (footbrake > 0)
                {
                    rearLights.SetActive(true);
                }
                else
                {
                    rearLights.SetActive(false);
                }
            }
            else if (footbrake > 0)
            {
                rearLights.SetActive(false);

                frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                backRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
                frontLeftCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                frontRightCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                backLeftCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                backRightCol.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;

                foreach (var wheelCol in extraWheelsColList)
                {
                    wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = 0f;
                    wheelCol.collider.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                }
            }
        }

        #endregion

    }

    private void MaxSpeedReached()
    {
        switch (speedType)
        {
            case SpeedTypeAI.MPH:

                if (currentSpeed > maxSpeed)
                    rb.velocity = (maxSpeed / 2.23693629f) * rb.velocity.normalized;
                break;

            case SpeedTypeAI.KPH:

                if (currentSpeed > maxSpeed)
                    rb.velocity = (maxSpeed / 3.6f) * rb.velocity.normalized;
                break;        
        }
    }

    private void HandBreaking(float handbrake)
    {
        if (handbrake > 0f)
        {
            var hbTorque = handbrake * handbrakeTorque;
            backLeftCol.GetComponent<WheelCollider>().brakeTorque = hbTorque;
            backRightCol.GetComponent<WheelCollider>().brakeTorque = hbTorque;
        }
    }

    IEnumerator ABSCoroutine(float footbrake)
    {
        frontLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
        frontRightCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
        backLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
        backRightCol.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;

        foreach (var wheelCol in extraWheelsColList)
        {
            wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
        }

        yield return new WaitForSeconds(0.1f);

        frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
        frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0;
        backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
        backRightCol.GetComponent<WheelCollider>().brakeTorque = 0;

        foreach (var wheelCol in extraWheelsColList)
        {
            wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = 0;
        }

        yield return new WaitForSeconds(0.1f);
    }

    private void isDrivingDebug()
    {
        if (rb.velocity.magnitude < 1)
        {
            if (isDriving)
            {
                if (carAIInputs.reverseGearOn)
                {
                    rb.AddRelativeForce(0, 0, -vehicleMass * 1000 * Time.deltaTime);
                }
                else
                {
                    rb.AddRelativeForce(0, 0, vehicleMass * 1000 * Time.deltaTime);
                }                
            }
        }
    }

    #endregion

    #region WHEELS VALUES

    private void SetWheelsValues()
    {
        frontLeftCol.GetComponent<WheelCollider>().mass = wheelsMass;
        frontRightCol.GetComponent<WheelCollider>().mass = wheelsMass;
        backLeftCol.GetComponent<WheelCollider>().mass = wheelsMass;
        backRightCol.GetComponent<WheelCollider>().mass = wheelsMass;

        frontLeftCol.GetComponent<WheelCollider>().radius *= wheelsRadius;
        frontRightCol.GetComponent<WheelCollider>().radius *= wheelsRadius;
        backLeftCol.GetComponent<WheelCollider>().radius *= wheelsRadius;
        backRightCol.GetComponent<WheelCollider>().radius *= wheelsRadius;

        frontLeftCol.GetComponent<WheelCollider>().forceAppPointDistance = forcePoint;
        frontRightCol.GetComponent<WheelCollider>().forceAppPointDistance = forcePoint;
        backLeftCol.GetComponent<WheelCollider>().forceAppPointDistance = forcePoint;
        backRightCol.GetComponent<WheelCollider>().forceAppPointDistance = forcePoint;

        frontLeftCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
        frontRightCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
        backLeftCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
        backRightCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;

        frontLeftCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
        frontRightCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
        backLeftCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
        backRightCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;

        frontLeftCol.GetComponent<WheelCollider>().center = wheelsPosition;
        frontRightCol.GetComponent<WheelCollider>().center = wheelsPosition;
        backLeftCol.GetComponent<WheelCollider>().center = wheelsPosition;
        backRightCol.GetComponent<WheelCollider>().center = wheelsPosition;

        var leftCol = frontLeftCol.GetComponent<WheelCollider>().suspensionSpring;
        var rightCol = frontRightCol.GetComponent<WheelCollider>().suspensionSpring;
        var bLeft = backLeftCol.GetComponent<WheelCollider>().suspensionSpring;
        var bRight = backRightCol.GetComponent<WheelCollider>().suspensionSpring;
        leftCol.spring = suspensionSpring;
        rightCol.spring = suspensionSpring;
        bLeft.spring = suspensionSpring;
        bRight.spring = suspensionSpring;
        leftCol.damper = suspensionDamper;
        rightCol.damper = suspensionDamper;
        bLeft.damper = suspensionDamper;
        bRight.damper = suspensionDamper;
        leftCol.targetPosition = targetPosition;
        rightCol.targetPosition = targetPosition;
        bLeft.targetPosition = targetPosition;
        bRight.targetPosition = targetPosition;
        frontLeftCol.GetComponent<WheelCollider>().suspensionSpring = leftCol;
        frontRightCol.GetComponent<WheelCollider>().suspensionSpring = rightCol;
        backLeftCol.GetComponent<WheelCollider>().suspensionSpring = bLeft;
        backRightCol.GetComponent<WheelCollider>().suspensionSpring = bRight;

        var fl = frontLeftCol.GetComponent<WheelCollider>().sidewaysFriction;
        var fr = frontRightCol.GetComponent<WheelCollider>().sidewaysFriction;
        var bl = backLeftCol.GetComponent<WheelCollider>().sidewaysFriction;
        var br = backRightCol.GetComponent<WheelCollider>().sidewaysFriction;
        fl.stiffness = wheelStiffness;
        fr.stiffness = wheelStiffness;
        bl.stiffness = wheelStiffness;
        br.stiffness = wheelStiffness;
        frontLeftCol.GetComponent<WheelCollider>().sidewaysFriction = fl;
        frontRightCol.GetComponent<WheelCollider>().sidewaysFriction = fr;
        backLeftCol.GetComponent<WheelCollider>().sidewaysFriction = bl;
        backRightCol.GetComponent<WheelCollider>().sidewaysFriction = br;


        var flf = frontLeftCol.GetComponent<WheelCollider>().forwardFriction;
        var frf = frontRightCol.GetComponent<WheelCollider>().forwardFriction;
        var blf = backLeftCol.GetComponent<WheelCollider>().forwardFriction;
        var brf = backRightCol.GetComponent<WheelCollider>().forwardFriction;
        flf.stiffness = wheelStiffness;
        frf.stiffness = wheelStiffness;
        blf.stiffness = wheelStiffness;
        brf.stiffness = wheelStiffness;
        frontLeftCol.GetComponent<WheelCollider>().forwardFriction = flf;
        frontRightCol.GetComponent<WheelCollider>().forwardFriction = frf;
        backLeftCol.GetComponent<WheelCollider>().forwardFriction = blf;
        backRightCol.GetComponent<WheelCollider>().forwardFriction = brf;

        foreach (var wheelCol in extraWheelsColList)
        {
            wheelCol.collider.GetComponent<WheelCollider>().mass = wheelsMass;
            wheelCol.collider.GetComponent<WheelCollider>().radius = extraWheelRadius * wheelsRadius;
            wheelCol.collider.GetComponent<WheelCollider>().forceAppPointDistance = forcePoint;
            wheelCol.collider.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
            wheelCol.collider.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
            wheelCol.collider.GetComponent<WheelCollider>().center = wheelsPosition;

            var susp = wheelCol.collider.GetComponent<WheelCollider>().suspensionSpring;
            susp.spring = suspensionSpring;
            susp.damper = suspensionDamper;
            susp.targetPosition = targetPosition;
            wheelCol.collider.GetComponent<WheelCollider>().suspensionSpring = susp;

            var stifn = wheelCol.collider.GetComponent<WheelCollider>().sidewaysFriction;
            stifn.stiffness = wheelStiffness;
            wheelCol.collider.GetComponent<WheelCollider>().sidewaysFriction = stifn;
        }
    }

    #endregion

    #region GEAR SYSTEM
    private void AutoGearSystem()
    {
        float gearRatio = Mathf.Abs(currentSpeed / maxSpeed);

        float gearUp = (1 / (float)numberOfGears) * (currentGear + 1);
        float gearDown = (1 / (float)numberOfGears) * currentGear;

        if (currentGear > 0 && gearRatio < gearDown)
        {
            currentGear--;
        }

        if (gearRatio > gearUp && (currentGear < (numberOfGears - 1)))
        {
            if (!reverseGearOn)
            {
                if (exhaustFlame)
                {
                    ExhaustFX();
                }
                currentGear++;
            }
        }
    }    

    // Curved Bias towards 1 for a value between 0-1
    private static float BiasCurve(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }


    // Smooth Lerp with no fixed Boundaries
    private static float SmoothLerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }


    private void CalculateGearFactor()
    {
        // Smooth Gear Changing
        float f = (1 / (float)numberOfGears);

        var targetGearFactor = Mathf.InverseLerp(f * currentGear, f * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));
        gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime * 5f);
    }


    private void CalculateRPM()
    {
        // Calculate engine RPM
        CalculateGearFactor();
        float gearNumFactor;

        gearNumFactor = (currentGear / (float)numberOfGears);

        var minRPM = SmoothLerp(0f, rpmRange, BiasCurve(gearNumFactor));
        var maxRPM = SmoothLerp(rpmRange, 1f, gearNumFactor);

        RPM = SmoothLerp(minRPM, maxRPM, gearFactor);
    }

    #endregion

    #region AERODYNAMICS
    private void AddDownForce()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
    }

    private void TractionControl()
    {
        WheelHit wheelHit;
        WheelHit flHit;
        WheelHit frHit;
        WheelHit blHit;
        WheelHit brHit;

        switch (carDriveType)
        {
            case CarDriveTypeAI.FourWheelDrive:

                frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out flHit);
                AdjustTorque(flHit.forwardSlip);

                frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out frHit);
                AdjustTorque(frHit.forwardSlip);

                backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out blHit);
                AdjustTorque(blHit.forwardSlip);

                backRightCol.GetComponent<WheelCollider>().GetGroundHit(out brHit);
                AdjustTorque(brHit.forwardSlip);

                foreach (var wheelCol in extraWheelsColList)
                {
                    wheelCol.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                }

                break;

            case CarDriveTypeAI.RearWheelDrive:

                backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out blHit);
                AdjustTorque(blHit.forwardSlip);

                backRightCol.GetComponent<WheelCollider>().GetGroundHit(out brHit);
                AdjustTorque(brHit.forwardSlip);

                foreach (var wheelCol in extraWheelsColList)
                {
                    if (wheelCol.axel == AxlAI.Rear)
                    {
                        wheelCol.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
                        AdjustTorque(wheelHit.forwardSlip);
                    }
                }

                break;

            case CarDriveTypeAI.FrontWheelDrive:

                frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out flHit);
                AdjustTorque(flHit.forwardSlip);

                frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out frHit);
                AdjustTorque(frHit.forwardSlip);

                foreach (var wheelCol in extraWheelsColList)
                {
                    if (wheelCol.axel == AxlAI.Front)
                    {
                        wheelCol.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
                        AdjustTorque(wheelHit.forwardSlip);
                    }
                }

                break;
        }
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= slipLimit && currentTorque >= 0)
        {
            currentTorque -= 10 * tractionControl;
        }
        else
        {
            currentTorque += 10 * tractionControl;
            if (currentTorque > motorTorque)
            {
                currentTorque = motorTorque;
            }
        }
    }

    #endregion

    #region SKID MANAGER

    private void CheckForWheelSpin()
    {
        WheelHit wheelnHit;
        WheelHit flnHit;
        WheelHit frnHit;
        WheelHit blnHit;
        WheelHit brnHit;

        frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out flnHit);
        frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out frnHit);
        backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out blnHit);
        backRightCol.GetComponent<WheelCollider>().GetGroundHit(out brnHit);

        #region FRONT LEFT SLIP


        if (Mathf.Abs(flnHit.forwardSlip) >= slipLimit || Mathf.Abs(flnHit.sidewaysSlip) >= slipLimit)
        {
            frontLeftCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();

            if (!AnySkidSoundPlaying())
            {
                frontLeftCol.GetComponent<WheelsFXAI>().PlayAudio();
            }
        }
        else
        {
            if (frontLeftCol.GetComponent<WheelsFXAI>().playingAudio)
            {
                frontLeftCol.GetComponent<WheelsFXAI>().StopAudio();
            }

            frontLeftCol.GetComponent<WheelsFXAI>().EndSkidTrail();
        }


        #endregion

        #region FRONT RIGHT SLIP

        if (Mathf.Abs(frnHit.forwardSlip) >= slipLimit || Mathf.Abs(frnHit.sidewaysSlip) >= slipLimit)
        {
            frontRightCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();

            if (!AnySkidSoundPlaying())
            {
                frontRightCol.GetComponent<WheelsFXAI>().PlayAudio();
            }
        }
        else
        {
            if (frontRightCol.GetComponent<WheelsFXAI>().playingAudio)
            {
                frontRightCol.GetComponent<WheelsFXAI>().StopAudio();
            }

            frontRightCol.GetComponent<WheelsFXAI>().EndSkidTrail();
        }


        #endregion

        #region BACK LEFT SLIP

        if (Mathf.Abs(blnHit.forwardSlip) >= slipLimit || Mathf.Abs(blnHit.sidewaysSlip) >= slipLimit)
        {
            backLeftCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();

            if (!AnySkidSoundPlaying())
            {
                backLeftCol.GetComponent<WheelsFXAI>().PlayAudio();
            }
        }
        else
        {
            if (backLeftCol.GetComponent<WheelsFXAI>().playingAudio)
            {
                backLeftCol.GetComponent<WheelsFXAI>().StopAudio();
            }

            backLeftCol.GetComponent<WheelsFXAI>().EndSkidTrail();
        }



        #endregion

        #region BACK RIGHT SLIP

        if (Mathf.Abs(brnHit.forwardSlip) >= slipLimit || Mathf.Abs(brnHit.sidewaysSlip) >= slipLimit)
        {
            backRightCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();

            if (!AnySkidSoundPlaying())
            {
                backRightCol.GetComponent<WheelsFXAI>().PlayAudio();
            }
        }
        else
        {
            if (backRightCol.GetComponent<WheelsFXAI>().playingAudio)
            {
                backRightCol.GetComponent<WheelsFXAI>().StopAudio();
            }

            backRightCol.GetComponent<WheelsFXAI>().EndSkidTrail();
        }

        #endregion

        #region EXTRA WHEELS SLIP

        foreach (var wheelCol in extraWheelsColList)
        {
            wheelCol.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelnHit);

            if (Mathf.Abs(wheelnHit.forwardSlip) >= slipLimit || Mathf.Abs(wheelnHit.sidewaysSlip) >= slipLimit)
            {
                wheelCol.collider.GetComponent<WheelsFXAI>().EmitTyreSmoke();

                if (!AnySkidSoundPlaying())
                {
                    wheelCol.collider.GetComponent<WheelsFXAI>().PlayAudio();
                }
                continue;
            }


            if (wheelCol.collider.GetComponent<WheelsFXAI>().playingAudio)
            {
                wheelCol.collider.GetComponent<WheelsFXAI>().StopAudio();
            }

            wheelCol.collider.GetComponent<WheelsFXAI>().EndSkidTrail();

        }

        #endregion
    }

    private bool AnySkidSoundPlaying()
    {

        if (frontLeftCol.GetComponent<WheelsFXAI>().playingAudio)
        {
            return true;
        }

        else if (frontRightCol.GetComponent<WheelsFXAI>().playingAudio)
        {
            return true;
        }

        else if (backLeftCol.GetComponent<WheelsFXAI>().playingAudio)
        {
            return true;
        }

        else if (backRightCol.GetComponent<WheelsFXAI>().playingAudio)
        {
            return true;
        }

        else foreach (var wheelCol in extraWheelsColList)
            {
                if (wheelCol.collider.GetComponent<WheelsFXAI>().playingAudio)
                {
                    return true;
                }
            }

        return false;
    }

    #endregion

    #region EXAUST

    public void ExhaustFX()
    {
        if (exhaustObj != null)
        {
            exhaustSoundSource.Play();
            exhaustVisual.Play();
        }
    }

    public void CreateExhaustGameObj()
    {
        exhaustObjectPrefab = Resources.Load<Transform>("ExhaustPipe");
        exhaustObject = Instantiate(exhaustObjectPrefab);
        exhaustObject.transform.parent = this.transform.GetChild(1);
    }

    public void DestroyExhaustGameObj()
    {
        exhaustObj = transform.GetChild(1).Find("ExhaustPipe(Clone)").gameObject;
        DestroyImmediate(exhaustObj, true);
    }

    #endregion
}
