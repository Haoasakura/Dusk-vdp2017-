using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGun : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag(Tags.player)) {
            //GameObject gun = collision.GetComponent<Player>().gun;
            GunController gunController = collision.GetComponent<Player>().gun.GetComponent<GunController>();
            if(!gunController.hasGun) {
                gunController.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 255f);
                gunController.gunShadow.color = new Color(255f, 255f, 255f, 255f);
                gunController.GetComponent<LineRenderer>().enabled = true;
                gunController.mLineRenderer.enabled = true;
                gunController.dotsight.GetComponent<SpriteRenderer>().enabled = true;
                transform.gameObject.SetActive(false);
                gunController.hasGun = true;
            }
        }
    }
}
