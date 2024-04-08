using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    enum asteroidType { ammoRock, Normal, Goo, Sharp }

    [SerializeField]
    asteroidType currentType;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (currentType == asteroidType.Normal)
            {
                if (collision.gameObject.GetComponent<Player>().playerVelocity >= 45)
                {
                    collision.gameObject.GetComponent<Player>().TakeDamage(10);
                }
                else if (collision.gameObject.GetComponent<Player>().playerVelocity <= 44 && collision.gameObject.GetComponent<Player>().playerVelocity > 25)
                {
                    collision.gameObject.GetComponent<Player>().TakeDamage(5);
                }

            }
            else if (currentType == asteroidType.Goo)
            {

                collision.gameObject.GetComponent<Player>().Bounced = true;
                collision.gameObject.GetComponent<Player>().BounceVector = collision.GetContact(0).point;
                //collision.gameObject.GetComponent<Player>().Bounce();
                //Vector3 reflectPosition = Vector3.Reflect(transform.position - collision.gameObject.transform.position, collision.GetContact(0).normal);
                // collision.gameObject.GetComponent<SuckGun>().BounceVector = reflectPosition;
                // transform.LookAt(new Vector3(reflectPosition.x, transform.position.y, reflectPosition.z));
                // transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z));
                //do reflect function 
            }
            else if (currentType == asteroidType.Sharp)
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(100);
            }
            
        }
    }
}
