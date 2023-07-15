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
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = directionalLight.position - new Vector3(3.2f, 0, -(375f + 444f));
        SpawnHouseBars();

        if (Input.GetKeyDown(KeyCode.Keypad1))
            SpawnCar(1, 45f, 0f);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            SpawnCar(2, 45f, 0f, Vehicle.Police);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            SpawnCar(3, 45f, 0f);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            SpawnCar(4, 45f, 0f);
        // else
        //     i += Time.deltaTime;
    }

    private void SpawnHouseBars()
    {
        if (transform.position.z > 96.39 + (numberHousesBars - 2) * 178.60)
        {
            var h1 = Instantiate(houseBar, 
                new Vector3(518.3763f, 0f, 119.561f + numberHousesBars * 178.55f), 
                Quaternion.Euler(0, 0, 0));
            numberHousesBars++;
            houseBars.Add(h1);
            var h2 = Instantiate(houseBar, 
                new Vector3(518.3763f, 0f, 119.561f + numberHousesBars * 178.55f), 
                Quaternion.Euler(0, 0, 0));
            numberHousesBars++;
            houseBars.Add(h2);
        }
    }
    

    private void SpawnCar(int way, float speed, float avancement = 0f, Vehicle vehicle = Vehicle.All)
    {
        var position = transform.position;
        var car = Instantiate(GetRandomCar(vehicle),
            new Vector3(GetCenterWay(way), position.y, position.z - avancement),
            Quaternion.Euler(0, 180, 0));
        car.GetComponent<MovingObject>().Build(speed, way);
    }


    private static float GetCenterWay(int way) =>
        way switch { 1 => 498.85f, 2 => 503.3f, 3 => 508.3f, 4 => 512.8f, _ => 498.85f };

    private GameObject GetRandomCar(Vehicle vehicle = Vehicle.All) => 
        vehicle switch
        {
            Vehicle.Taxi => prefabs[7],
            Vehicle.Police => prefabs[6],
            Vehicle.Car => prefabs[Random.Range(2, 6)],
            Vehicle.Bus => prefabs[Random.Range(0, 2)],
            _ => prefabs[Random.Range(0, 8)]
        };
    // 453.49 - 96.39 = 357.1
    // 357.1 / 2 = 178.55
    // 502.34 - 507.37
    private enum Vehicle { Car, Bus, Taxi, Police, All }
}
