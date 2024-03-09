using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    private Vector3 fireDirection;
    public Vector3 FireDirection
    {
        set { fireDirection = value; }
    }
    // Use this for initialization
    private void OnEnable()
    {
        StartCoroutine("SelfDestruct");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        transform.Translate(fireDirection * speed * Time.deltaTime, Space.World);
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(10);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }

    private void OnColliderEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            //damage them?
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
