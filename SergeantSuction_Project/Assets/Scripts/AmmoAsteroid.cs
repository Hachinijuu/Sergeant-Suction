using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoAsteroid : MonoBehaviour
{
    public float moveSpeed = 5.0f; 
    public float rotateSpeed = 30.0f; 
    public float moveDistance = 10.0f; 

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = startPosition + new Vector3(movement, 0f, 0f);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
