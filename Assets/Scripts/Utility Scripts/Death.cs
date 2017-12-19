using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    public bool fallCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!fallCollider)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                EventManager.TriggerEvent("PlayerDied");
            }
            else if (collision.gameObject.tag.Equals("Enemy"))
            {
                Destroy(collision.gameObject);
            }
        }
        else
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                EventManager.TriggerEvent("PlayerDiedFromFall");
            }
            else if (collision.gameObject.tag.Equals("Enemy"))
            {
                Destroy(collision.gameObject);
            }
        }


    }
}
