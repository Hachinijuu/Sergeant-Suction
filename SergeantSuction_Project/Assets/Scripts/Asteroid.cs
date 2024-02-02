using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    enum asteroidType { Normal, Goo, Sharp }

    [SerializeField]
    asteroidType currentType;
    // Start is called before the first frame update

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
                //do smth?
                //Just do damage to the player - S.
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
                //take more damage than normal?
                //We could have the player take more damage, and get the asteroid destroyed. we could freeze the player too even. - S.
            }
        }
    }


}
