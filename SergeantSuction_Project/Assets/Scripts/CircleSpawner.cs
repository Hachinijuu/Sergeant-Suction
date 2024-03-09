using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Transform spawner;
    [SerializeField]
    private GameObject objectPrefab;
    [SerializeField]
    private float spawnRate = 1.0f;
    [SerializeField]
    private int maxObjects = 10;
    private int currentObjects;
    public int CurrentObjects
    {
        get { return currentObjects; }
        set { currentObjects = value; }
    }
   
    private float dispenserRotation = 0.0f;     //We'll dispense each asteroid at a different angle forming a little circle with them.                      
    Material spawnerMat;                        //This is for debugging purposes
    public bool canSpawn = false;               //Change to private later

    private float deployCount = 0f;

    private static Spawner instance;
    public static Spawner Instance
    {
        get
        {
            if (instance == null)
            {
                //we refer to this current instance
                instance = FindObjectOfType<Spawner>();
            }

            return instance;

        }
    }

    private void Awake()
    {
        //Debug
        //We wanna change the material color to yellow for standby
        spawnerMat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (canSpawn)
        {
            spawnerMat.color = Color.yellow;
            Deploy();
        }
        else if (!canSpawn || currentObjects == maxObjects)
        {
            spawnerMat.color = Color.red;
            return;

        }
    }

    //We want to spawn asteroids at a rate until the player's collider leaves the spawner's collider
    private void OnTriggerEnter(Collider other)
    {
        canSpawn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canSpawn = false;
    }
    
    private void SpawnObject()
    {
        spawnerMat.color = Color.green;
        Vector3 fireDirection = transform.forward;

        GameObject asteroidGO = ObjectPoolManager.Instance.GetPooledObject(ObjectPoolManager.PoolTypes.ammoAsteroid);
        AmmoAsteroid asteroid = asteroidGO.GetComponent<AmmoAsteroid>();
        asteroidGO.SetActive(true);

        asteroidGO.transform.position = spawner.position;
        asteroid.SpawnDirection = fireDirection;

        transform.Rotate(0, 45, 0); //We rotate the spawner 45 degrees to the right
    }

    void Deploy()
    {
        deployCount += Time.deltaTime;

        if (deployCount >= spawnRate)
        {
            deployCount = 0f;
            SpawnObject();
        }

        currentObjects++;
    }

    //TODO: Code an exeption to stop the spawning when the object pool is empty
}
