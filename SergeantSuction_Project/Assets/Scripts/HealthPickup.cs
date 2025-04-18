using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private enum PickupType { HEALING, LIFE }
    [SerializeField]
    private PickupType type;

    [SerializeField]
    private int healingAmmount;

    [SerializeField]
    private ParticleSystem pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == PickupType.HEALING)
            {
                other.GetComponent<Player>().TakeDamage(-healingAmmount);
            }
            else if (type == PickupType.LIFE)
            {
                GameManager.Instance.PlayerLives += 1;
            }
            gameObject.SetActive(false);
            
        }
        else
        {
            return;
        }
    }
}
