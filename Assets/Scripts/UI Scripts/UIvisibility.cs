using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIvisibility : MonoBehaviour {

    private Image eye;
    public Player player;

    public Sprite[] sprites = new Sprite[3];

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Player>();
        eye = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy))
        {
            if(enemy.GetComponent<Enemy>().moveMinSpeed >= 4f)
            {
                eye.sprite = sprites[2];
            }
            else
            {
                if (player.isVisible)
                {
                    eye.sprite = sprites[1];
                }
                else
                {
                    eye.sprite = sprites[0];
                }
            }
        }
        
	}
}
