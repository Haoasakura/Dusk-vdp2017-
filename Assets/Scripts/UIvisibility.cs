using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIvisibility : MonoBehaviour {

    private Text text;
    private Player player;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        player = GameObject.Find("PlayerWithGun2D").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        if (player.isVisible)
        {
            text.text = "OH GOD, YOU ARE FUCKED!";
            text.color = Color.red;
            text.fontSize = 28;
        }
        else
        {
            text.text = "BETTER THAN SNAKE!";
            text.color = Color.white;
            text.fontSize = 18;
        }
	}
}
