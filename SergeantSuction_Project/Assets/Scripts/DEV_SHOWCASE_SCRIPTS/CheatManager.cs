using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : MonoBehaviour
{
    [Header("Setup")]
    //[SerializeField]
    //private GameManager gameManager;
    [SerializeField]
    private Player player;
    [SerializeField]
    private GameObject playerGO;
    [SerializeField]
    private GameObject playerCamera;

    [Header("Cheat Buttons")]
    [SerializeField]
    private Button toSpawn;
    [SerializeField]
    private Button toMaze;
    [SerializeField]
    private Button maxAmmo;
    [SerializeField]
    private Button godMode;
    [SerializeField]
    private Button reloadGame;

    bool neverDepleteAmmo = false;
    bool godModeOn = false;

    public void ToSpawn()
    {
        playerGO.transform.position = GameManager.Instance.startRespawn.position;
    }

    public void ToMaze()
    {
        playerGO.transform.position = GameManager.Instance.endRespawn.position;
    }

    public void MaxAmmo()
    {
        if (neverDepleteAmmo)
        {
            neverDepleteAmmo = false;
        }
        else
        {
            neverDepleteAmmo = true;
        }
    }

    public void GodMode()
    {
        if (godModeOn)
        {
            godModeOn = false;
        }
        else
        {
            godModeOn = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (neverDepleteAmmo)
        {
            player.ammo = 10;
        }
        
        if (godModeOn)
        {
            player.health = 100;
        }
    }
}
