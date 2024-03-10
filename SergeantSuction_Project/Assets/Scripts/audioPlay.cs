using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioPlay : MonoBehaviour
{
    AudioSource sfx;

    // Start is called before the first frame update
    void Start()
    {   
        // Set 'sfx' as the audio source
        sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySound()
    {
        // Play the audio when called
        sfx.Play();
    }
}
