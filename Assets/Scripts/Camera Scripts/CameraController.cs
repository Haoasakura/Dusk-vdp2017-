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

    private void OnEnable()
    {
        coll = GetComponent<BoxCollider2D>();
        collX = coll.size.x;
        collY = coll.size.y;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            Debug.Log(Mathf.Abs(transform.position.x - collision.transform.position.x));


            //Traslazioni nel caso il giocatore esca dal collider
            if ((Mathf.Abs(transform.position.x - collision.transform.position.x) > collX / 2) && Mathf.Sign(transform.position.x - collision.transform.position.x) < 0)
            {
                newX = transform.position.x + collX - offsetX;
            }
            else if ((Mathf.Abs(transform.position.x - collision.transform.position.x) > collX / 2) && Mathf.Sign(transform.position.x - collision.transform.position.x) > 0)
            {
                newX = transform.position.x - collX + offsetX;
            }
            else if ((Mathf.Abs(transform.position.y - collision.transform.position.y) > collY / 2) && Mathf.Sign(transform.position.y - collision.transform.position.y) < 0)
            {
                newY = transform.position.y + collY - offsetY;
            }
            else if ((Mathf.Abs(transform.position.y - collision.transform.position.y) > collY / 2) && Mathf.Sign(transform.position.y - collision.transform.position.y) > 0)
            {
                newY = transform.position.y - collY + offsetY;
            }

            newZ = transform.position.z;

            transform.position= new Vector3 (newX, newY, newZ);
        }
    }
}
