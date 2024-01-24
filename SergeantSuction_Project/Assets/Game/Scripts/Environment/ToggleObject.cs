using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : ResetBehaviour
{
    // List of GameObjects to be toggled on/off
    public List<GameObject> objectsToDisable = new List<GameObject>();
    public string triggeringObjectTag = "Player";
    // Flag to determine if the objects should be disabled at the start
    public bool startDisabled = false;

    public bool permanentToggle = false;


    [Tooltip("Enter an optional delay before toggling")]
    public float delayTime = 0f;

    public Material onMaterial, offMaterial;
    MeshRenderer rend;
    // Flag to track if the trigger has been activated
    bool triggerUsed = false;
    bool objectsActive = false;
     

    // Start is called before the first frame update
    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        DoToggle(!startDisabled);
    }
    public override void Reset()
    {
        //Debug.Log("Toggler Reset");
        if (permanentToggle)
        {
            return;
        }

        triggerUsed = false;
        DoToggle(!startDisabled);
    }
    void DoToggle(bool turnOn)
    {
        objectsActive = turnOn;

        if (rend)
            GetComponent<MeshRenderer>().material = turnOn ? offMaterial : onMaterial;

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(turnOn); // Enable if inactive            
        }
    }
    // Called when another object enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Skip if already activated
        if (triggerUsed || (triggerUsed && permanentToggle))
            return;

        // Check if the entering object is tagged as "Player"
        if (other.CompareTag(triggeringObjectTag))
        {
            // Set the trigger as activated
            triggerUsed = true;
            Invoke("ToggleObjectsDelayed", delayTime);
        }
    }
    void ToggleObjectsDelayed()
    {
        DoToggle(!objectsActive);
    }

    // Called when another object exits the trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Skip if not activated
        if (!triggerUsed)
            return;

        // Check if the exiting object is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Wait for a short duration before flagging the trigger to be usable again
            if (!permanentToggle)
            {
                Invoke("ResetTriggerStatus", 0.2f);
            }
        }
    }

    // Method to reset the trigger status
    void ResetTriggerStatus()
    {
        triggerUsed = false;
    }

    // Called when the script is enabled
    private void OnEnable()
    {
        // Reset the trigger status on script enable
        ResetTriggerStatus();
    }
}
