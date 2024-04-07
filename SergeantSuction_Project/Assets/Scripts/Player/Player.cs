using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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
    public float Health
    {
        get { return health; }
    }
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
    private int maxAmmo = 10;
    private int ammo = 0;

    [Header("Charge Up")]
    [SerializeField]
    private Image movementGauge;
    [SerializeField]
    private Image fullMovement;
    [SerializeField]
    private Image ammoGauge;
    [SerializeField]
    private Image fullAmmo;
    [SerializeField]
    private Image brakeIndicator;
    [SerializeField]
    private Image slowChargeGauge;

    [Header("Movement")]
    [SerializeField]
    private float maxCharge = 2.0f;
    [SerializeField]
    private float maxForce = 50f;
    [SerializeField]
    private float brakeDampening = 0.5f;
    [SerializeField]
    private float recoilStrength = 1.0f;
    public bool isBraking = false;

    private float chargeTimeTotal;
    private float force;

    private Transform playerLocation;
    private Rigidbody rb;

    private bool isCharging = false;
    private float chargeStartTime;

    private float currentForce = 0.0f;

    [Header("Goo Bounce")]
    public bool Bounced = false;
    public Vector3 BounceVector = Vector3.zero;

    public float playerVelocity;

    bool canCharge = true;

    [Header("Sound")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip deathClip;
    [SerializeField]
    private AudioClip SGModeSwitch;

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem movementParticle;
    [SerializeField]
    private ParticleSystem combatParticle;

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
        isBraking = false;
        isCharging = false;
        Bounced = false;
        ammoGauge.gameObject.SetActive(false);
        movementGauge.gameObject.SetActive(false);
        brakeIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        if (canMove)
        {
            if(playerCamera)
            {
                UpdateCamera();
                UpdatePlayer();    //We may need the space of the update function so we will choose to create functions
                ammoGauge.fillAmount = (float)ammo / (float)maxAmmo;
                movementGauge.fillAmount = (Time.time - chargeStartTime) / maxCharge;  
            }

            if (!dying)
            {
                UpdateHealth();
            }
        }
        else
        {
            movementGauge.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector3.zero;
        }
        playerVelocity = rb.velocity.magnitude;
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

                ammoGauge.gameObject.SetActive(true);
                //StartCoroutine(SgSwitchSound());

                movementGauge.gameObject.SetActive(false);
                movementParticle.gameObject.SetActive(false);
                isCharging = false;
                chargeStartTime = 0;
                chargeTimeTotal = 0;
                canCharge = false;
            }
            else if (currentMode == SuckGunMode.COMBAT)
            {
                currentMode = SuckGunMode.MOVEMENT;
                //Blue material
                SGMouth.material = movementModeMat;
                SGRing.material = movementModeMat;

                ammoGauge.gameObject.SetActive(false);
                //StartCoroutine(SgSwitchSound());
            }

            if (ammo == 0 && currentMode == SuckGunMode.COMBAT)
            {
                currentMode = SuckGunMode.MOVEMENT;
                //Blue material
                SGMouth.material = movementModeMat;
                SGRing.material = movementModeMat;


                ammoGauge.gameObject.SetActive(false);
                //StartCoroutine(SgSwitchSound());
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

                playerLocation.position = new Vector3(playerLocation.position.x, 7f, playerLocation.position.z);
                fullMovement.gameObject.SetActive(false);

                switch (currentMode)
                {
                    case SuckGunMode.MOVEMENT:

                        if (Input.GetMouseButtonDown(0) && isBraking == false)
                        {
                            ChargeUp();
                            movementGauge.gameObject.SetActive(true);
                            canCharge = true;
                        }

                        if (Input.GetMouseButtonUp(0) && isBraking == false && canCharge)
                        { 
                            ChargeRelease(Direction);  
                            movementGauge.gameObject.SetActive(false);
                        }

                        if(Input.GetMouseButtonUp(0) && isBraking == true)
                        {
                            movementGauge.gameObject.SetActive(false);
                            movementParticle.gameObject.SetActive(false);
                            chargeStartTime = 0;
                        }

                        if (Input.GetMouseButtonDown(1))
                        {
                            isBraking = true;
                            isCharging = false;
                            canCharge = false;
                            movementParticle.gameObject.SetActive(false);
                            movementGauge.gameObject.SetActive(false);
                            brakeIndicator.gameObject.SetActive(true);
                        }

                        if(Input.GetMouseButtonUp(1))
                        {
                            isBraking = false;
                            brakeIndicator.gameObject.SetActive(false);
                        }
                        break;

                    case SuckGunMode.COMBAT:

                        if (Input.GetMouseButtonDown(0))
                        {
                            combatParticle.gameObject.SetActive(true);
                            WaitForSeconds wait = new WaitForSeconds(0.1f);
                            combatParticle.gameObject.SetActive(false);
                            FireProjectile();
                            BulletReBound(Direction);
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            isBraking = true;
                            brakeIndicator.gameObject.SetActive(true);
                        }

                        if (Input.GetMouseButtonUp(1))
                        {
                            isBraking = false;
                            brakeIndicator.gameObject.SetActive(false);
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
        if(!isBraking)
        {
            isCharging = true;
            chargeStartTime = Time.time;
            movementParticle.gameObject.SetActive(true);
        }
    }
    private void ChargeRelease(Vector3 Direction)
    {
        //End the charge and reset the gauge
        if (!isBraking)
        {
            isCharging = false;
           
            if (chargeTimeTotal > maxCharge)
            {
                chargeTimeTotal = maxCharge;
            }

            chargeTimeTotal = Time.time - chargeStartTime;

            force = Mathf.Lerp(0f, maxForce, chargeTimeTotal);
            Vector3 oppositeDir = -Direction;
            rb.AddForce(oppositeDir * force, ForceMode.Impulse);
            movementParticle.gameObject.SetActive(false); 
        }
    }

    private void BulletReBound(Vector3 Direction)
    {
        Vector3 oppositeDir = -Direction;
        rb.AddForce(oppositeDir * rb.velocity.magnitude * recoilStrength, ForceMode.Impulse);
    }

    public void TakeDamage(int dmgAmount)
    {
        health -= dmgAmount;

        if(health > maxHealth)
        {
            health = maxHealth;
        }
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
    }

    private IEnumerator Death()
    {
        //audioSource.PlayOneShot(deathClip);
        dying = true;
        canMove = false;
        //animator.SetTrigger("Death");

        yield return new WaitForSeconds(respawnDelay);
        GameManager.Instance.PlayerDeathEvent();
    }

    private IEnumerator SgSwitchSound()
    {
        if (audioSource != null && SGModeSwitch != null)
        {
            audioSource.clip = SGModeSwitch;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
        {
            Debug.LogWarning("AudioSource or Clip is not assigned.");
        }
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
