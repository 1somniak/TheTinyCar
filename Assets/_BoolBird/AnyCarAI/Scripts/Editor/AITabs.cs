using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnyCarAI))]
public class AITabs : Editor
{
    private AnyCarAI myTarget;
    private SerializedObject soTarget;

    #region SETUP

    private SerializedProperty frontLeft;
    private SerializedProperty frontRight;
    private SerializedProperty backLeft;
    private SerializedProperty backRight;
    private SerializedProperty extraWheels;
    private SerializedProperty bodyMesh;

    #endregion

    #region WHEELS

    private SerializedProperty wheelsRadius;
    private SerializedProperty wheelsMass;
    private SerializedProperty forcePoint;
    private SerializedProperty dumpingRate;
    private SerializedProperty suspensionDistance;
    private SerializedProperty wheelsPosition;
    private SerializedProperty wheelsRotation;
    private SerializedProperty wheelStiffness;
    private SerializedProperty suspensionSpring;
    private SerializedProperty suspensionDamper;
    private SerializedProperty targetPosition;
    private SerializedProperty maximumSteerAngle;
    private SerializedProperty steerHelper;
    private SerializedProperty slipLimit;

    #endregion

    #region ENGINE

    private SerializedProperty enginePower;
    private SerializedProperty carDriveType;
    private SerializedProperty tractionControl;
    private SerializedProperty motorTorque;
    private SerializedProperty brakeTorque;
    private SerializedProperty reverseTorque;
    private SerializedProperty handbrakeTorque;
    private SerializedProperty speedType;
    private SerializedProperty maxSpeed;
    private SerializedProperty numberOfGears;
    private SerializedProperty downForce;
    private SerializedProperty vehicleMass;


    #endregion

    #region FEATURES

    private SerializedProperty turboON;
    private SerializedProperty exhaustFlame;
    private SerializedProperty centerOfMass;
    private SerializedProperty ABS;
    private SerializedProperty skidMarks;
    private SerializedProperty collisionSystem;
    private SerializedProperty collisionParticles;
    private SerializedProperty demolutionStrenght;
    private SerializedProperty demolutionRange;
    private SerializedProperty optionalMeshList;
    private SerializedProperty customMesh;
    private SerializedProperty smokeOn;

    private SerializedProperty brakeCondition;
    private SerializedProperty cautiousAngle;
    private SerializedProperty cautiousDistance;
    private SerializedProperty steerSensitivity;
    private SerializedProperty accelSensitivity;
    private SerializedProperty brakeSensitivity;
    private SerializedProperty lateralWander;
    private SerializedProperty wanderAmount;
    private SerializedProperty isDriving;
    private SerializedProperty stopWhenTargetReached;
    private SerializedProperty reachTargetThreshold;

    private SerializedProperty AIcircuit;
    private SerializedProperty lookAheadForTarget;
    private SerializedProperty progressStyle;
    private SerializedProperty pointThreshold;

    private SerializedProperty sensorsAngle;
    private SerializedProperty avoidDistance;
    private SerializedProperty brakeDistance;
    private SerializedProperty reverseDistance;

    private SerializedProperty persuitAiOn;
    private SerializedProperty persuitTarget;
    private SerializedProperty persuitDistance;

    #endregion

    #region AUDIO

    private SerializedProperty exhaustSound;
    private SerializedProperty exhaustVolume;
    private SerializedProperty skidSound;
    private SerializedProperty skidVolume;
    private SerializedProperty lowAcceleration;
    private SerializedProperty lowDeceleration;
    private SerializedProperty highAcceleration;
    private SerializedProperty highDeceleration;
    private SerializedProperty engineVolume;
    private SerializedProperty collisionSound;
    private SerializedProperty collisionVolume;
    private SerializedProperty suspensionsSound;
    private SerializedProperty suspensionsVolume;
    private SerializedProperty turboAudioClip;
    private SerializedProperty turboVolume;

    #endregion

    #region BUTTON REFERENCES

    public Texture buttonSetUp;
    public Texture buttonWheels;
    public Texture buttonEngine;
    public Texture buttonFeatures;
    public Texture buttonAudio;
    public Texture anyCarAILabel;

    public string turboTextButton = "OFF";
    public bool howToStop = false;

