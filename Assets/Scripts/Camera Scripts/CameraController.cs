using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float offsetX;
    public float offsetY;

    private float newX;
    private float newY;
    private float newZ;
    private float collX;
    private float collY;
    private BoxCollider2D coll;

    private void OnEnable() {
        coll = GetComponent<BoxCollider2D>();
        collX = coll.size.x;
        collY = coll.size.y;
        ActivateEnemies();
    }

    private void OnTriggerExit2D(Collider2D collision) {

        newX = transform.position.x;
        newY = transform.position.y;
        newZ = transform.position.z;

        if (collision.CompareTag(Tags.player)) {
            Transform current = collision.transform;

            //Traslazioni nel caso il giocatore esca dal collider
            if ((Mathf.Abs(transform.position.x - current.position.x) > collX / 2) && Mathf.Sign(transform.position.x - current.position.x) < 0)
            {
                newX = transform.position.x + collX - offsetX;
            }
            else if ((Mathf.Abs(transform.position.x - current.position.x) > collX / 2) && Mathf.Sign(transform.position.x - current.position.x) > 0)
            {
                newX = transform.position.x - collX + offsetX;
            }
            else if ((Mathf.Abs(transform.position.y - current.position.y) > collY / 2) && Mathf.Sign(transform.position.y - current.position.y) < 0)
            {
                newY = transform.position.y + collY - offsetY;
            }
            else if ((Mathf.Abs(transform.position.y - current.position.y) > collY / 2) && Mathf.Sign(transform.position.y - current.position.y) > 0)
            {
                newY = transform.position.y - collY + offsetY;
            }

            newZ = transform.position.z;

            transform.position= new Vector3 (newX, newY, newZ);
        }
        ActivateEnemies();  
    }

    private void ActivateEnemies() {     
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy)) {
            if (coll.bounds.Contains(enemy.transform.position + new Vector3(0, 0, -10))) {
                if (enemy.gameObject.GetComponent<Animator>().GetBool("Idle")) {
                    enemy.gameObject.GetComponent<Animator>().SetBool("Idle",false);
                }
            }
        }
    }
}
