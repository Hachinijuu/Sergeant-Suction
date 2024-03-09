using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private enum ProjectileType { REGULAR, HOMING }
    [SerializeField]
    private ProjectileType currentMode;

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float deathTime = 5f;
    [SerializeField]
    private float maxHomingTime = 1.0f;

    private float timeHoming;

    private Vector3 fireDirection;
    public Vector3 FireDirection
    {
        set { fireDirection = value; }
    }

    GameObject player;

    private void Awake()
    {
        //Getthe player's position
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            currentMode = ProjectileType.REGULAR;
            Debug.LogError("No player found");
        }
    }
    private void OnEnable()
    {
        StartCoroutine("SelfDestruct");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMode)
        {
            case ProjectileType.REGULAR:
                transform.Translate(fireDirection * speed * Time.deltaTime, Space.World);
                break;
            case ProjectileType.HOMING:
                //Turn toward the player and FOLLOW IT
                fireDirection = (player.transform.position - transform.position).normalized;
                transform.Translate(fireDirection * speed * Time.deltaTime);
                timeHoming += Time.deltaTime;
                if (timeHoming > maxHomingTime)
                    currentMode = ProjectileType.REGULAR;
                break;
        }

    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(deathTime);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EventTrigger"))
        {
            return;     //Ignore event triggers
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Uhh
    }
}
