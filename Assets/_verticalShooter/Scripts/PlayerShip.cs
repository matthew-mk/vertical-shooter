using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public Weapon currentWeapon;
    public int maxHealth = 3;
    public float maxSpeed = 3f;
    public float maxAcceleration = 0.002f;
    public float destinationArrivedThreshold = 0.003f;

    private int currentHealth;
    private float currentSpeed;
    private float accelerationCounter;

    //intermediary variables
    private Vector3 previousDirection;

    //consts
    private const float accelerationIncrease = 0.00003f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentSpeed = maxSpeed / 4f;
        accelerationCounter = maxAcceleration / 4f;
        previousDirection = Vector3.zero;
        currentWeapon.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentWeapon.Shoot();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 mosePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //transform.position = new Vector3(mosePos2D.x, mosePos2D.y, 0);
        //sample our input
        Vector3 dest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //ship goes to dest
        Vector3 fixedDest = new Vector3(dest.x, dest.y, 0f);
        if ((fixedDest - transform.position).magnitude < destinationArrivedThreshold)
        {
            currentSpeed = maxSpeed / 4f;
            accelerationCounter = maxAcceleration / 4f;
            return;
        }
        //float angleOfDirection = Vector3.Angle(fixedDest, previousDirection);
        //accelerationCounter = acceleration * Mathf.Cos(angleOfDirection);
        accelerationCounter += accelerationIncrease;
        if (accelerationCounter > maxAcceleration)
        {
            accelerationCounter = maxAcceleration;
        }
        currentSpeed += accelerationCounter;
        if (currentSpeed > maxSpeed)
            currentSpeed = maxSpeed;
        Vector3 moveDirection = (fixedDest - transform.position).normalized * currentSpeed;
        transform.Translate(moveDirection);

        previousDirection = fixedDest;
    }

    //get hit
    private void GetHit(int damageTaken)
    {
        currentHealth -= damageTaken;

        BusSystem.General.ShipGotHit(currentHealth, maxHealth);
        StartCoroutine(GetHitCoroutine());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        BusSystem.General.ShipGotDead(gameObject);
    }

    IEnumerator GetHitCoroutine()
    {
        float maxDuration = 0.2f;
        float currentDuration = 0f;
        while (currentDuration < maxDuration)
        {
            transform.localScale *= 1.01f;
            currentDuration += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        transform.localScale = Vector3.one;// new Vector(1f,1f,1f);
    }
}
