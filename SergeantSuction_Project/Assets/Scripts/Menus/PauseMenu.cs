using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuGO;

    private bool canPause = false;
    public bool CanPause
    {
        get { return canPause; }
        set { canPause = value; }
    }

    bool isPaused = false;

    private void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            Pause(!isPaused);
        }
    }

    private void Pause(bool pause)
    {
        isPaused = pause;
        pauseMenuGO.SetActive(isPaused);
        if(isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

}
