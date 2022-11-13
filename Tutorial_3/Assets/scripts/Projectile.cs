using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour

{
    Rigidbody2D rigidbody2d;
    // Start is called before the first frame update
    void Awake()
    {
         rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
    rigidbody2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
    
      EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
        }
    
        Destroy(gameObject);

        

HardEnemyController he = other.collider.GetComponent<HardEnemyController>(); //This code is storing a reference to the EnemyController script, so you'd want a similar one for your HardEnemyController

    if (he != null)

    {

        he.Fix(); // this code is calling the "Fix" function in the EnemyController script when the projectile hits the Enemy, so you'd want a similar one for your HardEnemyController

    }

    Destroy(gameObject);



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
