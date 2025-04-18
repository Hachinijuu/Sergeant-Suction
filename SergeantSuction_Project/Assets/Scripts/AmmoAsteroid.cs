using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class AmmoAsteroid : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float maxTravelDistance = 10f;
    private float currentTravelDistance = 0f;

    private Vector3 spawnDirection;
    public Vector3 SpawnDirection
    {
        set { spawnDirection = value; }
    }

    [SerializeField]
    private Rigidbody rb;

    void Update()
    {
        //transform.Translate(spawnDirection * speed * Time.deltaTime, Space.World);
        if(rb.velocity.magnitude < speed)
        {
            rb.AddForce(spawnDirection * speed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    private void OnEnable()
    {
        rb.velocity = Vector3.zero;
        StartCoroutine("Deploy");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Deploy()
    {
        yield return new WaitForSeconds(4f);
        
        gameObject.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
