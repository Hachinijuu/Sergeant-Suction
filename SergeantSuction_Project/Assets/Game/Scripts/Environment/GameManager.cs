using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    private List<ResetBehaviour> registeredForReset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        registeredForReset = new List<ResetBehaviour>();
    }

    public static void RegisterForReset(ResetBehaviour resetBehaviour)
    {
        Instance.registeredForReset.Add(resetBehaviour);
    }
    
    public static void GameReset()
    {
        for(int i =0; i < instance.registeredForReset.Count; i ++)
        {
            Instance.registeredForReset[i].Reset();
        }
    }
}
