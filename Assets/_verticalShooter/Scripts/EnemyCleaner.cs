using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCleaner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy en = collision.gameObject.GetComponent<Enemy>();
        if(en != null)
        {
            BusSystem.General.DestroyEnemy(en);
        }
    }
}
