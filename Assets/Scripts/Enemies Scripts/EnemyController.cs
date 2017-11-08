using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float secondsForOneLength = 5f;
    public float speed = 10f;
    public float switchTime = 3f;
    public int controlCost = 25;
    public bool controlled = false;
    public bool changingStatus = false;
    public bool autodestruct = true;

    public Transform endPosition;
    public GameObject absorptionEffect;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private GameObject particleEffect;
    private Player enemy;
    private Player shooter = null;
    private GunController gun;
    private Vector3 mDirection;
    private Transform mDestination;

    [SerializeField]
    private Transform enemyTransform;
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;

    void Start () {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemy = GetComponent<Player>();
        gun = GetComponentInChildren<GunController>();
        setDestination(startPoint);
    }
	
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

            enemy.controller.Move(mDirection * Time.deltaTime, mDirection);
            if (transform.position.x > lastX) {
                spriteRenderer.flipX = false;                
            }
            else {
                spriteRenderer.flipX = true;
            }

            if (startPoint.position.x - transform.position.x >= 0 || endPoint.position.x - transform.position.x <= 0) {
                setDestination(mDestination == startPoint ? endPoint : startPoint);
                //gun.transform.rotation = new Quaternion(gun.transform.rotation.x, gun.transform.rotation.y, gun.transform.rotation.z + 180, Quaternion.identity.z);
                //non ruota!!!!
                gun.transform.rotation = Quaternion.Euler(0, 0, -(gun.transform.rotation.z));
                gun.GetComponent<SpriteRenderer>().flipX = !gun.GetComponent<SpriteRenderer>().flipX;
                //gun.GetComponent<SpriteRenderer>().flipY = !gun.GetComponent<SpriteRenderer>().flipY;

            }

            if (gun.mTarget!=null && gun.mTarget.CompareTag(Tags.player) && gun.InLineOfSight(gun.mTarget.GetComponent<Collider2D>()))
                Debug.Log("Die Player, Die!");
        }
        else {
            if (autodestruct && Input.GetKey(KeyCode.F))
                Destroy(gameObject);
        }

    }

    public void ControlledOnOff(Transform gun) {  
        if (!controlled)
            StartCoroutine("ConrtolledOn", gun);
        else
            StartCoroutine("ControlledOff", gun);
    }

    private void setDestination(Transform destination) {
        mDestination = destination;
        mDirection = (mDestination.position - enemyTransform.position).normalized;
    }

    IEnumerator ConrtolledOn(Transform gun) {
        
        /*if (GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Player>() != null)
            shooter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Player>();*/
        shooter = gun.GetComponentInParent<Player>();
        GunController gunController = gun.GetComponent<GunController>();
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", gunController.barrel);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        controlled = true;
        gunController.currentCharge -= controlCost;
        shooter.controlling = true;
        enemy.controlling = false;
        changingStatus = false;
    }

    /*IEnumerator ControlledOff(Transform gun) {
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOff", gun.GetComponent<GunController>().barrel);
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
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(gun.position, transform.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }

    /*IEnumerator TrailingEffectOff(Transform gun) {
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(transform.position, gun.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }*/

    private void OnDestroy() {
        if(controlled && GameObject.FindGameObjectWithTag(Tags.player))
            if(GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Player>()!=null)
                GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Player>().controlling = false;

        if (shooter != null)
            if (controlled && shooter.gameObject.tag == Tags.enemy) {
                shooter.GetComponent<EnemyController>().controlled = false;
                shooter.controlling = true;
            }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPoint.position, enemyTransform.GetComponent<BoxCollider2D>().size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(endPoint.position, enemyTransform.GetComponent<BoxCollider2D>().size);
    }
}
