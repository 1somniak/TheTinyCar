using UnityEngine;

public class CarAIWaipointTracker : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    private WaypointsPath circuit;
    private AnyCarAI ACAI;

    private float lookAheadForTargetOffset = 5;
    private float lookAheadForTargetFactor = .1f;
    private float lookAheadForSpeedOffset = 10;
    private float lookAheadForSpeedFactor = .2f;
    private int progressStyle;
    private float pointToPointThreshold = 4;

    private float progressDistance;
    private int progressNum;
    private Vector3 lastPosition;
    private float speed;


    #region KEY POINTS

    [HideInInspector]
    public Transform[] pathTransform;

    [HideInInspector]
    public WaypointsPath.RoutePoint targetPoint { get; private set; }
    [HideInInspector]
    public WaypointsPath.RoutePoint speedPoint { get; private set; }
    [HideInInspector]
    public WaypointsPath.RoutePoint progressPoint { get; private set; }

    #endregion

    private void Start()
    {
        #region GET ACAI VALUES

        ACAI = this.GetComponent<AnyCarAI>();
        circuit = ACAI.AIcircuit;
        lookAheadForTargetOffset = ACAI.lookAheadForTarget;
        lookAheadForTargetFactor = ACAI.lookAheadForTargetFactor;
        lookAheadForSpeedOffset = ACAI.lookAheadForSpeedOffset;
        lookAheadForSpeedFactor = ACAI.lookAheadForSpeedFactor;
        pointToPointThreshold = ACAI.pointThreshold;
        progressStyle = (int)ACAI.progressStyle;

        #endregion

        if (target == null)
        {
            target = ACAI.carAItarget;
        }

        Reset();
    }  

    public void Reset()
    {
        progressDistance = 0;
        progressNum = 0;

        if (progressStyle == 1)
        {
            target.position = circuit.nodes[progressNum].position;
            target.rotation = circuit.nodes[progressNum].rotation;
        }
    }


    private void Update()
    {
        if (!this.transform.GetComponent<AnyCarAI>().persuitAiOn)
        {
            this.transform.GetComponent<CarAIInputs>().persuitAiOn = false;
            FollowPath();
        }
        else
        {
            if(ACAI.persuitTarget != null)
            {
                Transform tempPersuitCollider = ACAI.persuitTarget.GetComponentInChildren<MeshCollider>().transform;

                target = tempPersuitCollider;
            }
            else
            {
                FollowPath();
            }
        }
    }

    #region DRAW DIRECTION

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, target.position + target.forward);
        }
    }

    #endregion

    #region FOLLOW PATH

    public void FollowPath()
    {
        if (ACAI.progressStyle == 0)
        {
            if (Time.deltaTime > 0)
            {
                speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude / Time.deltaTime, Time.deltaTime);
            }
            target.position = circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed).position;
            target.rotation = Quaternion.LookRotation(circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed).direction);

            progressPoint = circuit.GetRoutePoint(progressDistance);
            Vector3 progressDelta = progressPoint.position - transform.position;
            if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
            {
                progressDistance += progressDelta.magnitude * 0.5f;
            }

            lastPosition = transform.position;
        }
        else
        {
            Vector3 targetDelta = target.position - transform.position;
            if (targetDelta.magnitude < pointToPointThreshold)
            {
                progressNum = (progressNum + 1) % circuit.nodes.Count;
            }

            target.position = circuit.nodes[progressNum].position;
            target.rotation = circuit.nodes[progressNum].rotation;

            progressPoint = circuit.GetRoutePoint(progressDistance);
            Vector3 progressDelta = progressPoint.position - transform.position;
            if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
            {
                progressDistance += progressDelta.magnitude;
            }
            lastPosition = transform.position;
        }
    }

    #endregion
}
