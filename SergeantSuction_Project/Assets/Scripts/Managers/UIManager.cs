using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }

            if (!instance)
            {
                Debug.LogError("No UI Manager Present !!!");
            }

            return instance;

        }
    }

    [SerializeField]
    private GameObject gameHUD;

    [Header("HUD Settings")]
    [SerializeField]
    private GameObject[] livesImages;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (gameHUD)
        {
            if (livesImages.Length > 0)
            {
                for (int i = 0; i < livesImages.Length; i++)
                {
                    if (GameManager.Instance.PlayerLives > i)
                    {
                        livesImages[i].SetActive(true);
                    }
                    else
                    {
                        livesImages[i].SetActive(false);
                    }
                }
            }
        }
    }

    /*
    public void ShowUITrigger(GameObject uiElement)
    {
        uiElement.SetActive(true);
    }

    public void HideUITrigger(GameObject uiElement)
    {
        uiElement.SetActive(false);
    }
    */
}
