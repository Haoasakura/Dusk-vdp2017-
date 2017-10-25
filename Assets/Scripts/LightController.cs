using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

    public bool lightStatus = true;
    public bool changingStatus = false;
    public int lightCharge = 25;
    public float switchTime = 3f;
    public Sprite[] lightStates;
    public GameObject absorptionEffect;
    
    private SpriteRenderer spriteRenderer;
    private GameObject particleEffect;

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
                StopCoroutine("TrailingEffectOn");
                StopCoroutine("TrailingEffectOff");
                Destroy(particleEffect);
                changingStatus = false; 
            }
        }
	}

    public void SwitchOnOff(Transform gun) {
        if (!lightStatus) {
            StartCoroutine("SwitchingOn", gun); 
        }
        else {
            StartCoroutine("SwitchingOff", gun);  
        }
    }

    IEnumerator SwitchingOn(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", gun.GetChild(0));
        while (seconds>0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        spriteRenderer.sprite = lightStates[0];
        lightStatus = true;
        gun.GetComponent<GunController>().currentCharge -= lightCharge;
        changingStatus = false;
    }

    IEnumerator SwitchingOff(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOff", gun.GetChild(0));
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOff");
        Destroy(particleEffect);
        spriteRenderer.sprite = lightStates[1];
        lightStatus = false;
        gun.GetComponent<GunController>().currentCharge += lightCharge;
        changingStatus = false;
    }

    IEnumerator TrailingEffectOn(Transform gun) {
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(gun.position, transform.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }

    IEnumerator TrailingEffectOff(Transform gun) {
        float startTime=Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position,transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(transform.position, gun.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }
}
