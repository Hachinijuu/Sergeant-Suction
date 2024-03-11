using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button continueButton;

    public void OnEnable()
    {
        //continueButton.interactable = GameManager.Instance.SaveGamePresent;
    }

    public void ContinueGame()
    {
        GameManager.Instance.ContinueGame();
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
