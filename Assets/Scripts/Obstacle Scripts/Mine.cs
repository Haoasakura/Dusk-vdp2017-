using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            EventManager.TriggerEvent("PlayerDied");
        }
        
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            Destroy(collision.gameObject);
        }

        Explode();
    }

    private void Explode()
    {
        Destroy(transform.parent.gameObject);
        return;
    }

}
