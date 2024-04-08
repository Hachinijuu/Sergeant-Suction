using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAreaTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject uiElement;
    private bool doNotRetrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!doNotRetrigger)
        {
            if (other.CompareTag("Player"))
            {
                uiElement.SetActive(true);
            }
        }
        else
        {
            uiElement.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiElement.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && uiElement.activeSelf)
        {
            uiElement.SetActive(false);
            doNotRetrigger = true;
        }
    }
}
