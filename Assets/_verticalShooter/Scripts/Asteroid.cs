using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Enemy
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0f, -speed * Time.deltaTime, 0f));
    }
}
