using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetMoveToPoints : MonoBehaviour
{
    [SerializeField]
    List<MoveToPoints> moveToPoints;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (moveToPoints.Count > 0)
            {
                for(int i = 0; i < moveToPoints.Count; i++)
                {
                    moveToPoints[i].Reset();
                }
            }
        }
    }
}
