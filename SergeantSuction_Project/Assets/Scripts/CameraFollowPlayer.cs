using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;

    [SerializeField]
    //We want a 50 height camera
    private float cameraHeight = 50;

    private Transform playerLocation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerLocation = GetComponent<Transform>();
        camera.transform.position = new Vector3(playerLocation.position.x, cameraHeight, playerLocation.position.z);
    }
}
