//#-----------------------------------------------------
//# Script:     KeyEntity.cs
//#
//# Author:     Mark Proveau (Professor)
//# Course:     GAME1204 - Niagara College - Game Dev Program
//#-----------------------------------------------------
using UnityEngine;

public class KeyEntity : MonoBehaviour
{
    public Vector3 startingPos;
    public Quaternion startingRot;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        startingRot = transform.rotation;
    }
}
