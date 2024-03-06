using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

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

    //Variables
    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private Player sergeant;

    [SerializeField]
    private GameObject sergeantGO;
    public GameObject SergeantGO
    {
        get { return sergeantGO; }
    }

    [SerializeField]
    private Transform respawnPoint;

    //Code

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
