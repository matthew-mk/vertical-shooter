using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip1 : Enemy
{
    public float speed = 3;
    public float directionChangeDuration = 2f;
    public EnemyBullet bulletPrefab;

    private float directionChangedCounter = 0f;

    private float moveDirection = -1f;

    private ObjectPool<EnemyBullet> bulletsPool;

    //monobehaviour functions
    private void Update()
    {
        if (isDeactivated) return;
        transform.Translate(moveDirection * speed * Time.deltaTime, 0f, 0f);
        directionChangedCounter += Time.deltaTime;
        if (directionChangedCounter > directionChangeDuration)
        {
            directionChangedCounter = 0f;
            moveDirection *= -1f;
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        bulletsPool = new ObjectPool<EnemyBullet>(bulletPrefab, 50);
        bulletsPool.Init();
    }

    public override void Init(float leftBound, float rightBound, PlayerShip playerShip)
    {
        base.Init(leftBound, rightBound, playerShip);
        directionChangedCounter = directionChangeDuration / 2;
        moveDirection = -1f;
    }

    public override void Stimulate()
    {
        base.Stimulate();
        FireWeapon();
    }

    private void FireWeapon()
    {
        var bullet = bulletsPool.Instantiate(transform.root);
        bullet.transform.position = transform.position;
        bullet.Init(Vector3.down);
        bullet.OnCleanSelf = (enBullet) => 
        {
            bulletsPool.Destroy(enBullet);
        };
    }
}
