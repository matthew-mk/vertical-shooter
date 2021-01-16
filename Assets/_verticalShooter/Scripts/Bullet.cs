using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public Action<Bullet> OnCleanSelf;

    public int damage = 1;
    public float speed;
    public float lifeTime = 3f;
    protected Vector3 normalizedDirection;
    protected float lifeTimeCounter;

    public virtual void Init(Vector3 direction)
    {
        this.normalizedDirection = direction.normalized;
        lifeTimeCounter = 0f;
    }

    protected virtual void HitEnemy(Vector3 impactPosition)
    {
        Debug.Log("Hit Enemy");
        BusSystem.Effects.BulletHit(impactPosition);
    }

    protected void CleanSelf()
    {
        OnCleanSelf?.Invoke(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            HitEnemy(transform.position);
            collision.SendMessage("GetHit", this);           
        }
    }
}
