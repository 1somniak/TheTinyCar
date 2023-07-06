using UnityEngine;

public class FollowPlayerAI : MonoBehaviour
{

    private Transform cameraTarget;
    [HideInInspector]
    public float sSpeed = 10.0f;
    [HideInInspector]
    public Vector3 dist;
    public Transform lookTarget;
    public Transform[] camerasPlaces;
    private int i = 0;

    public void Start()
    {
        this.transform.GetComponent<Camera>().nearClipPlane = 0.2f;
        cameraTarget = camerasPlaces[0];
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown("c"))
        {
            i++;
            if (i >= camerasPlaces.Length)
            {
                i = 0;
            }
        }
        cameraTarget = camerasPlaces[i];

        Vector3 dPos = cameraTarget.position + dist;
        Vector3 sPos = Vector3.Lerp(transform.position, dPos, sSpeed * Time.deltaTime);
        transform.position = sPos;
        transform.LookAt(lookTarget.position);
    }
}
