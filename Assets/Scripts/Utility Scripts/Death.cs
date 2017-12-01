using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
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
}
