using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    private enum SuckGunMode { MOVEMENT, COMBAT };

    [Header("Dev Menu")]
    [SerializeField]
    private bool devMenu = false;

    [Header("HP")]
    [SerializeField]
    private float maxHealth = 100.0f;
    private float health;
    [SerializeField]
    private float respawnDelay = 2.0f;
    [SerializeField]
    private float timeToRegen = 5.0f;
    [SerializeField]
    private float regenAmount = 2.5f;
    public float NormalizedHealth
    {
        get { return health / maxHealth; }
    }
    private bool dying = false;
    private bool canMove = true;
    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    [Header("Suck Gun")]
    [SerializeField]
    private SuckGunMode currentMode;
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private float cameraDistance = 5f;
    [SerializeField]
    private Transform fireLocation;

    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Material combatModeMat;
    [SerializeField]
    private Material movementModeMat;
    [SerializeField]
    private Renderer SGMouth;
    [SerializeField]
    private Renderer SGRing;
    [SerializeField]
    private Renderer SergeantScreen;
    [SerializeField]
    private int maxAmmo = 10;
    private int ammo = 0;

    [Header("Movement")]
    [SerializeField]
    private float maxForce = 25f;
    [SerializeField]
    private float chargeTime = 3f;
    [SerializeField]
    private float forceMultiplier = 2f;
    [SerializeField]
    private float brakeDampening = 0.5f;
    private bool isBraking = false;

    private Transform playerLocation;
    private Rigidbody rb;

    private bool isCharging = false;
    private float chargeStartTime;

    private float currentForce = 0.0f;

    [Header("Goo Bounce")]
    public bool Bounced = false;
    public Vector3 BounceVector = Vector3.zero;

    [Header("Sound")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip deathClip;
    [SerializeField]
    private AudioClip SGModeSwitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerLocation = GetComponent<Transform>();
        //originalColor = playerRenderer.material.color;    //we won't be using this soon.
        health = maxHealth;
        ammo = 0;
    }

    public void Reset()
    {
        health = maxHealth;
        dying = false;
        canMove = true;
    }

    void Update()
    {
        if (canMove)
        {
            UpdateCamera();
            UpdatePlayer();    //We may need the space of the update function so we will choose to create functions

            if (!dying)
            {
                UpdateHealth();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (currentMode == SuckGunMode.MOVEMENT)
        {
            if (other.CompareTag("AmmoRock"))
            {
                if (ammo < maxAmmo)
                {
                    other.gameObject.SetActive(false);
                    ammo++;
                }
            }
        }

        if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(10);
        }
    }

    private void UpdateCamera()
    {
        playerCamera.transform.position = new Vector3(playerLocation.position.x, cameraDistance, playerLocation.position.z);
    }

    private void UpdateSuckGunMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentMode == SuckGunMode.MOVEMENT)
            {
                currentMode = SuckGunMode.COMBAT;
                //Orange Material
                SGMouth.material = combatModeMat;
                SGRing.material = combatModeMat;
                SergeantScreen.material = combatModeMat;

            }
            else if (currentMode == SuckGunMode.COMBAT)
            {
                currentMode = SuckGunMode.MOVEMENT;
                //Blue material
                SGMouth.material = movementModeMat;
                SGRing.material = movementModeMat;
                SergeantScreen.material = movementModeMat;
            }

        }
    }
    private void UpdatePlayer()
    {
        if(canMove)
        {
            //We call to update the suck gun mode
            UpdateSuckGunMode();

            //tracking the mouse position in relation to the player
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;

            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 lookDir = cameraRay.GetPoint(rayLength);
                lookDir.y = transform.position.y;
                Vector3 Direction = (lookDir - transform.position).normalized;

                Quaternion lookRotation = Quaternion.LookRotation(Direction);
                Vector3 lookAngles = lookRotation.eulerAngles;

                playerLocation.rotation = Quaternion.Euler(0f, lookAngles.y, 0f);
                rb.rotation = Quaternion.Euler(0f, lookAngles.y, 0f);

                switch (currentMode)
                {
                    case SuckGunMode.MOVEMENT:

                        if (Input.GetMouseButtonDown(0))
                        {
                            ChargeUp();
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            ChargeRelease(Direction);
                        }

                        if (Input.GetMouseButtonDown(1))
                        {
                            isBraking = true;
                        }

                        if(Input.GetMouseButtonUp(1))
                        {
                            isBraking = false;
                        }
                        break;

                    case SuckGunMode.COMBAT:

                        if (Input.GetMouseButtonDown(0))
                        {
                            FireProjectile();
                        }
                        break;
                }
            }


            //Handle the goo bounce
            if (Bounced == true)
            {
                rb.AddForce(-BounceVector, ForceMode.Impulse);
                Bounced = false;
            }
            //handle the braking
            if (isBraking == true)
            {
                rb.AddForce(-rb.velocity * brakeDampening, ForceMode.Force);
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxForce);
        }

    }

    //Suck Gun Functions
    private void FireProjectile()
    {
        if (projectilePrefab && ammo > 0)
        {
            Vector3 direction = transform.forward;
            direction.y = 0f;
            direction.Normalize();

            GameObject projectileGO = ObjectPoolManager.Instance.GetPooledObject(ObjectPoolManager.PoolTypes.playerBullet);
            Projectile projScript = projectileGO.GetComponent<Projectile>();
            projectileGO.SetActive(true);

            projectileGO.transform.position = fireLocation.position;
            projScript.FireDirection = direction;
            ammo--;
        }
    }
    
    private void ChargeUp()
    {
        //Start the charge
        isCharging = true;
        chargeStartTime = Time.time;
    }
    private void ChargeRelease(Vector3 Direction)
    {
        //End the charge and reset the gauge
        isCharging = false;
        chargeStartTime = 0f;

        float chargeTimeTotal = Time.time - chargeStartTime;
        float normalizedCharge = Mathf.Clamp01(chargeTimeTotal / chargeTime);
        float force = Mathf.Lerp(0f, maxForce, normalizedCharge);
        currentForce = force;
        Vector3 oppositeDir = -Direction;
        rb.AddForce(oppositeDir * force * forceMultiplier, ForceMode.Impulse);
    }

    public void TakeDamage(int dmgAmount)
    {
        health -= dmgAmount;

        if (health <= 0)
        {
            Death();
        }
    }

    private void UpdateHealth()
    {
        //Death Coroutine
        if (health <= 0)
            StartCoroutine("Death");
        else if (health < 100f)
            StartCoroutine("RegenHealth");
    }

    private IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(timeToRegen);

        health += regenAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    private IEnumerator Death()
    {
        audioSource.PlayOneShot(deathClip);
        dying = true;
        canMove = false;
        //animator.SetTrigger("Death");

        yield return new WaitForSeconds(respawnDelay);
        GameManager.Instance.PlayerDeathEvent();
    }

    public void UpdateAmmo()
    {
        if (health <= 0)
            ammo = 0;       //Ammo resets on death
        
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
    }

    private void OnGUI()
    {
        if(devMenu)
        {
            GUI.Label(new Rect(10, 10, 200, 40), "Health " + health);
            GUI.Label(new Rect(10, 30, 400, 40), "Ammo " + ammo);
            GUI.Label(new Rect(10, 50, 600, 40), "Mode " + currentMode);

            if (GUI.Button(new Rect(10, 70, 200, 40), "Full Ammo"))
            {
                ammo = maxAmmo;
            }
        }
    }
}
