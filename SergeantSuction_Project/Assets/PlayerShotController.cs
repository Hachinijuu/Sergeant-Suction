using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotController : MonoBehaviour
{
    [SerializeField]
    private SuckGun suckGun;

    bool fireHolding = false;
    [SerializeField]
    float MAX_HOLDTIME = 2.0f;
    [SerializeField]
    float timeHolding = 0.0f;

    private void Update()
    {
        CheckForFire();
        if (fireHolding && timeHolding < MAX_HOLDTIME)
        {
            timeHolding += Time.deltaTime;
            if (timeHolding >= MAX_HOLDTIME)
            {
                timeHolding = MAX_HOLDTIME;
            }
            suckGun.FireHoldTime = timeHolding;
        }

    }

    private void CheckForFire()
    {
        bool checkForPrimaryFire = Input.GetButtonUp("Fire1");
        bool fireCheckStart = Input.GetButtonDown("Fire1");
        bool checkForSecondaryFire = Input.GetButtonDown("Fire2");
        if (suckGun && fireCheckStart)
        {

            fireHolding = true;
        }
        if (suckGun && checkForPrimaryFire)
        {
            suckGun.FirePrimaryFire();
            fireHolding = false;
            timeHolding = 0.0f;
        }
    }
}
