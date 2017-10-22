using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public Transform endPosition;
    public float secondsForOneLength = 5f;
    public float mSpeed = 10f;
    public int controlCost = 25;
    public bool controlled = false;
    public bool changingStatus = false;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    

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
        int seconds = 3;
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        //spriteRenderer.sprite = lightStates[0];
        controlled = true;
        gun.GetComponent<GunController>().currentCharge -= controlCost;
        gun.GetComponent<GunController>().inControl = true;
        changingStatus = false;
    }

    IEnumerator ControlledOff(Transform gun) {
        changingStatus = true;
        int seconds = 3;
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        //spriteRenderer.sprite = lightStates[1];
        controlled = false;
        changingStatus = false;
    }

    private void OnDestroy() {
        if(controlled) {
            GameObject.FindGameObjectWithTag(Tags.player).GetComponent<GunController>().inControl = false;
        }
    }
}
