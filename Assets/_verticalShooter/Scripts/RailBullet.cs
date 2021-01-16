using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailBullet : Bullet
{
    private ParticleSystem ps;
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
        SpawnRaycast(new Vector2(direction.x, direction.y));
    }

    public void SpawnRaycast(Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction);

        if(hits == null)
        {
            Debug.Log("Nothing detected");
            return;
        }

        foreach(var hit in hits)
        {            
            if (hit.collider.tag == "Enemy")
            {
                HitEnemy(hit.transform.position);
                hit.collider.SendMessage("GetHit", this);
            }
        }
    }
}
