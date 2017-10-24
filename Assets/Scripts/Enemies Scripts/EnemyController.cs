using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public Transform endPosition;
    public float secondsForOneLength = 5f;
    public float mSpeed = 10f;
    public float switchTime = 3f;
    public int controlCost = 25;
    public bool controlled = false;
    public bool changingStatus = false;
    public GameObject absorptionEffect;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private GameObject particleEffect;


    // Use this for initialization
    void Start () {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        if (changingStatus) {
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal")) {
                StopCoroutine("ConrtolledOn");
                StopCoroutine("ControlledOff");
                StopCoroutine("TrailingEffectOn");
                StopCoroutine("TrailingEffectOff");
                Destroy(particleEffect);
                changingStatus = false;
                controlled = false;
            }
        }

        if (!controlled && !changingStatus) {
            float lastX = transform.position.x;
            transform.position = Vector3.Lerp(startPosition, endPosition.position, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time / secondsForOneLength, 1f)));
            if (transform.position.x > lastX)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }
        else {
           float mHorizontal = Input.GetAxisRaw("Horizontal");
           float mVertical = Input.GetAxisRaw("Vertical");

            GetComponent<Rigidbody2D>().velocity = new Vector2(mHorizontal*mSpeed,0f);

            if(Input.GetKey(KeyCode.F)) {
                Destroy(gameObject);
            }
        }
    }

    public void ControlledOnOff(Transform gun) {
        if (!controlled) {
            StartCoroutine("ConrtolledOn", gun);

        }
        else {
            StartCoroutine("ControlledOff", gun);

        }
    }

    IEnumerator ConrtolledOn(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", gun);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        controlled = true;
        gun.GetComponent<GunController>().currentCharge -= controlCost;
        gun.GetComponent<GunController>().inControl = true;
        changingStatus = false;
    }

    /*IEnumerator ControlledOff(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOff", gun);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOff");
        Destroy(particleEffect);
        controlled = false;
        changingStatus = false;
    }*/

    IEnumerator TrailingEffectOn(Transform gun) {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(gun.position, transform.position);
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(gun.position, transform.position, ((Time.time - startTime) * (switchTime-0.5f)) / journeyLength);
            yield return null;
        }
    }

    /*IEnumerator TrailingEffectOff(Transform gun) {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.position, gun.position);
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(transform.position, gun.position, ((Time.time - startTime) * switchTime) / journeyLength);
            yield return null;
        }
    }*/

    private void OnDestroy() {
        if(controlled) {
            GameObject.FindGameObjectWithTag(Tags.player).GetComponent<GunController>().inControl = false;
        }
    }
}
