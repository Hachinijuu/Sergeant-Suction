using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button newGameButton;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button exitButton;

    public void OnEnable()
    {
        //continueButton.interactable = GameManager.Instance.SaveGamePresent;
    }

    public void Credits()
    {
        GameManager.Instance.Credits();
    }

    public void NewGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void ExitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
