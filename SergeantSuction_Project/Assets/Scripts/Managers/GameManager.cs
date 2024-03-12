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
    private string mainMenuName;
    [SerializeField]
    private LoadingScreen loadingScreen;
    [SerializeField]
    private PauseMenu pauseMenu;
    [SerializeField]
    private GameOver gameOverScreen;
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

    [SerializeField]
    private string[] levelNames;

    private int currentLevel = 0;
    private string currentLevelName;

    //Code
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        sergeant = sergeantGO.GetComponent<Player>();
    }

    private void Update()
    {
        if(currentLevelName == mainMenuName)
        {
            sergeantGO.SetActive(false);
        }
    }

    private void Start()
    {
        ReturnToMainMenu();
        //Load Save Game
    }

    //Code for respawning the player
    public void PlayerDeathEvent()
    {
        gameOverScreen.gameObject.SetActive(true);
        pauseMenu.CanPause = false;
    }

    public void PlayerRespawn()
    {
        //LevelManager.Instance.Reset();
        sergeant.transform.position = startRespawn.position;
        sergeant.transform.rotation = startRespawn.rotation;
        
        sergeant.Reset();

        sergeant.gameObject.SetActive(true);
        pauseMenu.CanPause = true;
        gameOverScreen.gameObject.SetActive(false);
    }

    private IEnumerator LoadLevel(string levelName)
    {
        sergeant.gameObject.SetActive(false);

        loadingScreen.gameObject.SetActive(true);  // Turn on loading screen

        yield return new WaitForSeconds(.25f);

        if ((!string.IsNullOrEmpty(currentLevelName)))
        {
            //yield return SoundManager.Instance.StartCoroutine("UnLoadLevel");

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);

            while (!asyncUnload.isDone)
            {
                yield return null;
            }
        }


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            // Update the loading slider to match the progress of this process. 
            loadingScreen.UpdateSlider(asyncLoad.progress);
            yield return null;
        }

        yield return new WaitForSeconds(.75f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        //SoundManager.LevelLoadComplete();

        //if (LevelManager.Instance && (LevelManager.Instance.GetSpawnPoint() != null))
        //{
        //PlayerRespawn();
        //}

        PlayerRespawn();
        sergeant.Reset();

        currentLevelName = levelName;

        loadingScreen.gameObject.SetActive(false);  // Shut off loading screen
    }

    public void StartNewGame()
    {
        currentLevel = 0;
        StartCoroutine(LoadLevel(levelNames[currentLevel]));
    }

    public void ContinueGame()
    {

    }

    public void GameComplete()
    {
        currentLevel++;
        if (currentLevel < levelNames.Length)
        {
            //load level coroutine
        }
        else
        {
            //can't pause pause menu
            //victoryScreen.gameObject.SetActive(true);
        }
        //SaveGame();
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadLevel(mainMenuName));
        pauseMenu.CanPause = false;
        sergeantGO.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);
        //victoryScreen.gameObject.SetActive(false);
    }
}
