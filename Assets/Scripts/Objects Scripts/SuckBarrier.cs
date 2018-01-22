using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckBarrier : MonoBehaviour {

    private DigitalRuby.LightningBolt.LightningBoltScript lightning;

    void Start()
    {
        lightning = GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.player)  && collision.gameObject.GetComponentInChildren<GunController>().currentCharge > 0)
        {
            lightning.EndObject = collision.gameObject;
            lightning.Trigger();
            GetComponent<AudioSource>().Play();
            collision.gameObject.GetComponentInChildren<GunController>().currentCharge = 0;
        }

        if (collision.CompareTag(Tags.enemy) && collision.gameObject.GetComponentInChildren<EnemyWeapon>().currentCharge > 0)
        {
            lightning.EndObject = collision.gameObject;
            lightning.Trigger();
            GetComponent<AudioSource>().Play();
            collision.gameObject.GetComponentInChildren<EnemyWeapon>().currentCharge = 0;
        }
    }

}
