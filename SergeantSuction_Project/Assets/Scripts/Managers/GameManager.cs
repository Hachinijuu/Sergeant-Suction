using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using Unity.VisualScripting;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    //Instance declaration
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            if (!instance)
            {
                Debug.LogError("No Game Manager Present !!!");
            }

            return instance;

        }
    }

    [SerializeField]
    bool devMenu = false;

    [Header("Menus")]
    [SerializeField]
    private GameObject loadingScreen;

    /*
    [SerializeField]
    private PauseMenu pauseMenu;
    [SerializeField]
    private GameOverScreen gameOverScreen;
    */

    //private WinScreen winScreen;

    [Header("Player")]
    [SerializeField]
    private Player sergeant;

    [SerializeField]
    private GameObject sergeantGO;
    public GameObject SergeantGO
    {
        get { return sergeantGO; }
    }

    [Header("Respawn Points")]
    [SerializeField]
    private Transform startRespawn;
    [SerializeField]
    private Transform midwayRespawn;
    [SerializeField]
    private Transform endRespawn;
    //These respawn locations will be assigned into the GM and then flags will determine what respawnPoint equals to.
    private Transform respawnPoint;

    [Header("Audio")]
    private AudioMixer audioManager;

    //Code
    private void Awake()
    {
        
    }
    //Code for respawning the player
    public void PlayerDeathEvent()
    {
        //Show the "You've Died" screen
        PlayerRespawn();
    }

    public void PlayerRespawn()
    {
        sergeant.CanMove = false;
        sergeantGO.transform.position = startRespawn.position;
        sergeantGO.transform.rotation = startRespawn.rotation;
        
        sergeant.Reset();
        //LevelManager.Instance.Reset();
        sergeant.gameObject.SetActive(true);
        //pauseMenu.CanPause = true;
        //gameOverScreen.gameObject.SetActive(false);
    }
}
