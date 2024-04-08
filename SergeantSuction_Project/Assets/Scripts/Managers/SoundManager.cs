using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource musicAudioSource;

    [SerializeField]
    private AudioMixer mainMixer;

    private float masterVol = 0f;
    private float musicVol = 0f;
    private float sfxVol = 0f;

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        
        instance = this;
        masterVol = GameManager.Instance.NewMasterVolume;
        musicVol = GameManager.Instance.NewMusicVolume;
        sfxVol = GameManager.Instance.NewEffectsVolume;
        
    }


    public static void LevelLoadComplete()
    {
        
        if (GameManager.Instance && GameManager.Instance.LevelMusic != null)
        {
            AudioClip levelMusic = GameManager.Instance.LevelMusic;

            if (levelMusic)
            {
                instance.musicAudioSource.clip = levelMusic;
                instance.musicAudioSource.Play();
            }

            instance.AudioFadeLevelStart();
        }
        
    }

    private void  AudioFadeLevelStart()
    {
        
        masterVol = GameManager.Instance.NewMasterVolume;
        musicVol = GameManager.Instance.NewMusicVolume;
        sfxVol = GameManager.Instance.NewEffectsVolume;
        
        instance.StartCoroutine(LerpVolume(-80, masterVol, .5f));
    }

    public IEnumerator UnLoadLevel()
    {
        yield return LerpVolume(masterVol, -80, .5f);
    }

    private IEnumerator LerpVolume(float startVol, float endVol, float time)
    {
        float currVol = startVol;
        float currentTime = 0;
        while (currentTime < time)
        {
            currentTime += Time.deltaTime;
            currentTime = Mathf.Clamp(currentTime, 0, time);

            currVol = Mathf.Lerp(startVol, endVol, currentTime / time);
            instance.mainMixer.SetFloat("masterVolume", currVol);
            yield return null;
        }
    }


}

