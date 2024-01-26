using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SuckGun : MonoBehaviour
{
    [SerializeField]
    private float fireStrength = 5; 
    [SerializeField]
    private float movementSpeed = .5f;

    private float fireHoldTime = 0.0f;
    public float FireHoldTime
    {
        get { return fireHoldTime; }
        set { fireHoldTime = value; }
    }
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection;

        if (movementSpeed > 0.1f) { movementSpeed = movementSpeed - 0.2f * Time.deltaTime * movementSpeed; }
        else { movementSpeed = 0.1f; }
        moveDirection = transform.forward * movementSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + moveDirection);

    }
    public void FirePrimaryFire()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.cyan);

            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z));
        }
       
        movementSpeed = fireStrength * fireHoldTime;
    }
}
