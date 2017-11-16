﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

    public float switchTime = 3f;
    public int lightCharge = 25;
    public bool lightStatus = true;
    public bool changingStatus = false;

    public Sprite[] lightStates;
    public GameObject absorptionEffect;
    
    private SpriteRenderer spriteRenderer;
    private Light lightAttached;
    private Collider2D lightCollider;
    private GameObject particleEffect;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightAttached = gameObject.transform.GetChild(0).gameObject.GetComponent<Light>();
        lightCollider = gameObject.transform.GetChild(1).gameObject.GetComponent<Collider2D>();
    }

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
        if (!lightStatus)
            StartCoroutine("SwitchingOn", gun); 
        else
            StartCoroutine("SwitchingOff", gun);  
    }

    IEnumerator SwitchingOn(Transform gun) {
        GunController gunController = gun.GetComponent<GunController>();
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", gunController.barrel);
        while (seconds>0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        spriteRenderer.sprite = lightStates[0];
        lightAttached.enabled = true;
        lightCollider.enabled = true;
        lightStatus = true;
        gunController.currentCharge -= lightCharge;
        changingStatus = false;
    }

    IEnumerator SwitchingOff(Transform gun) {
        GunController gunController = gun.GetComponent<GunController>();
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOff", gunController.barrel);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOff");
        Destroy(particleEffect);
        spriteRenderer.sprite = lightStates[1];
        lightAttached.enabled = false;
        lightCollider.enabled = false;
        lightStatus = false;
        gunController.currentCharge += lightCharge;
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