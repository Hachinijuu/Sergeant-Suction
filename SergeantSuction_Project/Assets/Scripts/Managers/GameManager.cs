using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using Unity.VisualScripting;

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
    private GameObject pauseMenu;
    private GameObject gameOverScreen;
    private GameObject winScreen;

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

    //Code
    private void Awake()
    {
        //Remove
        respawnPoint = startRespawn;

        //Every time the game starts, the player will be trasported to the last respawn point
        sergeantGO.transform.position = respawnPoint.position;
        sergeantGO.transform.rotation = respawnPoint.rotation;



        
    }

    //Code for respawning the player
    public void PlayerDeathEvent()
    {
        //MenuManager.Instance.ShowDeathScreen();
        PlayerRespawn();
    }

    public void PlayerRespawn()
    {
        sergeantGO.transform.position = respawnPoint.position;
        sergeantGO.transform.rotation = respawnPoint.rotation;
        sergeant.Reset();
        //LevelManager.Instance.Reset();
        sergeantGO.SetActive(true);
        //pauseMenu.CanPause = true;
        //gameOverScreen.gameObject.SetActive(false);
    }
}
