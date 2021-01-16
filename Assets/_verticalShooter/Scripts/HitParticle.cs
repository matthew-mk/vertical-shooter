using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitParticle : MonoBehaviour
{
    public Action OnCleanMe;
    public float lifeTime = 1f;

    private float lifeTimeCounter = 0f;
    private bool cleanUpFlag;

    void OnEnable()
    {
        lifeTimeCounter = 0f;
        cleanUpFlag = false;
    }

    private void Update()
    {
        lifeTimeCounter += Time.deltaTime;
        if(lifeTimeCounter > lifeTime)
        {
            if (!cleanUpFlag)
            {
                OnCleanMe?.Invoke();
                cleanUpFlag = true;
            }            
        }
    }
}
