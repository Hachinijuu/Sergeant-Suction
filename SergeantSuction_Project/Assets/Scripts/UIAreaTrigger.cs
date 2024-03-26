using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAreaTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject uiElement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiElement.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiElement.SetActive(false);
        }
    }
}
