using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    public bool fallCollider;
    public bool explosionCollider = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!fallCollider)
        {
            if (explosionCollider)
            {
                if (collision.gameObject.tag.Equals("Player"))
                {
                    EventManager.TriggerEvent("PlayerDied");
                }
                else if (collision.gameObject.tag.Equals("Enemy"))
                {
                    collision.GetComponent<EnemyController>().Kill();
                }
            }
            else
            {
                if (collision.gameObject.tag.Equals("Player"))
                {
                    EventManager.TriggerEvent("PlayerFallApart");
                }
                else if (collision.gameObject.tag.Equals("Enemy"))
                {
                    collision.GetComponent<EnemyController>().Kill();
                }
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
                StartCoroutine(DestroyFromFall(collision));
            }
        }
    }

    private IEnumerator DestroyFromFall(Collider2D collision)
    {
        if (collision.GetComponent<EnemyController>().controlled)
        {
            collision.GetComponent<EnemyController>().controlled = false;
            EventManager.TriggerEvent("EnemyDestroyed");
            Player mPlayer = GameObject.Find("Player").GetComponent<Player>();
            if (mPlayer != null)
            {
                mPlayer.controlling = false;
                mPlayer.GetComponentInChildren<GunController>().isLocked = false;
            }
        }
        SoundManager.Instance.PlayFallSound();
        yield return new WaitForSeconds(1);
        if(collision)
            collision.GetComponent<EnemyController>().Kill();
    }
}
