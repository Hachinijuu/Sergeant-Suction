//#-----------------------------------------------------
//# Script:     Lock.cs
//#
//# Author:     Mark Proveau (Professor)
//# Course:     GAME1204 - Niagara College - Game Dev Program
//#-----------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    [HideInInspector]
    public LockedObject parent;
    [HideInInspector]
    public TextMesh keysRemaining;

    public List<GameObject> objectsToDisable = new List<GameObject>();
    private void Start()
    {
        keysRemaining = GetComponentInChildren<TextMesh>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(parent)
        {
            parent.TriggerEnterEvent(other);
        }
    }
    public void OpenChest()
    {
        foreach(var v in objectsToDisable)
        {
            v.SetActive(false);
        }
        GetComponent<Collider>().enabled = false;
    }
}
