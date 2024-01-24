using UnityEngine;

public class RotatingObject : ResetBehaviour
{
    // Selectable rotation axis
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Rotation Settings")]
    [Tooltip("Speed of rotation")]
    // Exposed speed variable
    public float rotationSpeed = 30f;

    [Tooltip("Axis around which the object will rotate")]
    // Exposed rotation axis
    public RotationAxis axis = RotationAxis.Y;

    private Rigidbody rb;
    private Quaternion startRotation;

    // Ensure a Rigidbody is present and set it as kinematic
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            // If there is no Rigidbody, add one and set it as kinematic
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // Store the starting rotation
        startRotation = transform.rotation;
    }

    public override void Reset()
    {
        // Reset the object's rotation to its starting rotation
        transform.rotation = startRotation;
        rb.Sleep();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Rotate the object based on the selected axis and speed
        switch (axis)
        {
            case RotationAxis.X:
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationSpeed * Time.deltaTime, 0, 0));
                break;
            case RotationAxis.Y:
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0));
                break;
            case RotationAxis.Z:
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime));
                break;
        }
    }
}