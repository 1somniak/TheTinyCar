using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform directionalLight;
    public List<GameObject> prefabs;
    public List<GameObject> houseBars;
    public GameObject houseBar;
    public float i;
    public int numberHousesBars = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = directionalLight.position - new Vector3(3.2f, 0, -(375f + 444f));
        SpawnHouseBars();

        if (Input.GetKeyDown(KeyCode.R))
            Function1();
        // else
        //     i += Time.deltaTime;
    }

    private void SpawnHouseBars()
    {
        if (transform.position.z > 96.39 + (numberHousesBars - 2) * 178.60)
        {
            var h1 = Instantiate(houseBar, new Vector3(518.3763f, 0f, 119.561f + numberHousesBars * 178.55f), Quaternion.Euler(0, 0, 0));
            numberHousesBars++;
            houseBars.Add(h1);
            var h2 = Instantiate(houseBar, new Vector3(518.3763f, 0f, 119.561f + numberHousesBars * 178.55f), Quaternion.Euler(0, 0, 0));
            numberHousesBars++;
            houseBars.Add(h2);
        }
    }

    private void Function1()
    {
        var car1 = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform.position - new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));
        car1.GetComponent<MovingObject>().Build(45);
    }


    private float GetCenterWay(int way) =>
        way switch { 1 => 498.85f, 2 => 503.3f, 3 => 508.3f, 4 => 512.8f, _ => 498.85f };
    // 453.49 - 96.39 = 357.1
    // 357.1 / 2 = 178.55
    // 502.34 - 507.37
}
