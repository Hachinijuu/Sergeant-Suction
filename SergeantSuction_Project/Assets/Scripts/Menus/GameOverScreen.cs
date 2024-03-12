using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void PlayAgain()
    {
        GameManager.Instance.PlayerRespawn();
    }

    public void DontPlayAgain()
    {
        //GameManager.Instance.ReturntoMainMenu();
    }
}
