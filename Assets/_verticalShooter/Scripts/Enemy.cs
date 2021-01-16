using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHitPoints;
    protected PlayerShip playerShip;
    protected float xBoundaryLeft;
    protected float xBoundaryRight;
    protected bool isDeactivated;

    private int hitPoints;

    public enum EnemyType
    {
        Asteroid = 0,
        Enemy1 = 1
    }
    public EnemyType enemyType;

    private void Awake()
    {
        OnCreate();
    }

    //custom functions
    protected virtual void OnCreate()
    {

    }

    public virtual void Init(float leftBound, float rightBound, PlayerShip playerShip)
    {
        this.playerShip = playerShip;
        xBoundaryLeft = leftBound;
        xBoundaryRight = rightBound;
        hitPoints = maxHitPoints;
        isDeactivated = true;
    }

    public void Activate()
    {
        isDeactivated = false;
    }

    public virtual void Stimulate()
    {

    }

    public void GetHit(Bullet bulletInfo)
    {
        hitPoints -= bulletInfo.damage;
        if(hitPoints <= 0)
        {
            KillSelf();
        }
    }

    private void KillSelf()
    {
        BusSystem.General.DestroyEnemy(this);
    }
}
