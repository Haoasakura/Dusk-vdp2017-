using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineryController : MonoBehaviour {

    [Header("GameObject associated with linked mechanism")]
    [SerializeField]
    private GameObject mechanism;

    public float switchTime = 3f;
    public int powerCharge = 25;
    public bool powered = false;
    public bool changingStatus = false;

    public GameObject absorptionEffect;

    private SpriteRenderer spriteRenderer;
    private GameObject particleEffect;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
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
        if (!powered)
            StartCoroutine("SwitchingOn", gun);
        else
            StartCoroutine("SwitchingOff", gun);
    }

    IEnumerator SwitchingOn(Transform gun) {
        GunController gunController = gun.GetComponent<GunController>();
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", gunController.barrel);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.color = Color.red;
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        powered = true;
        gunController.currentCharge -= powerCharge;
        changingStatus = false;
        Activate();
    }

    IEnumerator SwitchingOff(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOff", gun.GetComponent<GunController>().barrel);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.color = Color.white;
        StopCoroutine("TrailingEffectOff");
        Destroy(particleEffect);
        powered = false;
        changingStatus = false;
        Activate();
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
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(transform.position, gun.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }

    private void Activate()
    {
        if (mechanism.GetComponent<ElevatorMovement>() != null)
        {
            mechanism.GetComponent<ElevatorMovement>().ChangeDestination();
        }
        else if (mechanism.GetComponent<BarrierController>() != null)
        {
            mechanism.GetComponent<BarrierController>().ChangeDestination();
        }
        else if (mechanism.GetComponent<TrapdoorController>() != null)
        {
            mechanism.GetComponent<TrapdoorController>().Activate();
        }
        else if (mechanism.GetComponent<DoorController>() != null)
        {
            mechanism.GetComponent<DoorController>().Activate();
        }
    }
}
