using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootyBullet : Bullet
{
    private void Update()
    {
        transform.Translate(normalizedDirection * speed * Time.deltaTime);
        lifeTimeCounter += Time.deltaTime;
        if (lifeTimeCounter > lifeTime)
        {
            CleanSelf();
        }
    }

    public override void Init(Vector3 direction)
    {
        base.Init(direction);
    }

    protected override void HitEnemy(Vector3 impactPosition)
    {
        base.HitEnemy(impactPosition);
        //add explosion
        CleanSelf();
    }
}
