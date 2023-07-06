using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarAIInputs : MonoBehaviour
{
    #region REFERENCES

    private AnyCarAI carAIReference;

    private Transform frontSensor;
    private Transform rightSensor;
    private Transform leftSensor;

    #endregion

    #region UTILITY

    private float randomValue;    
    private float avoidOtherCarTime;
    private float avoidOtherCarSlowdown;
    private float avoidPathOffset;

    [HideInInspector]
    public bool reverseGearOn = false;
    [HideInInspector]
    public bool persuitAiOn = false;
    private bool avoidingObstacle = false;
    private bool isBraking = false;

    private float avoidObstacleMultiplier;

    private float targetAngle;

    #endregion

    private void Awake()
    {
        #region REFERENCES

        carAIReference = GetComponent<AnyCarAI>();
        frontSensor = this.transform.GetChild(1).GetChild(3).GetChild(0);
        rightSensor = this.transform.GetChild(1).GetChild(3).GetChild(1);
        leftSensor = this.transform.GetChild(1).GetChild(3).GetChild(2);

        persuitAiOn = carAIReference.persuitAiOn;

        #endregion

        randomValue = Random.value * 100;
    }


    private void FixedUpdate()
    {
        if (carAIReference.carAItarget == null || !carAIReference.isDriving)
        {
            if (carAIReference.persuitTarget != null && carAIReference.persuitAiOn)
            {
                PersuitSensors();
            }
            else
            {
                carAIReference.Move(0, 0, -1f, 1f);
            }
        }
        else
        {
            if (carAIReference.persuitAiOn && carAIReference.persuitTarget != null)
            {
                PersuitSensors();
            }
            else if(!carAIReference.persuitAiOn)
            {
                persuitAiOn = false;
            }

            ReverseGearSensors();
            if (!reverseGearOn)
            {
                AvoidSensors();
                BrakeSensors();
            }            

            #region MAX SPEED

            Vector3 fwd = transform.forward;
            if (carAIReference.rb.velocity.magnitude > carAIReference.maxSpeed * 0.1f)
            {
                fwd = carAIReference.rb.velocity;
            }

            float desiredSpeed = carAIReference.maxSpeed;

            #endregion

            #region BRAKE FOR PATH

            switch (carAIReference.brakeCondition)
            {
                case BrakeCondition.TargetDirectionDifference:
                    {
                        float approachingCornerAngle = Vector3.Angle(carAIReference.carAItarget.forward, fwd);
                        float spinningAngle = carAIReference.rb.angularVelocity.magnitude * carAIReference.cautiousAngularVelocityFactor;
                        float cautiousnessRequired = Mathf.InverseLerp(0, carAIReference.cautiousAngle, Mathf.Max(spinningAngle, approachingCornerAngle));
                        desiredSpeed = Mathf.Lerp(carAIReference.maxSpeed, carAIReference.maxSpeed * carAIReference.cautiousSpeedFactor, cautiousnessRequired);
                        break;
                    }

                case BrakeCondition.TargetDistance:
                    {
                        Vector3 delta = carAIReference.carAItarget.position - transform.position;
                        float distanceCautiousFactor = Mathf.InverseLerp(carAIReference.cautiousDistance, 0, delta.magnitude);
                        float spinningAngle = carAIReference.rb.angularVelocity.magnitude * carAIReference.cautiousAngularVelocityFactor;
                        float cautiousnessRequired = Mathf.Max( Mathf.InverseLerp(0, carAIReference.cautiousAngle, spinningAngle), distanceCautiousFactor);
                        desiredSpeed = Mathf.Lerp(carAIReference.maxSpeed, carAIReference.maxSpeed * carAIReference.cautiousSpeedFactor, cautiousnessRequired);
                        break;
                    }

                case BrakeCondition.NeverBrake:
                    break;
            }

            #endregion

            #region EVASIVE ACTION

            Vector3 offsetTargetPos = carAIReference.carAItarget.position;

            if (Time.time < avoidOtherCarTime)
            {
                desiredSpeed *= avoidOtherCarSlowdown;
                offsetTargetPos += carAIReference.carAItarget.right * avoidPathOffset;
            }
            else
            {
                offsetTargetPos += carAIReference.carAItarget.right * (Mathf.PerlinNoise(Time.time * carAIReference.lateralWanderSpeed, randomValue) * 2 - 1) * carAIReference.lateralWander;
            }

            #endregion

            #region SENSITIVITY

            float accelBrakeSensitivity = (desiredSpeed < carAIReference.currentSpeed)
                                              ? carAIReference.brakeSensitivity
                                              : carAIReference.accelSensitivity;


            float accel = Mathf.Clamp((desiredSpeed - carAIReference.currentSpeed) * accelBrakeSensitivity, -1, 1);

            #endregion

            #region STEER

            accel *= carAIReference.wanderAmount + (Mathf.PerlinNoise(Time.time * carAIReference.accelWanderSpeed, randomValue) * carAIReference.wanderAmount);

            Vector3 localTarget;

            localTarget = transform.InverseTransformPoint(offsetTargetPos);

            if (avoidingObstacle)
            {
                targetAngle = carAIReference.maximumSteerAngle * avoidObstacleMultiplier;
            }
            else if (carAIReference.persuitAiOn)
            {
                if(carAIReference.persuitTarget != null)
                {
                    Transform tempTarget = carAIReference.persuitTarget.GetComponentInChildren<MeshCollider>().transform;
                    Vector3 relativeVector = transform.InverseTransformPoint(tempTarget.position);
                    targetAngle = (relativeVector.x / relativeVector.magnitude) * carAIReference.maximumSteerAngle;
                }
            }
            else
            {
                targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            }

            float steer = Mathf.Clamp(targetAngle * carAIReference.steerSensitivity, -1, 1) * Mathf.Sign(carAIReference.currentSpeed);

            #endregion

            #region MOVE CAR

            if (isBraking)
            {
                carAIReference.Move(steer, 0f, -1f, 0f);
            }
            else if (reverseGearOn)
            {
                carAIReference.Move(-steer, -1f, -1f, 0f);
            }
            else
            {
                carAIReference.Move(steer, accel, accel, 0f);
            }

            #endregion

            #region REACHED TARGET STOP

            if (carAIReference.stopWhenTargetReached && localTarget.magnitude < carAIReference.reachTargetThreshold)
            {
                carAIReference.isDriving = false;
            }

            #endregion
        }
    }

    #region COLLISION WITH OTHER CARS

    private void OnCollisionStay(Collision col)
    {
        if (col.rigidbody != null)
        {
            var otherAI = col.rigidbody.GetComponent<CarAIInputs>();
            if (otherAI != null)
            {
                avoidOtherCarTime = Time.time + 1;

                if (Vector3.Angle(transform.forward, otherAI.transform.position - transform.position) < 90)
                {
                    avoidOtherCarSlowdown = 0.5f;
                }
                else
                {
                    avoidOtherCarSlowdown = 1;
                }

                var otherCarLocalDelta = transform.InverseTransformPoint(otherAI.transform.position);
                float otherCarAngle = Mathf.Atan2(otherCarLocalDelta.x, otherCarLocalDelta.z);
                avoidPathOffset = carAIReference.lateralWander * -Mathf.Sign(otherCarAngle);
            }
        }
    }

    #endregion

    #region AVOID SENSORS    
    private void AvoidSensors()
    {
        RaycastHit hit;
        avoidingObstacle = false;
        bool b;

        // Right Sensor
        if (Physics.Raycast(rightSensor.position, frontSensor.transform.forward, out hit, carAIReference.avoidDistance))
        {
            try
            {
                b = !hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain");
                if (b)
                {
                    Debug.DrawLine(rightSensor.position, hit.point, Color.yellow);
                    avoidingObstacle = true;
                    avoidObstacleMultiplier -= 1f;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        // Right Angle Sensor
        else if (Physics.Raycast(rightSensor.position, Quaternion.AngleAxis(carAIReference.sensorsAngle, rightSensor.up) * rightSensor.forward, out hit, carAIReference.avoidDistance))
        {
            try
            {
                b = !hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain");
                if (b)
                {
                    Debug.DrawLine(rightSensor.position, hit.point, Color.yellow);
                    avoidingObstacle = true;
                    avoidObstacleMultiplier -= 0.5f;
                }
            }
            catch (Exception)
            {
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensor.position, frontSensor.forward, out hit, carAIReference.avoidDistance))
        {
            try
            {
                b = !hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain");
                if (b)
                {
                    Debug.DrawLine(leftSensor.position, hit.point, Color.yellow);
                    avoidingObstacle = true;
                    avoidObstacleMultiplier += 1f;
                }
            }
            catch (Exception)
            {
                //
            }
        }


        // Left Angle Sensor
        else if (Physics.Raycast(leftSensor.position, Quaternion.AngleAxis(-carAIReference.sensorsAngle, leftSensor.up) * leftSensor.forward, out hit, carAIReference.avoidDistance))
        {
            try
            {
                b = !hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain");
                if (b)
                {
                    Debug.DrawLine(leftSensor.position, hit.point, Color.yellow);
                    avoidingObstacle = true;
                    avoidObstacleMultiplier += 0.5f;
                }
            }
            catch (Exception)
            {
                //
            }
        }

        if (avoidObstacleMultiplier == 0)
        {
            //front center sensor
            if (Physics.Raycast(frontSensor.position, frontSensor.forward, out hit, carAIReference.avoidDistance))
            {
                try
                {
                    b = !hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain");
                    if (b)
                    {
                        Debug.DrawLine(frontSensor.position, hit.point, Color.yellow);
                        avoidingObstacle = true;
                        if (hit.normal.x < 0)
                        {
                            avoidObstacleMultiplier = -1f;
                        }
                        else
                        {
                            avoidObstacleMultiplier = 1f;
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
        }
    }

    #endregion

    #region BRAKE SENSORS

    private void BrakeSensors()
    {
        RaycastHit hit;
        isBraking = false;

        // Right Sensor
        if (Physics.Raycast(rightSensor.position, frontSensor.forward, out hit, carAIReference.brakeDistance))
        {
            if (!hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(rightSensor.position, hit.point, Color.magenta);
                isBraking = true;
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensor.position, frontSensor.forward, out hit, carAIReference.brakeDistance))
        {
            if (!hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(leftSensor.position, hit.point, Color.magenta);
                isBraking = true;
            }
        }
    }

    #endregion

    #region REVERSE GEAR SENSORS

    private void ReverseGearSensors()
    {
        RaycastHit hit;
        reverseGearOn = false;

        // Right Sensor
        if (Physics.Raycast(rightSensor.position, frontSensor.forward, out hit, carAIReference.reverseDistance))
        {
            if (!hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(rightSensor.position, hit.point, Color.blue);
                reverseGearOn = true;
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensor.position, frontSensor.forward, out hit, carAIReference.reverseDistance))
        {
            if (!hit.collider.GetComponent<CompetitiveDrivingCheck>() && !hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(leftSensor.position, hit.point, Color.blue);
                reverseGearOn = true;
            }
        }
    }

    #endregion

    #region PERSUIT AI SENSORS

    private void PersuitSensors()
    {
        RaycastHit hit;

        // Right Sensor
        if (Physics.Raycast(rightSensor.position, frontSensor.forward, out hit, carAIReference.persuitDistance))
        {
            if (hit.collider == carAIReference.persuitTarget.GetComponentInChildren<MeshCollider>())
            {
                Debug.DrawLine(rightSensor.position, hit.point, Color.white);
                persuitAiOn = true;
            }
        }

        // Right Angle Sensor
        if (Physics.Raycast(rightSensor.position, Quaternion.AngleAxis(carAIReference.sensorsAngle, rightSensor.up) * rightSensor.forward, out hit, carAIReference.persuitDistance))
        {
            if (hit.collider == carAIReference.persuitTarget.GetComponentInChildren<MeshCollider>())
            {                
                Debug.DrawLine(rightSensor.position, hit.point, Color.white);
                persuitAiOn = true;
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensor.position, frontSensor.forward, out hit, carAIReference.persuitDistance))
        {
            if (hit.collider == carAIReference.persuitTarget.GetComponentInChildren<MeshCollider>())
            {
                Debug.DrawLine(leftSensor.position, hit.point, Color.white);
                persuitAiOn = true;
            }
        }


        // Left Angle Sensor
        if (Physics.Raycast(leftSensor.position, Quaternion.AngleAxis(-carAIReference.sensorsAngle, leftSensor.up) * leftSensor.forward, out hit, carAIReference.persuitDistance))
        {
            if (hit.collider == carAIReference.persuitTarget.GetComponentInChildren<MeshCollider>())
            {
                Debug.DrawLine(leftSensor.position, hit.point, Color.white);
                persuitAiOn = true;
            }
        }

        if (Physics.Raycast(frontSensor.position, frontSensor.forward, out hit, carAIReference.persuitDistance))
        {
            if (hit.collider == carAIReference.persuitTarget.GetComponentInChildren<MeshCollider>())
            {
                Debug.DrawLine(leftSensor.position, hit.point, Color.white);
                persuitAiOn = true;
            }
        }
    }

    #endregion

    public void SetTarget(Transform target)
    {
        carAIReference.carAItarget = target;
        carAIReference.isDriving = true;
    }
}
