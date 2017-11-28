using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            if (gameObject.transform.parent.tag == "Mine")
            {
                Destroy(gameObject.transform.parent.gameObject);
            }
            EventManager.TriggerEvent("PlayerDied");
        }
        
    }
}
