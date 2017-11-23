using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIvisibility : MonoBehaviour {

    private Text text;
    public Player player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Player>();
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (player.isVisible)
        {
            text.text = "OH GOD, YOU ARE FUCKED!";
            text.color = Color.white;
            text.fontSize = 18;
        }
        else
        {
            text.text = "BETTER THAN SNAKE!";
            text.color = Color.white;
            text.fontSize = 18;
        }
	}
}
