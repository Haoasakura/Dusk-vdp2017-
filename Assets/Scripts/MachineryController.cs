using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineryController : MonoBehaviour {

    public int powerCharge = 25;
    public bool powered = false;
    public bool changingStatus = false;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (changingStatus) {
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal")) {
                StopCoroutine("SwitchingOn");
                StopCoroutine("SwitchingOff");
                changingStatus = false;
            }
        }
    }

    public void SwitchOnOff(Transform gun) {
        if (!powered) {
            StartCoroutine("SwitchingOn", gun);

        }
        else {
            StartCoroutine("SwitchingOff", gun);

        }
    }

    IEnumerator SwitchingOn(Transform gun) {
        changingStatus = true;
        int seconds = 3;
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        //spriteRenderer.sprite = lightStates[0];
        spriteRenderer.color = Color.red;
        powered = true;
        gun.GetComponent<GunController>().currentCharge -= powerCharge;
        changingStatus = false;
    }

    IEnumerator SwitchingOff(Transform gun) {
        changingStatus = true;
        int seconds = 3;
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        //spriteRenderer.sprite = lightStates[1];
        spriteRenderer.color = Color.white;
        powered = false;
        changingStatus = false;
    }
}
