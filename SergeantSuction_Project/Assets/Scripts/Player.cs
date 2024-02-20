using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    private enum PlayerMode { Movement, Combat };

    [SerializeField]
    private float playerHealth;

    [SerializeField]
    private PlayerMode currentMode;
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private float cameraDistance = 5f;

    //Movement Variables
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

    //bounce mechanic variables
    public bool Bounced = false;
    public Vector3 BounceVector = Vector3.zero;

    //Combat/Suction Variables
    private Transform fireLocation;

    [SerializeField]
    private GameObject projectilePrefab;

    private float maxAmmo = 10f;
    private float currentAmmo = 0f;

    [SerializeField]
    private float respawnDelay = 2.0f;

    public GameObject respawnLocation;

    private Renderer playerRenderer;
    private Color originalColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerLocation = GetComponent<Transform>();
        fireLocation = transform.Find("FireLocation");
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color;
        playerHealth = 100.0f;
    }

    void Update()
    {
        //uses the same basic move camera script that the original was
        MoveCamera();
        UpdateMode();
        //tracking the mouse position in relation to the player
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        //
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
                case PlayerMode.Movement:

                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCharge();
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        EndCharge();
                        float chargeTimeTotal = Time.time - chargeStartTime;
                        float normalizedCharge = Mathf.Clamp01(chargeTimeTotal / chargeTime);
                        float force = Mathf.Lerp(0f, maxForce, normalizedCharge);
                        currentForce = force;
                        Vector3 oppositeDir = -Direction;
                        rb.AddForce(oppositeDir * force * forceMultiplier, ForceMode.Impulse);
                    }

                    
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
                    

                    break;
                case PlayerMode.Combat:

                    if(Input.GetMouseButtonDown(0))
                    {
                        FireProjectile();
                    }
                    break;
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxForce);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
  
    }

    public void OnTriggerEnter(Collider other)
    {
        if (currentMode == PlayerMode.Movement)
        {
            if (other.CompareTag("AmmoRock"))
            {
                if (currentAmmo < maxAmmo)
                {
                    other.gameObject.SetActive(false);
                    currentAmmo++;
                }
            }
        }  
    }

    private void MoveCamera()
    {
        playerCamera.transform.position = new Vector3(playerLocation.position.x, cameraDistance, playerLocation.position.z);
    }

    private void StartCharge()
    {
        isCharging = true;
        chargeStartTime = Time.time;
    }

    private void EndCharge()
    {
        isCharging = false;
    }

    private void UpdateMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentMode == PlayerMode.Movement)
            {
                currentMode = PlayerMode.Combat;
            }
            else if (currentMode == PlayerMode.Combat)
            {
                currentMode = PlayerMode.Movement;
            }
            
        }
    }

    private void FireProjectile()
    {
        if(projectilePrefab && currentAmmo > 0)
        {
            Vector3 direction = transform.forward;
            direction.y = 0f;
            direction.Normalize();

            GameObject projectileGo = GameObject.Instantiate(projectilePrefab, fireLocation.position, Quaternion.identity);
            Projectile projScript = projectileGo.GetComponent<Projectile>();
            projectileGo.SetActive(true);
            projScript.Fire(direction);
            currentAmmo--;
        }
    }

    public void TakeDamage(int dmgAmount)
    {
        playerHealth -= dmgAmount;

        StartCoroutine("FlashRed");

        if(playerHealth <= 0)
        {
            DestroyPlayer();
        }
    }

    public void DestroyPlayer()
    {
        SetPlayerVisibility(false);
        StartCoroutine("Respawn");
    }

    IEnumerator FlashRed()
    {
        playerRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        playerRenderer.material.color = originalColor;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        SetPlayerVisibility(true);
        playerLocation.position = respawnLocation.transform.position;
    }

    private void SetPlayerVisibility(bool visible)
    {
        if(playerRenderer != null)
        {
            playerRenderer.enabled = visible;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 40), "Health " + playerHealth);
        GUI.Label(new Rect(10, 30, 400, 40), "Ammo " + currentAmmo);
        GUI.Label(new Rect(10, 50, 600, 40), "Mode " + currentMode);
    }

}
