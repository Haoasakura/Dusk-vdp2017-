using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

    public bool lightStatus = true;
    public bool changingStatus = false;
    public int lightCharge = 25;
    public Sprite[] lightStates;

    private SpriteRenderer spriteRenderer;
    
	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (changingStatus) {
            if(Input.GetButton("Vertical") || Input.GetButton("Horizontal")) {
                StopCoroutine("SwitchingOn");
                StopCoroutine("SwitchingOff");
                changingStatus = false;
            }
        }
	}

    public void SwitchOnOff(Transform gun) {
        if (lightStatus) {
            StartCoroutine("SwitchingOff",gun);
            
        }
        else {
            StartCoroutine("SwitchingOn",gun);
            
        }
    }

    IEnumerator SwitchingOn(Transform gun) {
        changingStatus = true;
        int seconds = 3;
        while(seconds>0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.sprite = lightStates[0];
        lightStatus = true;
        gun.GetComponent<GunController>().currentCharge -= lightCharge;
        changingStatus = false;
    }

    IEnumerator SwitchingOff(Transform gun) {
        changingStatus = true;
        int seconds = 3;
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.sprite = lightStates[1];
        lightStatus = false;
        gun.GetComponent<GunController>().currentCharge += lightCharge;
        changingStatus = false;
    }
}
