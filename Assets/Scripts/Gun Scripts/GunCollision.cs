using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCollision : MonoBehaviour {

    public GameObject aimSight;
    public SpriteRenderer gunShadow;
    private SpriteRenderer gun;

    private void Start()
    {
        gun = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer.Equals("8"))
        {

            aimSight.SetActive(false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer.Equals("8"))
        {
            aimSight.SetActive(true);
        }
    }
}
