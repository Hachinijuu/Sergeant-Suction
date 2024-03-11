using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuCanvas;

    public bool CanPause
    {
        get { return CanPause; }
        set { CanPause = value; }
    }

    private bool canPause = false;

    bool isPaused = false;

    private void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (canPause)
                Pause(!isPaused);
        }
    }

    private void Pause(bool pause)
    {
        isPaused = pause;
        pauseMenuCanvas.SetActive(isPaused);
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
