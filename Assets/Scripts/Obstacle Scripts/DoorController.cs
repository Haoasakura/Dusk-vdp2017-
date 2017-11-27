using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public bool active = false;

    [Header("0 -> Open; 1 -> Closed")]
    public Sprite[] sprites = new Sprite[2];

    [Header("Is the door open?")]
    [SerializeField]
    private bool isOpen = false;

    private void Start()
    {
        if (isOpen)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Move();
        }
    }

    public void Move()
    {
        if (!isOpen)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
            isOpen = true;
            active = false;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
            isOpen = false;
            active = false;
        }
    }

    public void Activate()
    {
        active = true;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        int otherDoorChildIndex = 1 - gameObject.transform.GetSiblingIndex();
        string otherName = otherDoorChildIndex.ToString();
        if (isOpen)
        {
            if(collision.gameObject.tag.Equals("Player") && Input.GetButtonDown("Fire2"))
            {
                collision.gameObject.transform.position = 
                    gameObject.transform.parent.Find(string.Concat("Door",otherName)).position;
            }
        }
    }
}
