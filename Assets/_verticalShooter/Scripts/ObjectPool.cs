using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private List<T> pooledObjects;
    private T initialObjectCopy;
    private int currentSize;
    private int maxSize;

    private GameObject poolParent;

    public ObjectPool(T objectToPool, int initialMaxSize, int maxAllowabaleSize = 50)
    {
        if(maxAllowabaleSize < initialMaxSize)
        {
            Debug.LogError("Max allowable size not allowed to be exceeded by initial max size of pool");
            return;
        }

        maxSize = maxAllowabaleSize;
        currentSize = initialMaxSize;
        initialObjectCopy = objectToPool;

        pooledObjects = new List<T>(initialMaxSize);
    }

    public void Init()
    {
        poolParent = new GameObject("_pool_" + initialObjectCopy.name);
        poolParent.transform.position = new Vector3(-1000f, 0f, 0f);
        for(int i = 0; i < currentSize; i++)
        {
            T tempObj = GameObject.Instantiate(initialObjectCopy, poolParent.transform);
            tempObj.gameObject.SetActive(false);
            pooledObjects.Add(tempObj);
        }
    }

    public T Instantiate()
    {
        foreach(T item in pooledObjects)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                item.gameObject.SetActive(true);
                return item;
            }
        }
        Debug.LogError("Our pool no longer has items");
        return null;
    }

    public T Instantiate(Transform parent)
    {
        T item = Instantiate();
        if (item == null)
            return null;
        item.gameObject.transform.SetParent(parent);
        return item;
    }

    public void Destroy(T objToDestroy)
    {
        foreach(T item in pooledObjects)
        {
            if(objToDestroy == item)
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.SetParent(poolParent.transform);
                item.gameObject.transform.localPosition = Vector3.zero;//new Vector3(0f,0f,0f)
                return;
            }
        }
        Debug.LogError("Item set to be destroyed not present in pool: " + "_pool_" + initialObjectCopy.name);
    }
}
