using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    //This script uses object pooling to handle player after image when dashing.

    [SerializeField] private GameObject afterImagePrefab;
    private Queue<GameObject> availableObjects = new Queue<GameObject>(); //Store all objects

    public static PlayerAfterImagePool Instance { get; private set; } //Singelton reference

    private void Awake()
    {
        Instance = this;
        GrowPool(); //Start game with a readied pool.
    }

    private void GrowPool() //Increase number of objects in the pool.
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab, transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance) //Return an object that we finished using to the pool.
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0) //If there is not enough objects in the pool, grow it.
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue(); //Take the first object from the queue
        instance.SetActive(true);

        return instance;
    }
}