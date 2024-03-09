using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    private enum SuckGunMode { MOVEMENT, COMBAT };

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

    [SerializeField]
    private SuckGunMode currentMode;
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private float cameraDistance = 5f;

    [Header("Movement")]
    [SerializeField]
    private float maxForce = 25f;
    [SerializeField]
    private float chargeTime = 3f;
    [SerializeField]
    private float forceMultiplier = 2f;

    private Transform playerLocation;
    private Rigidbody rb;

    private bool isCharging = false;
    private float chargeStartTime;

    private float currentForce = 0.0f;

    [Header("Goo Bounce")]
    public bool Bounced = false;
    public Vector3 BounceVector = Vector3.zero;

    [Header("Suck Gun Parameters")]
    private Transform fireLocation;

    [SerializeField]
    private GameObject projectilePrefab;

    private int maxAmmo = 10;
    private int ammo = 0;

    [Header("Sound")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip deathClip;

    private void Start()
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
        if(!dying)
        {
            UpdateHealth();
        }

        if (canMove)
        {
            UpdateCamera();
            UpdatePlayer();    //We may need the space of the update function so we will choose to create functions
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
                    //other.gameObject.SetActive(false);
                }
            }
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
            }
            else if (currentMode == SuckGunMode.COMBAT)
            {
                currentMode = SuckGunMode.MOVEMENT;
            }

        }
    }
    private void UpdatePlayer()
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

            float lookAng = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
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
                    break;

                case SuckGunMode.COMBAT:

                    if (Input.GetMouseButtonDown(0))
                    {
                        FireProjectile();
                    }
                    break;
            }
            
            //Handle the goo bounce
            if (Bounced == true)
            {
                Ray bounceRay = new Ray(BounceVector, transform.position - BounceVector);

                if (groundPlane.Raycast(bounceRay, out rayLength))
                {
                    Vector3 pointToLook = bounceRay.GetPoint(rayLength);
                    Debug.DrawLine(bounceRay.origin, pointToLook, Color.cyan);

                    lookAng = Mathf.Atan2(pointToLook.z, pointToLook.x) * Mathf.Rad2Deg;
                    playerLocation.rotation = Quaternion.Euler(0f, lookAng, 0f);
                    rb.rotation = Quaternion.Euler(0f, lookAng, 0f);

                    lookDir.y = transform.position.y;
                    Direction = (lookDir - transform.position).normalized;
                    rb.AddForce(Direction * currentForce * forceMultiplier, ForceMode.Impulse);
                }
                Bounced = false;
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

            GameObject projectileGO = GameObject.Instantiate(projectilePrefab, fireLocation.position, Quaternion.identity);
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

        //StartCoroutine("FlashRed");

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
        GUI.Label(new Rect(10, 10, 200, 40), "Health " + health);
        GUI.Label(new Rect(10, 30, 400, 40), "Ammo " + ammo);
        GUI.Label(new Rect(10, 50, 600, 40), "Mode " + currentMode);

        if(GUI.Button(new Rect(10, 70, 200, 40), "Full Ammo"))
        {
            ammo = maxAmmo;
        }
    }
}
