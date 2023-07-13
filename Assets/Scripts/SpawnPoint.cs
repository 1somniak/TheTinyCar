using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform directionalLight;
    public List<GameObject> prefabs;
    public int n;
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

        // if (i > 5)
        //     Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform.position - new Vector3(0, 0, n), Quaternion.Euler(0, 180, 0));
        // else
        //     i += Time.deltaTime;
    }

    private void SpawnHouseBars()
    {
        if (transform.position.z > 96.39 + (numberHousesBars - 2) * 178.55)
        {
            Instantiate(houseBar, new Vector3(518.3763f, 0f, 119.561f + numberHousesBars * 178.55f), Quaternion.Euler(0, 0, 0));
            numberHousesBars++;
            Instantiate(houseBar, new Vector3(518.3763f, 0f, 119.561f + numberHousesBars * 178.55f), Quaternion.Euler(0, 0, 0));
            numberHousesBars++;
        }
    }
    
    
    // 453.49 - 96.39 = 357.1
    // 357.1 / 2 = 178.55
}
