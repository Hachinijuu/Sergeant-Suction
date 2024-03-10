using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormholeTunnel : MonoBehaviour
{
    [SerializeField]
    Vector3 tunnelDir;
    [SerializeField]
    private float tunnelSpeed = 10f; 
    private Rigidbody playerRB;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRB = other.GetComponent<Rigidbody>();
            if (playerRB != null)
            {   
                playerRB.AddForce(tunnelDir.normalized * tunnelSpeed, ForceMode.Force);
            }
        }
    }
}
