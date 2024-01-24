using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagFilteredPlatform : MonoBehaviour
{
    [Tooltip("does this platform require an object to activate it")]
    public bool colliderActivated = false;
    [Tooltip("Tag for the object that will activate the platform")]
    public string colliderTag = "Player";

    [HideInInspector]
    public bool playerOnPlatform = false;

    Collider trigger;

    private void Start()
    {
        trigger = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == colliderTag)
        {
            playerOnPlatform = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == colliderTag)
        {
            playerOnPlatform = false;
        }
    }
}
