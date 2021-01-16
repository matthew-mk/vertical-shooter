using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBullet : MonoBehaviour
{
    public Action<EnemyBullet> OnCleanSelf;

    public int damage = 1;
    public float speed;
    public float lifeTime = 3f;

    private Vector3 normalizedDirection;
    private float lifeTimeCounter;

    //monobehaviour functions
    private void Update()
    {
        transform.Translate(normalizedDirection * speed * Time.deltaTime);
        lifeTimeCounter += Time.deltaTime;
        if (lifeTimeCounter > lifeTime)
        {
            CleanSelf();
        }
    }

    //public functions
    public void Init(Vector3 direction)
    {
        this.normalizedDirection = direction.normalized;
        lifeTimeCounter = 0f;
    }
    //private functions
    private void CleanSelf()
    {
        OnCleanSelf?.Invoke(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.SendMessage("GetHit", damage);
            CleanSelf();
        }
    }
}
