using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float speed = 5f;

    [SerializeField]
    private float lifeTime = 5f;

    protected Vector3 direction;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();   
    }

    private void Start()
    {
        //destry bullet after lifetime (to avoid wasting memory etc... )
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if(rb != null)
        {
            rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
        }
    }

    public void Fire(Vector3 dir)
    {
        //set the direction of the bullet
        direction = dir;
    }

    private void OnColliderEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
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
