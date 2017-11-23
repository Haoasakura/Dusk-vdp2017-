using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    private bool active = false;

    [Header("Element0 -> OpenDoorSprite; Element1 -> ClosedDoorSprite")]
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
}
