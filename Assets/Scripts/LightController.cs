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
                changingStatus = false;
                Destroy(particleEffect);
            }
        }
	}

    public void SwitchOnOff(Transform gun) {
        if (!lightStatus) {
            StartCoroutine("SwitchingOn",gun); 
        }
        else {
            StartCoroutine("SwitchingOff",gun);  
        }
    }

    IEnumerator SwitchingOn(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", gun);
        while (seconds>0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        //Destroy(absorptionParticleSystem);
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
        StartCoroutine("TrailingEffectOff", gun);
        //particleSystem.transform.position = Vector3.Lerp(transform.position, gun.position, 3f);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        //Destroy(absorptionParticleSystem);
        StopCoroutine("TrailingEffectOff");
        Destroy(particleEffect);
        spriteRenderer.sprite = lightStates[1];
        lightStatus = false;
        gun.GetComponent<GunController>().currentCharge += lightCharge;
        changingStatus = false;
    }

    IEnumerator TrailingEffectOn(Transform gun) {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(gun.position, transform.position);
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(gun.position, transform.position, ((Time.time - startTime) * switchTime) / journeyLength);
            yield return null;
        }
    }

    IEnumerator TrailingEffectOff(Transform gun) {
        float startTime=Time.time;
        float journeyLength = Vector3.Distance(transform.position, gun.position);
        particleEffect = Instantiate(absorptionEffect, transform.position,transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position=Vector3.Lerp(transform.position, gun.position, ((Time.time - startTime) * switchTime) / journeyLength);
            yield return null;
        }
    }
}
