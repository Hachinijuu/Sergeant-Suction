using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool: MonoBehaviour
{
    [SerializeField]
    private GameObject pooledObject;
    [SerializeField]
    private int pooledAmount = 20;
    [SerializeField]
    private bool willGrow = true;

    private bool emptyPool = false;
    public bool EmptyPool
    {
        get { return emptyPool; }
    }

    public List<GameObject> pooledObjects;

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.transform.SetParent(gameObject.transform);
            obj.SetActive(false);

            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            pooledObjects.Add(obj);
            obj.transform.SetParent(gameObject.transform);
            return obj;
        }

        Debug.LogError("Object Pool for " + pooledObject.name + " is not big enough");
        emptyPool = true;

        return null;
    }

}

