using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootyWeaponButCooler : Weapon
{
    public AudioClip magnumSound;
    public MuzzleFlashParticle muzzleFlashPrefab;
    public Bullet bulletPrefab;
    public int weaponLevel = 1;
    private ObjectPool<Bullet> bulletPool;

    private List<Bullet> activeBullets;

    private ObjectPool<MuzzleFlashParticle> muzzleFlashPool;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Init()
    {
        //object pool inits
        bulletPool = new ObjectPool<Bullet>(bulletPrefab, 200, 200);
        bulletPool.Init();
        muzzleFlashPool = new ObjectPool<MuzzleFlashParticle>(muzzleFlashPrefab, 10, 10);
        muzzleFlashPool.Init();

        weaponLevel = 1;
        activeBullets = new List<Bullet>(200);
    }

    public override void Shoot()
    {
        base.Shoot();
        StartCoroutine(ShootCoroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            weaponLevel++;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            weaponLevel--;
            if (weaponLevel < 1)
                weaponLevel = 1;
        }
    }

    IEnumerator ShootCoroutine()
    {
        var muzzleFlash = muzzleFlashPool.Instantiate();
        muzzleFlash.transform.SetParent(this.transform);
        muzzleFlash.transform.localPosition = Vector3.zero;
        StartCoroutine(CleanUpMuzzleFlash(muzzleFlash));

        audioSource.clip = magnumSound;
        audioSource.pitch = Random.Range(0.3f, 1.8f);
        audioSource.Play();

        //number of bullets is 2*(weapon level) - 1
        for (int i = -(2 * weaponLevel - 1) / 2; i <= (2 * weaponLevel - 1) / 2; i++)
        {
            var bullet = bulletPool.Instantiate();
            Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (i * 10)), Mathf.Cos(Mathf.Deg2Rad * (i * 10)), 0f);
            bullet.transform.position = transform.position;
            bullet.Init(direction);
            activeBullets.Add(bullet);
            bullet.OnCleanSelf = (Bullet bulletToBeCleaned) =>
            {
                activeBullets.Remove(bullet);
                bulletPool.Destroy(bullet);
            };
            yield return new WaitForSeconds(0.04f);
        }
    }

    IEnumerator CleanUpMuzzleFlash(MuzzleFlashParticle muzzleFlashToClean)
    {
        yield return new WaitForSeconds(1f);
        muzzleFlashPool.Destroy(muzzleFlashToClean);
    }
}
