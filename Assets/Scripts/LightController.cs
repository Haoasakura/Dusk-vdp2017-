using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

    public bool lightStatus = true;
    public int lightCharge = 25;
    public Sprite[] lightStates;

    private SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchOnOff() {
        if (lightStatus) {
            StartCoroutine("SwitchingOn");
            spriteRenderer.sprite = lightStates[1];
            lightStatus = false;
        }
        else {
            StartCoroutine("SwitchingOff");
            spriteRenderer.sprite = lightStates[0];
            lightStatus = true;
        }
    }

    IEnumerator SwitchingOn() {
        yield return null;
    }

    IEnumerator SwitchingOff() {
        yield return null;
    }
}