    #endregion


    private void OnEnable()
    {
        myTarget = (AnyCarAI)target;
        soTarget = new SerializedObject(target);

        #region BUTTONS

        buttonSetUp = Resources.Load<Texture>("buttonSetUp");
        buttonWheels = Resources.Load<Texture>("buttonWheels");
        buttonEngine = Resources.Load<Texture>("buttonEngine");
        buttonFeatures = Resources.Load<Texture>("buttonFeatures");
        buttonAudio = Resources.Load<Texture>("buttonAudio");
        anyCarAILabel = Resources.Load<Texture>("anyCarAILabel");

        #endregion

        #region SETUP

        frontLeft = soTarget.FindProperty("frontLeft");
        frontRight = soTarget.FindProperty("frontRight");
        backLeft = soTarget.FindProperty("backLeft");
        backRight = soTarget.FindProperty("backRight");
        extraWheels = soTarget.FindProperty("extraWheels");
        bodyMesh = soTarget.FindProperty("bodyMesh");

        #endregion

        #region WHEELS

        wheelsRadius = soTarget.FindProperty("wheelsRadius");
        wheelsMass = soTarget.FindProperty("wheelsMass");
        forcePoint = soTarget.FindProperty("forcePoint");
        dumpingRate = soTarget.FindProperty("dumpingRate");
        suspensionDistance = soTarget.FindProperty("suspensionDistance");
        suspensionDamper = soTarget.FindProperty("suspensionDamper");
        suspensionSpring = soTarget.FindProperty("suspensionSpring");
        targetPosition = soTarget.FindProperty("targetPosition");
        wheelsPosition = soTarget.FindProperty("wheelsPosition");
        wheelsRotation = soTarget.FindProperty("wheelsRotation");
        wheelStiffness = soTarget.FindProperty("wheelStiffness");
        maximumSteerAngle = soTarget.FindProperty("maximumSteerAngle");
        steerHelper = soTarget.FindProperty("steerHelper");
        slipLimit = soTarget.FindProperty("slipLimit");

        #endregion

        #region ENGINE

        enginePower = soTarget.FindProperty("enginePower");
        carDriveType = soTarget.FindProperty("carDriveType");
        tractionControl = soTarget.FindProperty("tractionControl");
        motorTorque = soTarget.FindProperty("motorTorque");
        brakeTorque = soTarget.FindProperty("brakeTorque");
        reverseTorque = soTarget.FindProperty("reverseTorque");
        handbrakeTorque = soTarget.FindProperty("handbrakeTorque");
        speedType = soTarget.FindProperty("speedType");
        maxSpeed = soTarget.FindProperty("maxSpeed");
        numberOfGears = soTarget.FindProperty("numberOfGears");
        downForce = soTarget.FindProperty("downForce");
        vehicleMass = soTarget.FindProperty("vehicleMass");

        #endregion

        #region FEATURES

        turboON = soTarget.FindProperty("turboON");
        smokeOn = soTarget.FindProperty("smokeOn");
        centerOfMass = soTarget.FindProperty("centerOfMass");
        ABS = soTarget.FindProperty("ABS");
        skidMarks = soTarget.FindProperty("skidMarks");
        optionalMeshList = soTarget.FindProperty("optionalMeshList");
        collisionSystem = soTarget.FindProperty("collisionSystem");
        collisionParticles = soTarget.FindProperty("collisionParticles");
        customMesh = soTarget.FindProperty("customMesh");
        demolutionStrenght = soTarget.FindProperty("demolutionStrenght");
        demolutionRange = soTarget.FindProperty("demolutionRange");
        exhaustFlame = soTarget.FindProperty("exhaustFlame");

        brakeCondition = soTarget.FindProperty("brakeCondition");
        cautiousAngle = soTarget.FindProperty("cautiousAngle");
        cautiousDistance = soTarget.FindProperty("cautiousDistance");
        steerSensitivity = soTarget.FindProperty("steerSensitivity");
        accelSensitivity = soTarget.FindProperty("accelSensitivity");
        brakeSensitivity = soTarget.FindProperty("brakeSensitivity");
        lateralWander = soTarget.FindProperty("lateralWander");
        wanderAmount = soTarget.FindProperty("wanderAmount");
        isDriving = soTarget.FindProperty("isDriving");
        stopWhenTargetReached = soTarget.FindProperty("stopWhenTargetReached");
        reachTargetThreshold = soTarget.FindProperty("reachTargetThreshold");


        AIcircuit = soTarget.FindProperty("AIcircuit");
        lookAheadForTarget = soTarget.FindProperty("lookAheadForTarget");
        progressStyle = soTarget.FindProperty("progressStyle");
        pointThreshold = soTarget.FindProperty("pointThreshold");

        sensorsAngle = soTarget.FindProperty("sensorsAngle");
        avoidDistance = soTarget.FindProperty("avoidDistance");
        brakeDistance = soTarget.FindProperty("brakeDistance");
        reverseDistance = soTarget.FindProperty("reverseDistance");

        persuitDistance = soTarget.FindProperty("persuitDistance");
        persuitTarget = soTarget.FindProperty("persuitTarget");
        persuitAiOn = soTarget.FindProperty("persuitAiOn");

        #endregion

        #region AUDIO

        skidSound = soTarget.FindProperty("skidSound");
        skidVolume = soTarget.FindProperty("skidVolume");
        lowAcceleration = soTarget.FindProperty("lowAcceleration");
        lowDeceleration = soTarget.FindProperty("lowDeceleration");
        highAcceleration = soTarget.FindProperty("highAcceleration");
        highDeceleration = soTarget.FindProperty("highDeceleration");
        engineVolume = soTarget.FindProperty("engineVolume");
        collisionVolume = soTarget.FindProperty("collisionVolume");
        collisionSound = soTarget.FindProperty("collisionSound");
        exhaustSound = soTarget.FindProperty("exhaustSound");
        exhaustVolume = soTarget.FindProperty("exhaustVolume");
        suspensionsSound = soTarget.FindProperty("suspensionsSound");
        suspensionsVolume = soTarget.FindProperty("suspensionsVolume");
        turboAudioClip = soTarget.FindProperty("turboAudioClip");
        turboVolume = soTarget.FindProperty("turboVolume");

        #endregion
    }


