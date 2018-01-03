using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {

    public GameObject explosion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            EventManager.TriggerEvent("PlayerDied");
        }
        
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            collision.GetComponent<EnemyController>().Kill();
        }

        Explode();
    }

    private void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(transform.parent.gameObject);
        return;
    }

}
