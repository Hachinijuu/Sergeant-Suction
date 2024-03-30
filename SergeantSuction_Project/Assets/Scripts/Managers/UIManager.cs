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
    private Player sergeant;

    [SerializeField]
    private GameObject gameHUD;

    [SerializeField]
    private GameObject[] livesImages;

    [SerializeField]
    private Slider healthSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        healthSlider.maxValue = 100.0f;
        healthSlider.value = sergeant.Health;
    }

    void Update()
    {
        UpdateHUD();
        healthSlider.value = sergeant.Health;
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
}