    public override void OnInspectorGUI()
    {
        GUILayout.Box(anyCarAILabel, GUILayout.ExpandWidth(true), GUILayout.Height(55));
        soTarget.Update();

        EditorGUI.BeginChangeCheck();


        myTarget.toolbarTab = GUILayout.Toolbar(myTarget.toolbarTab, new Texture[] { buttonSetUp, buttonWheels, buttonEngine, buttonFeatures, buttonAudio }, GUILayout.Height(30));

        switch (myTarget.toolbarTab)
        {
            case 0:
                myTarget.currentTab = "Set Up";
                break;
            case 1:
                myTarget.currentTab = "Wheels";
                break;
            case 2:
                myTarget.currentTab = "Engine";
                break;
            case 3:
                myTarget.currentTab = "Features";
                break;
            case 4:
                myTarget.currentTab = "Audio";
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        EditorGUI.BeginChangeCheck();

        switch (myTarget.currentTab)
        {
            case "Set Up":

                #region SETUP

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("SET UP", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Make references of your car model and attach script.");

                #region MAIN SETUP

                EditorGUILayout.Space();
                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Wheels", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(frontLeft);
                EditorGUILayout.PropertyField(frontRight);
                EditorGUILayout.PropertyField(backLeft);
                EditorGUILayout.PropertyField(backRight);
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.Space(12);
                EditorGUILayout.PropertyField(extraWheels, true);
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Car Body", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(bodyMesh);

                if (GUILayout.Button("Debug BodyCollider"))
                {
                    myTarget.CreateDebugBodyCol();
                }

                GUILayout.EndVertical();
                
                GUI.color = Color.Lerp(Color.white, Color.grey, 0.2f);
                if (GUILayout.Button("ATTACH SCRIPT", GUILayout.MinHeight(40)))
                {
                    myTarget.UnpackPrefab();
                    UnpackPrefab();
                    myTarget.CreateColliders();
                }
                GUI.color = Color.white;
                EditorGUILayout.Space();

                #endregion

                #endregion

                #region FEATURES

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("OPTIONS", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Set extra features and customize user preferences.");
                EditorGUILayout.Space();

                #endregion

                #region FEATURES BUTTONS

                GUILayout.BeginHorizontal();

                #region SKID MARKS

                if (skidMarks.boolValue == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("Skid Marks", GUILayout.MinWidth(120)))
                {
                    if (skidMarks.boolValue == true)
                    {
                        skidMarks.boolValue = false;
                    }
                    else
                    {
                        skidMarks.boolValue = true;
                    }
                }

                GUI.color = Color.white;

                #endregion

                #region EXHAUST

                if (exhaustFlame.boolValue == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("Exhaust FX", GUILayout.MinWidth(120)))
                {

                    if (exhaustFlame.boolValue == true)
                    {
                        exhaustFlame.boolValue = false;
                        myTarget.DestroyExhaustGameObj();
                    }
                    else
                    {
                        exhaustFlame.boolValue = true;
                        myTarget.CreateExhaustGameObj();
                    }
                }


                GUI.color = Color.white;

                #endregion


                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();

                #region ABS

                if (ABS.boolValue == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("ABS", GUILayout.MinWidth(120)))
                {
                    if (ABS.boolValue == true)
                    {
                        ABS.boolValue = false;
                    }
                    else
                    {
                        ABS.boolValue = true;
                    }
                }

                GUI.color = Color.white;

                #endregion

                #region SMOKE

                if (smokeOn.boolValue == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("Smoke", GUILayout.MinWidth(120)))
                {
                    if (smokeOn.boolValue == true)
                    {
                        smokeOn.boolValue = false;
                    }
                    else
                    {
                        smokeOn.boolValue = true;
                    }
                }

                GUI.color = Color.white;

                #endregion


                GUILayout.EndHorizontal();
                EditorGUILayout.Space();

                #endregion

                #region COLLISION

                if (collisionSystem.boolValue == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("Collision System"))
                {

                    if (collisionSystem.boolValue == true)
                    {
                        collisionSystem.boolValue = false;
                    }
                    else
                    {
                        collisionSystem.boolValue = true;
                    }
                }

                GUI.color = Color.white;



                if (collisionSystem.boolValue == true)
                {
                    GUILayout.BeginVertical("", "box");

                    EditorGUILayout.PropertyField(demolutionStrenght);
                    EditorGUILayout.PropertyField(demolutionRange);

                    GUILayout.BeginHorizontal();

                    #region CUSTOM MESH BUTTON

                    if (customMesh.boolValue)
                    {
                        GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                    }
                    else
                    {
                        GUI.color = Color.white;
                    }

                    if (GUILayout.Button("Custom Mesh"))
                    {
                        if (customMesh.boolValue == true)
                        {
                            customMesh.boolValue = false;
                        }
                        else
                        {
                            customMesh.boolValue = true;
                        }
                    }

                    GUI.color = Color.white;

                    #endregion

                    #region COLLISION PARTICLES

                    if (collisionParticles.boolValue)
                    {
                        GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                    }
                    else
                    {
                        GUI.color = Color.white;
                    }

                    if (GUILayout.Button("Collision Particles"))
                    {
                        if (collisionParticles.boolValue == true)
                        {
                            collisionParticles.boolValue = false;
                        }
                        else
                        {
                            collisionParticles.boolValue = true;
                        }
                    }

                    GUI.color = Color.white;

                    #endregion

                    GUILayout.EndHorizontal();


                    if (customMesh.boolValue)
                    {
                        GUI.color = Color.Lerp(Color.gray, Color.white, .8f);
                        GUILayout.BeginVertical("", "box");
                        EditorGUILayout.Space();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        GUI.color = Color.white;
                        EditorGUILayout.PropertyField(optionalMeshList, true);
                        GUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                        GUILayout.EndVertical();
                    }


                    GUILayout.EndVertical();

                }

                #endregion


                break;
            case "Wheels":

                #region WHEELS

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("WHEELS", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Set wheels physics and customize user preferences.");
                EditorGUILayout.Space();

                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Tyres", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(wheelsRadius);
                EditorGUILayout.PropertyField(wheelsMass);
                EditorGUILayout.PropertyField(dumpingRate);
                EditorGUILayout.PropertyField(forcePoint);
                EditorGUILayout.Space();
                GUILayout.EndVertical();
                EditorGUILayout.Space();
                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Suspensions", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(suspensionSpring);
                EditorGUILayout.PropertyField(suspensionDamper);
                EditorGUILayout.PropertyField(suspensionDistance);
                EditorGUILayout.PropertyField(targetPosition);
                EditorGUILayout.Space();
                GUILayout.EndVertical();

                EditorGUILayout.Space();
                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Drift Controls", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(maximumSteerAngle);
                EditorGUILayout.PropertyField(wheelStiffness);
                EditorGUILayout.PropertyField(steerHelper);
                EditorGUILayout.PropertyField(tractionControl);
                EditorGUILayout.PropertyField(slipLimit);
                EditorGUILayout.Space();
                GUILayout.EndVertical();
                EditorGUILayout.Space();
                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Wheels Debug", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(wheelsPosition);
                EditorGUILayout.PropertyField(wheelsRotation);
                EditorGUILayout.Space();
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                #endregion

                break;
            case "Engine":

                #region ENGINE

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("ENGINE", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Set engine features and customize user preferences.");
                EditorGUILayout.Space();

                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Type of Engine", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(carDriveType);

                EditorGUILayout.PropertyField(numberOfGears);
                EditorGUILayout.Space();
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Power Controls", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(enginePower);
                EditorGUILayout.PropertyField(motorTorque);
                EditorGUILayout.PropertyField(brakeTorque);
                EditorGUILayout.PropertyField(reverseTorque);
                EditorGUILayout.PropertyField(handbrakeTorque);
                EditorGUILayout.PropertyField(downForce);
                EditorGUILayout.Space();
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();

                #region TURBO

                GUILayout.Label("Turbo", EditorStyles.boldLabel, GUILayout.Width(50));

                if(turboON.boolValue == true)
                {
                    turboTextButton = "ON";
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    turboTextButton = "OFF";
                    GUI.color = Color.white;
                }

                if (turboON.boolValue == false)
                {
                    if (GUILayout.Button(turboTextButton))
                    {
                        turboON.boolValue = true;
                    }
                }
                else
                {
                    if (GUILayout.Button(turboTextButton))
                    {                        
                        turboON.boolValue = false;
                    }
                }
                GUI.color = Color.white;

                #endregion

                GUILayout.EndHorizontal();


                EditorGUILayout.Space();
                GUILayout.BeginVertical("", "box");                
                EditorGUILayout.LabelField("Speed Controls", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(speedType);
                EditorGUILayout.PropertyField(maxSpeed);
                EditorGUILayout.PropertyField(vehicleMass);
                EditorGUILayout.PropertyField(centerOfMass);
                EditorGUILayout.Space();
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                #endregion

                break;
            case "Features":

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("AI", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Set car Artificial Intelligence parameters");

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Waypoints Path", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AIcircuit, GUIContent.none, true);

                EditorGUILayout.Space();

                #region FOLLOW OPTIONS

                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Follow Options", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(progressStyle);

                if(progressStyle.intValue == 0)
                {
                    EditorGUILayout.PropertyField(lookAheadForTarget);
                }
                else
                {

                    EditorGUILayout.PropertyField(pointThreshold);
                }

                GUILayout.EndVertical();

                #endregion

                EditorGUILayout.Space();


                #region SENSORS

                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Sensors", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(sensorsAngle);
                EditorGUILayout.PropertyField(avoidDistance);
                EditorGUILayout.PropertyField(brakeDistance);
                EditorGUILayout.PropertyField(reverseDistance);

                GUILayout.EndVertical();

                #endregion

                EditorGUILayout.Space();

                #region HUMANIZATOR

                EditorGUILayout.LabelField("HUMANIZATOR", EditorStyles.boldLabel);

                GUILayout.BeginVertical("", "box");

                EditorGUILayout.LabelField("Accelerating", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(accelSensitivity);
                EditorGUILayout.PropertyField(wanderAmount);

                GuiLine();

                EditorGUILayout.LabelField("Steering", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(steerSensitivity);
                EditorGUILayout.PropertyField(lateralWander);

                GuiLine();

                EditorGUILayout.LabelField("Competitive Driving", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(cautiousAngle);
                EditorGUILayout.PropertyField(cautiousDistance);


                GuiLine();

                EditorGUILayout.LabelField("Braking", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(brakeCondition);
                EditorGUILayout.PropertyField(brakeSensitivity);


                GUILayout.EndVertical();

                #endregion

                EditorGUILayout.Space();

                #region PERSUIT AI

                if (persuitAiOn.boolValue == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("Persuit AI"))
                {
                    if (persuitAiOn.boolValue == true)
                    {
                        persuitAiOn.boolValue = false;
                    }
                    else
                    {
                        persuitAiOn.boolValue = true;
                    }
                }

                if (persuitAiOn.boolValue)
                {
                    GUI.color = Color.white;
                    GUILayout.BeginVertical("", "box");

                    EditorGUILayout.PropertyField(persuitTarget);
                    EditorGUILayout.PropertyField(persuitDistance);

                    GUILayout.EndVertical();
                }

                #endregion

                #region HOW TO STOP

                if (howToStop == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("How To Stop"))
                {
                    if (howToStop == true)
                    {
                        howToStop = false;
                    }
                    else
                    {
                        howToStop = true;
                    }
                }

                if (howToStop)
                {
                    GUI.color = Color.white;
                    GUILayout.BeginVertical("The car is driving by default on Play. If you need to stop using code set boolean 'isDriving' to false. Set it on true if you need the car to drive again.", "box");
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    GUILayout.EndVertical();
                }

                #endregion

                #region STOP WHEN TARGET REACHED

                if (stopWhenTargetReached.boolValue == true)
                {
                    GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("Stop When Target Reached"))
                {
                    if (stopWhenTargetReached.boolValue == true)
                    {
                        stopWhenTargetReached.boolValue = false;
                    }
                    else
                    {
                        stopWhenTargetReached.boolValue = true;
                    }
                }

                if (stopWhenTargetReached.boolValue)
                {
                    GUI.color = Color.white;
                    GUILayout.BeginVertical("", "box");

                    EditorGUILayout.PropertyField(reachTargetThreshold);

                    GUILayout.EndVertical();
                }

                #endregion

                EditorGUILayout.Space();

                break;

            case "Audio":

                #region AUDIO

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("AUDIO", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Set audio clips and mix volumes");
                EditorGUILayout.Space();

                GUILayout.BeginVertical("", "box");
                EditorGUILayout.LabelField("Engine Audio", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(lowAcceleration);
                EditorGUILayout.PropertyField(lowDeceleration);
                EditorGUILayout.PropertyField(highAcceleration);
                EditorGUILayout.PropertyField(highDeceleration);
                EditorGUILayout.PropertyField(engineVolume);
                EditorGUILayout.Space();
                GUILayout.EndVertical();


                if (turboON.boolValue == true)
                {
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical("", "box");
                    EditorGUILayout.LabelField("Turbo Audio", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(turboAudioClip);
                    EditorGUILayout.PropertyField(turboVolume);
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                }

                if (skidMarks.boolValue == true)
                {

                    EditorGUILayout.Space();
                    GUILayout.BeginVertical("", "box");
                    EditorGUILayout.LabelField("Skid Audio", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(skidSound);
                    EditorGUILayout.PropertyField(skidVolume);
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();

                    EditorGUILayout.Space();
                    GUILayout.BeginVertical("", "box");
                    EditorGUILayout.LabelField("Suspensions Audio", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(suspensionsSound);
                    EditorGUILayout.PropertyField(suspensionsVolume);
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                }
                

                if (collisionSystem.boolValue == true)
                {
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical("", "box");
                    EditorGUILayout.LabelField("Collision Audio", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(collisionSound);
                    EditorGUILayout.PropertyField(collisionVolume);
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                }

                if (exhaustFlame.boolValue == true)
                {
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical("", "box");
                    EditorGUILayout.LabelField("Exhaust Audio", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(exhaustSound);
                    EditorGUILayout.PropertyField(exhaustVolume);
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                }

                EditorGUILayout.Space();
                #endregion

                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
        }
    }

    
    public void UnpackPrefab()
    {
        PrefabUtility.UnpackPrefabInstance(myTarget.objToUnpack, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
    }


    void GuiLine(int i_height = 1)

    {

        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

    }
}
