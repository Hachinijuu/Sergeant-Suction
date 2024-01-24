//#-----------------------------------------------------
//# Script:     LockedObject.cs
//#
//# Author:     Mark Proveau (Professor)
//# Course:     GAME1204 - Niagara College - Game Dev Program
//#-----------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedObject : MonoBehaviour
{
    // Reference to the chest object
    public Lock lockedObject;

    //prefab ref for respawning keys
    public GameObject keyPrefab;

    // Prefab to spawn when chest is opened (optional)
    public List<GameObject> spawnPrefabs = new List<GameObject>();

    // TextMesh to display the number of keys required
    public TextMesh keysRequiredText;

    // Audio clips
    public AudioClip keyUseSound;
    public AudioClip chestOpenSound;

    // Audio source to play the sounds
    private AudioSource audioSource;

    //track keys brought to the chest
    private int collectedKeys;
    private int totalKeys;

    private bool isChestOpen = false;

    // List of key bounds objects with trigger colliders
    private List<GameObject> keyBounds = new List<GameObject>();
    // List to store all available keys
    private List<GameObject> availableKeys = new List<GameObject>();
    //queue for tracking keys taht need to be respawned
    private Queue<GameObject> keysforRespawn = new Queue<GameObject>();

    private void Start()
    {
        //set up locked object
        if (lockedObject.TryGetComponent(out Lock lok))
        {
            lockedObject = lok;
            lockedObject.parent = this;
            keysRequiredText = lockedObject.keysRemaining;
            audioSource = lockedObject.GetComponent<AudioSource>();                
        }
        else
            Debug.LogError("From Mark!: Your lockedObject needs Lock.cs attached");

        //set up key boundary
        KeyBounds[] bounds = GetComponentsInChildren<KeyBounds>(false);
        foreach (KeyBounds g in bounds)
        {
            g.GetComponent<MeshRenderer>().enabled = false;
            keyBounds.Add(g.gameObject);
        }

        //create list of keys
        KeyEntity[] temp = GetComponentsInChildren<KeyEntity>(false);
        foreach (KeyEntity k in temp)
        {
            if (!CheckKeyInBounds(k.gameObject))
                keysforRespawn.Enqueue(k.gameObject);
            else
                availableKeys.Add(k.gameObject);
        }
        //destroy keys startign outside bounds
        while (keysforRespawn.Count > 0)
            Destroy(keysforRespawn.Dequeue());

        totalKeys = availableKeys.Count;

        // Set the initial text
        UpdateKeysRequiredText();
    }

    public void TriggerEnterEvent(Collider other)
    {
        // Check if the colliding object is a key, the chest is not open, and it's one of the available keys
        if (!isChestOpen && availableKeys.Contains(other.gameObject))
        {
            // Deactivate the key
            availableKeys.Remove(other.gameObject);
            Destroy(other.gameObject);

            // Add the key to the collected keys list
            collectedKeys++;

            // Check if all keys are collected
            if (collectedKeys == totalKeys)
            {
                OpenChest();
            }
            else
            {
                // Play key use sound only if the chest is not opening
                if (keyUseSound != null)
                {
                    audioSource.PlayOneShot(keyUseSound);
                }

                // Update the text to show the remaining keys required
                UpdateKeysRequiredText();
            }
        }
    }

    private void UpdateKeysRequiredText()
    {
        // Update the text to show the remaining keys required
        if (keysRequiredText) keysRequiredText.text = $"{collectedKeys}/{totalKeys}";
    }

    private void OpenChest()
    {
        // Set the chest as open
        isChestOpen = true;

        // Optionally spawn the prefab when the chest is opened
        if (spawnPrefabs.Count > 0)
        {
            foreach (GameObject g in spawnPrefabs)
            {
                Instantiate(g, lockedObject.transform.position, Quaternion.identity);
            }
        }

        // Play chest open sound
        if (chestOpenSound != null)
        {
            audioSource.PlayOneShot(chestOpenSound);
        }

        // Deactivate the chest object
        lockedObject.OpenChest();

        // Deactivate the keys required text
        if (keysRequiredText) keysRequiredText.gameObject.SetActive(false);
    }

    private void Update()
    {
        while (keysforRespawn.Count > 0)
        {
            GameObject key = keysforRespawn.Dequeue();
            //save position data and remove key from list of available keys
            availableKeys.Remove(key);
            Vector3 pos = key.GetComponent<KeyEntity>().startingPos;
            Quaternion rot = key.GetComponent<KeyEntity>().startingRot;

            //destroy they key
            Destroy(key);

            //create a new key at the destroyed keys start position and rotation
            GameObject temp = Instantiate(keyPrefab, pos, rot);
            availableKeys.Add(temp);

        }
        // Check for keys outside of bounds and respawn them
        foreach (var key in availableKeys)
        {
            // If the key is not in any of the bounds, respawn it
            if (!CheckKeyInBounds(key))
            {
                keysforRespawn.Enqueue(key);
            }
        }
    }

    private bool CheckKeyInBounds(GameObject key)
    {
        foreach (var bound in keyBounds)
        {
            if (bound.GetComponent<Collider>().bounds.Contains(key.transform.position))
                return true;
        }

        return false;
    }
}
