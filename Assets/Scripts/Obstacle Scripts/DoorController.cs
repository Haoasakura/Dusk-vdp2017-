using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public bool active = false;

    private DigitalRuby.LightningBolt.LightningBoltScript lightning;


    [Header("0 -> Open; 1 -> Closed")]
    public Sprite[] sprites = new Sprite[2];

    [Header("Is the door open?")]
    [SerializeField]
    private bool isOpen = false;

    private void Start()
    {
        lightning = GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();

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
                StartCoroutine(MoveCamera(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y));
                lightning.Trigger();
                SoundManager.Instance.EmptyGunshot();
            }
        }
    }

    private IEnumerator MoveCamera(float x, float y)
    {
        yield return new WaitForSeconds(1f);
        GameObject c = GameObject.Find("Main Camera");
        if (x%30 > 15)
        {
            x = x + (30 - x % 30);
        }
        else
        {
            x = x - x % 30;
        }

        if (y% 18 > 9)
        {
            y = y + (18 - (y % 18));
        }
        else
        {
            y = y - ( y % 18);
        }

        c.transform.position = new Vector3(x, y, -10f);
        

    }
}
