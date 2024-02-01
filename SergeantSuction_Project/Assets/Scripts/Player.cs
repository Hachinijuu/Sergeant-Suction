using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private float cameraDistance = 5f;
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

    public bool Bounced = false;
    public Vector3 BounceVector = Vector3.zero;

    private float currentForce = 0.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerLocation = GetComponent<Transform>();
    }

    void Update()
    {
        //uses the same basic move camera script that the original was
        MoveCamera();

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
            

            float lookAng = Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg;
            playerLocation.rotation = Quaternion.Euler(0f, lookAng, 0f);
            rb.rotation = Quaternion.Euler(0f, lookAng, 0f);

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
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxForce);

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
}
