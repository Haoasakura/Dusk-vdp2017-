using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIvisibility : MonoBehaviour {

    private Image eye;
    public Player player;
    private bool chaseInProgress;
    private GameObject currentControl;

    public Sprite[] sprites = new Sprite[3];

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Player>();
        eye = GetComponent<Image>();
        currentControl = player.gameObject;
        chaseInProgress = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (player.controlling)
        {
            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy))
            {
                if (enemy.GetComponent<EnemyController>().controlled)
                {
                    currentControl = enemy.gameObject;
                }
                else
                {
                    currentControl = player.gameObject;
                }
            }
        }

        if (player != null && currentControl == player.gameObject)
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy))
            {
                if (enemy.GetComponent<Enemy>().moveMinSpeed == 4f)
                {
                    chaseInProgress = true;
                    break;
                }
                chaseInProgress = false;
            }
            if (chaseInProgress)
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
        else
        {
            eye.sprite = sprites[1];
        }
        
	}
}
