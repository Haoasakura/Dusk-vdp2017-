﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float secondsForOneLength = 5f;
    public float speed = 10f;
    public float switchTime = 3f;
    public float sightRange=10f;
    public int controlCost = 25;
    public int timeToReturnPatrol = 3;
    public bool controlled = false;
    public bool changingStatus = false;
    public bool autodestruct = true;
    public bool playerInSight;
    public LayerMask sightLayerMask;

    public GameObject absorptionEffect;
    public DecisionTree chasingDT;
    public BehaviourTree patrolBT;
    public Animator animator;

    private float weaponRange;
    private float lastX;
    private bool losingTarget=false;
    public bool shootingLights = false;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private GameObject particleEffect;
    private Enemy enemy;
    private Transform shooter = null;
    private EnemyWeapon weapon;
    private Transform player;
    private Vector3 mDirection;
    private Transform mDestination;
    private Vector3 mChaseTarget;

    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;

    

    private void Awake() {
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        weapon = GetComponentInChildren<EnemyWeapon>();      
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
    }
    void Start () {
        startPosition = transform.position;
        setDestination(startPoint);
        weaponRange = weapon.gunRange;
        lastX = transform.position.x;
    }
	
	void Update () {

        if (changingStatus) {
            if (!Input.GetButton("Fire1") && ((shooter != null && shooter.CompareTag(Tags.player)) || (shooter != null && shooter.CompareTag(Tags.enemy) && shooter.GetComponent<EnemyController>().controlled))/) {
                StopCoroutine("ConrtolledOn");
                StopCoroutine("ControlledOff");
                StopCoroutine("TrailingEffectOn");
                StopCoroutine("TrailingEffectOff");
                Destroy(particleEffect);
                changingStatus = false;
                controlled = false;
            }
        }
        else if (!controlled && !shootingLights) {
            if(transform.position.x>lastX)
                weapon.transform.rotation = Quaternion.Euler(0, 0, 0);
            else
                weapon.transform.rotation = Quaternion.Euler(0, 0, 180 /*- weapon.transform.rotation.eulerAngles.z*/);
        }
            
        lastX = transform.position.x;
    }

    public void StartPatrol() {
        Debug.Log("Start Patrol");
        StopCoroutine("Chase");
        enemy.moveMinSpeed = 2f;
        StartCoroutine("Patrol");
    }

    public void StartChase() {
        Debug.Log("Start Chase");
        StopCoroutine("Patrol");
        enemy.moveMinSpeed = 4f;
        StartCoroutine("Chase");
    }

    public void ControlledOnOff(Transform gun) {  
        if (!controlled)
            StartCoroutine("ConrtolledOn", gun);
        else
            StartCoroutine("ControlledOff", gun);
    }

    private void setDestination(Transform destination) {
        mDestination = destination;
        mDirection = (mDestination.position - transform.position).normalized/10;
    }

    private bool InLineOfSight(Collider2D target, float range) {
        if (target != null) {
            Vector3 dir = (target.transform.position - transform.position);
            dir.y = 0;
            RaycastHit2D hit = Physics2D.Raycast(weapon.barrel.position, dir, range, sightLayerMask);
            if (hit.collider != null && hit.collider.gameObject.name == target.gameObject.name && target.GetComponent<Player>()!=null && target.gameObject.GetComponent<Player>().isVisible)
                return true;
        }
        return false;
    }

    IEnumerator Patrol() {
        SpriteRenderer gunSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
        while (true) {
            foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, sightRange)) {
                if (obj.gameObject.CompareTag(Tags.light)) {
                    if (!obj.gameObject.GetComponent<LightController>().lightStatus && !obj.gameObject.GetComponent<LightController>().changingStatus && weapon.currentCharge>0) {
                        obj.GetComponent<LightController>().SwitchOnOff(weapon.transform);
                        float rotationZ = Mathf.Atan2((obj.transform.position - transform.position).y, (obj.transform.position - transform.position).x) * Mathf.Rad2Deg;
                        weapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                        shootingLights = true;

                    }
                }
            }

            if (!controlled && !changingStatus) {
                float lastX = transform.localPosition.x;
                mDirection = (mDestination.position - transform.position);
                if (Mathf.Abs(mDirection.x) < 1) {
                    mDirection.x += 0.03f * Mathf.Sign(mDirection.x);
                }
                if (shootingLights)
                    mDirection = Vector2.zero;
                /*else {
                    float rotationZ = Mathf.Atan2((weapon.laserDirection.transform.position - weapon.barrel.position).y, (weapon.laserDirection.transform.position - weapon.barrel.position).x) * Mathf.Rad2Deg;
                    weapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                }*/


                enemy.SetDirectionalInput(mDirection.normalized);

                //commented part is for the swap without the triggers
                /*if (mDirection.x>0) {
                    spriteRenderer.flipX = false;
                }
                else {
                    spriteRenderer.flipX = true;
                }

                if (startPoint.position.x - transform.position.x > 0 || endPoint.position.x - transform.position.x < 0) {
                    //setDestination(mDestination == startPoint ? endPoint : startPoint);
                    gun.transform.rotation = Quaternion.Euler(0, 0, 180 - gun.transform.rotation.eulerAngles.z);
                    gun.GetComponent<SpriteRenderer>().flipY = !gun.GetComponent<SpriteRenderer>().flipY;

                }*/
                /*if (gun.mTarget != null && gun.mTarget.CompareTag(Tags.player) && InLineOfSight(GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Collider2D>())) {
                    animator.SetBool("PlayerInSight", true);
                    mChaseTarget = gun.mTarget;
                }*/
                if(!changingStatus)
                    if (InLineOfSight(player.GetComponent<Collider2D>(), sightRange) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(transform.position + new Vector3(0, 0, -10))) {
                        animator.SetBool("PlayerInSight", true);
                        mChaseTarget = player.position;
                    }
            }
            else if (changingStatus)
                enemy.SetDirectionalInput(Vector2.zero);

            yield return null;
        }
    }

    IEnumerator Chase() {
        SpriteRenderer gunSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
        while (true) {
            if (!controlled && !changingStatus) {

                if (player != null) {
                    Debug.Log(InLineOfSight(player.GetComponent<Collider2D>(), sightRange));
                    if (!InLineOfSight(player.GetComponent<Collider2D>(), sightRange))
                        StartCoroutine("ReturnToPatrol");
                    else
                        StopCoroutine("ReturnToPatrol");
                }

                if (player!=null && InLineOfSight(player.GetComponent<Collider2D>(), weaponRange))
                    StartCoroutine("ShootPlayer");

                float lastX = transform.localPosition.x;

                // gestisce il chasing e i casi un sui deve salire le scale durante il chasing
                mDirection = (player.position - transform.position);
                if (mDirection.y >1.1f) {
                    float closestLadder = 10000f;
                    GameObject ladder = null;
                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, sightRange)) {
                        if (obj.gameObject.CompareTag(Tags.ladder)) {
                            if (Vector3.Distance(obj.transform.position, mChaseTarget) < closestLadder) {
                                closestLadder = Vector3.Distance(obj.transform.position, mChaseTarget);
                                ladder = obj.gameObject;
                            }
                        }
                    }
                    if (ladder != null) {
                        mChaseTarget = ladder.GetComponent<Collider2D>().bounds.max;
                        mDirection = (mChaseTarget - transform.position);
                        mDirection.y = -player.position.y;
                    }
                }
                else {
                    bool climbing=false;
                    GameObject ladder = null;

                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 1f)) {
                        if (obj.gameObject.CompareTag(Tags.ladder)) {
                            climbing = true;
                            ladder = obj.gameObject;
                        }
                    }

                    if(ladder != null && climbing && !enemy.GetComponent<Enemy>().controller.collisions.below) {
                        mChaseTarget = ladder.GetComponent<Collider2D>().bounds.max;
                        mDirection = (mChaseTarget - transform.position);
                        mDirection.y = -player.position.y;
                    }
                    else {
                        mChaseTarget = player.position;
                        mDirection = (mChaseTarget - transform.position);
                    }
                }
                //controllo per evitare di cadere nella sua morte
                foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 2.5f)) {
                    if (obj.gameObject.CompareTag(Tags.deathCollider)) {
                        if ((player.position - transform.position).x > (obj.gameObject.transform.position - transform.position).x) {
                            mDirection = Vector2.zero;
                            if (!losingTarget) {
                                StartCoroutine("ReturnToPatrol");
                                losingTarget = true;
                            }
                        }
                        else {
                            StopCoroutine("ReturnToPatrol");
                            losingTarget = false;
                        }
                        
                    }
                }
                if (shootingLights)
                    mDirection = Vector2.zero;

                enemy.SetDirectionalInput(mDirection.normalized);

                float rotationZ = Mathf.Atan2(mDirection.y, mDirection.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                if (mDirection.x>0) {
                    spriteRenderer.flipX = false;
                    weapon.GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    spriteRenderer.flipX = true;
                    weapon.GetComponent<SpriteRenderer>().flipY = true;
                }
            }
            else if (changingStatus)
                enemy.SetDirectionalInput(Vector2.zero);
            yield return null;
        }
    }

    IEnumerator ConrtolledOn(Transform gun) {
        Transform pointOfOrigin = null;
        if (gun.GetComponentInParent<Player>() != null) {
            pointOfOrigin = gun.GetComponent<GunController>().barrel;
            shooter = gun.GetComponentInParent<Player>().transform;
        }
        else {
            pointOfOrigin = gun.GetComponent<EnemyWeapon>().barrel;
            shooter = gun.GetComponentInParent<Enemy>().transform;
        }
        GunController gunController = gun.GetComponent<GunController>();
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", pointOfOrigin);
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        controlled = true;
        if (shooter.GetComponent<Player>() != null) {
            shooter.GetComponent<Player>().controlling = true;
            gun.GetComponent<GunController>().currentCharge -= controlCost;
        }
        else {
            shooter.GetComponent<Enemy>().controlling = true;
            gun.GetComponent<EnemyWeapon>().untraversableLayers = gun.GetComponent<EnemyWeapon>().groundLayer;
            gun.GetComponent<EnemyWeapon>().currentCharge -= controlCost;
        }
        enemy.controlling = false;
        enemy.GetComponent<EnemyInput>().enabled = true;
        enemy.GetComponentInChildren<EnemyWeapon>().untraversableLayers = enemy.GetComponentInChildren<EnemyWeapon>().groundLayer+enemy.GetComponentInChildren<EnemyWeapon>().gunLayer;
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
    IEnumerator ReturnToPatrol() {
        Debug.Log("loosing target");
        yield return new WaitForSeconds(timeToReturnPatrol);
        StopCoroutine("Patrol");
        StopCoroutine("Chase");
        Debug.Log("target lost, returning");
        while (Vector2.Distance(transform.position,startPosition)>0.3f) {
            mDirection = (startPosition - transform.position);
            enemy.SetDirectionalInput(mDirection.normalized);
            yield return null;
        }
        animator.SetBool("PlayerInSight", false);
    }

    IEnumerator ShootPlayer() {
        if(!shootingLights)
        StartCoroutine("TrailingEffectOff", player.transform);
        shootingLights = true;
        player.GetComponent<Player>().controlling = true;
        player.GetComponent<Player>().SetDirectionalInput(Vector2.zero);
        player.GetComponent<PlayerInput>().enabled = false;
        yield return new WaitForSeconds(switchTime);
        StopCoroutine("TrailingEffectOff");
        EventManager.TriggerEvent("PlayerDied");

    }

    IEnumerator TrailingEffectOff(Transform gun) {
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true) {
            particleEffect.transform.position = Vector3.Lerp(transform.position, gun.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }

    private void OnDestroy() {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.player);
        if (controlled && player != null) {
            Player mPlayer = player.GetComponent<Player>();
            if (mPlayer != null) {
                mPlayer.controlling = false;
                mPlayer.GetComponentInChildren<GunController>().isLocked = false;
            }

        }
        //questo dovrebbe essere inutile devo farlo quando sparo a un altro nemico non on destry
        if (shooter != null)
            if (controlled && shooter.gameObject.CompareTag(Tags.enemy)) {
                shooter.GetComponentInChildren<EnemyController>().controlled = false;
                /*if (shooter.GetComponent<Player>() != null) {
                    shooter.GetComponent<Player>().controlling = true;
                }
                else*/
                shooter.GetComponent<Enemy>().controlling = true;
                enemy.GetComponentInChildren<EnemyWeapon>().untraversableLayers = enemy.GetComponentInChildren<EnemyWeapon>().groundLayer;
            }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.patrolSwitch) && collision.transform.IsChildOf(transform.parent) && !controlled && !animator.GetBool("PlayerInSight")) {
            setDestination(mDestination == startPoint ? endPoint : startPoint);
            //weapon.transform.rotation = Quaternion.Euler(0, 0, 180 - weapon.transform.rotation.eulerAngles.z);
            weapon.GetComponent<SpriteRenderer>().flipY = !weapon.GetComponent<SpriteRenderer>().flipY;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(endPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
