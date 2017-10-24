using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineryController : MonoBehaviour {

    public float switchTime = 3f;
    public int powerCharge = 25;
    public bool powered = false;
    public bool changingStatus = false;
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
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal")) {
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
        if (!powered) {
            StartCoroutine("SwitchingOn", gun);

        }
        else {
            StartCoroutine("SwitchingOff", gun);

        }
    }

    IEnumerator SwitchingOn(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", gun);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.color = Color.red;
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        powered = true;
        gun.GetComponent<GunController>().currentCharge -= powerCharge;
        changingStatus = false;
    }

    IEnumerator SwitchingOff(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOff", gun);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.color = Color.white;
        StopCoroutine("TrailingEffectOff");
        Destroy(particleEffect);
        powered = false;
        changingStatus = false;
    }

    IEnumerator TrailingEffectOn(Transform gun) {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(gun.position, transform.position);
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(gun.position, transform.position, ((Time.time - startTime) * (switchTime-0.5f)) / journeyLength);
            yield return null;
        }
    }

    IEnumerator TrailingEffectOff(Transform gun) {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.position, gun.position);
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(transform.position, gun.position, ((Time.time - startTime) * switchTime) / journeyLength);
            yield return null;
        }
    }
}
