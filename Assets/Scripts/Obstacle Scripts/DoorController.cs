using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    private bool active = false;

    [Header("Is the door open?")]
    [SerializeField]
    private bool isOpen = false;
    
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
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites\\DoorOpenPlaceholder", typeof(Sprite)) as Sprite;
            isOpen = true;
            active = false;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("DoorClosedPlaceholder", typeof(Sprite)) as Sprite;
            isOpen = false;
            active = false;
        }
    }

    public void Activate()
    {
        active = true;
    }
}
