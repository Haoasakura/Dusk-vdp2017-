using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLight : MonoBehaviour {

    public float LightReflected = 0f;
    public float LightInDark = 0f;

    private Color color;
    

	// Use this for initialization
	void Start () {
        color = GetComponent<SpriteRenderer>().color;
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("LightCollider"))
        {
            color.a = LightReflected;
            GetComponent<SpriteRenderer>().color = color;
            Debug.Log(GetComponent<SpriteRenderer>().color.a);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("LightCollider"))
        {
            color.a = LightInDark;
            GetComponent<SpriteRenderer>().color = color;
            Debug.Log(GetComponent<SpriteRenderer>().color.a);
        }
    }
}
