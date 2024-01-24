using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResetBehaviour : MonoBehaviour
{
    protected void RegisterForReset()
    {
        GameManager.RegisterForReset(this);
    }

    private void Start()
    {
        RegisterForReset();
    }

    public abstract void Reset();  
}
