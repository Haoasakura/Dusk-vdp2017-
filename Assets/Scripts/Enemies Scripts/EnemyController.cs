using System.Collections;
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
    public bool flipStartDir = false;
    public LayerMask sightLayerMask;

    public GameObject explosion;
    public GameObject alert;
    public GameObject absorptionEffect;  
    public DecisionTree chasingDT;
    public BehaviourTree patrolBT;
    public Animator animator;

    private float weaponRange;
    private float lastX;
    public bool losingTarget=false;
    public bool shootingLights = false;

    private Vector3 startPosition;
    private GameObject particleEffect;
    private Enemy enemy;
    private Transform shooter = null;
    private EnemyWeapon weapon;
    private Transform player;
    private Vector3 mDirection;
    private Transform mDestination;
    private Vector3 mChaseTarget;
    private EnemyController2D enemyController2D;
    private BoxCollider2D boxCollider2D;
    private GameObject mLadder;
    private bool isClimbing = false;
    private bool gettingShoot=false;

    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;

    

    private void Awake() {
        enemy = GetComponent<Enemy>();
        weapon = GetComponentInChildren<EnemyWeapon>();      
        animator = GetComponent<Animator>();
        enemyController2D = GetComponent<EnemyController2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
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
            if (!Input.GetButton("Fire1") && ((shooter != null && shooter.CompareTag(Tags.player)) || (shooter != null && shooter.CompareTag(Tags.enemy) && shooter.GetComponent<EnemyController>().controlled))) {
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
            if (transform.position.x > lastX)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }   
        lastX = transform.position.x;
    }

    public void StartPatrol() {
        //Debug.Log("Start Patrol");
        StopCoroutine("Patrol");
        StopCoroutine("Chase");
        enemy.moveMinSpeed = 2f;
        animator.SetBool("PlayerInSight", false);
        playerInSight = false;
        if (flipStartDir)
            setDestination(endPoint);
        StartCoroutine("Patrol");
        weapon.mLineRenderer.material = weapon.idleMaterial;
    }

    public void StartChase() {
        //Debug.Log("Start Chase");
        StopCoroutine("Patrol");
        StopCoroutine("Chase");
        enemy.moveMinSpeed = 4f;
        animator.SetBool("PlayerInSight", true);
        playerInSight = true;
        SoundManager.Instance.ChangeSoundtrack();
        StartCoroutine("TransitionEffects");
        StartCoroutine("Chase");
        
    }

    public void ControlledOn(Transform gun) {
        StartCoroutine("ConrtolledOn", gun);
    }

    private void setDestination(Transform destination) {
        mDestination = destination;
        mDirection = (mDestination.position - transform.position).normalized/10f;
    }

    private bool InLineOfSight(Transform target, float range) {
        if (player != null) {
            Vector3 dir = (weapon.laserDirection.position - weapon.barrel.position);
            dir.y = 0;
            RaycastHit2D hit = Physics2D.Raycast(weapon.barrel.position, dir, range, sightLayerMask);
            if (hit.transform)
                if (hit.collider != null && hit.collider.gameObject.name == target.name && target.GetComponent<Player>() != null && target.GetComponent<Player>().isVisible)
                    return true;
        }
        return false;
    }

    IEnumerator Patrol() {
        
        while (true) {

            if ((startPoint.position.x - transform.position.x > 0 || enemyController2D.collisions.left) && !controlled && !animator.GetBool("PlayerInSight"))
                setDestination(endPoint);

            else if ((endPoint.position.x - transform.position.x < 0 || enemyController2D.collisions.right) && !controlled && !animator.GetBool("PlayerInSight"))
                setDestination(startPoint);

            if (!animator.GetBool("PlayerInSight") && transform.position.x > startPoint.position.x && transform.position.x < endPoint.position.x) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(weapon.pivot.position.x, transform.GetComponent<BoxCollider2D>().bounds.max.y+0.01f), Vector2.up);
                if(hit && hit.collider.CompareTag(Tags.light)) {
                    LightController lightController = hit.collider.gameObject.GetComponent<LightController>();
                    if (!lightController.lightStatus && !lightController.changingStatus && weapon.currentCharge > 0) {
                        lightController.SwitchOnOff(weapon.transform);
                        weapon.armTransform.rotation = Quaternion.Euler(0.0f, 0.0f, enemy.transform.localScale.x * 159.1f);
                        weapon.StartCoroutine("LightningEffectOn",lightController.switchTime);
                        shootingLights = true;
                    }
                }
            }

            if (!controlled && !changingStatus) {
                //questo è il caso del return to patrol
                if (transform.position.y - startPoint.position.y > 0.5f) {
                    float closestLadder = 10000f;
                    GameObject ladder = null;
                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 100f)) {
                        if (obj.CompareTag(Tags.ladder) && obj.transform.position.y <= boxCollider2D.bounds.min.y) {
                            if (Vector3.Distance(obj.transform.position, transform.position) < closestLadder) {
                                closestLadder = Vector3.Distance(obj.transform.position, transform.position);
                                ladder = obj.transform.gameObject;
                            }
                        }
                    }
                    if (ladder != null) {
                        mChaseTarget = ladder.GetComponent<Collider2D>().bounds.max /*+new Vector3(transform.localScale.x* 4,0,0)*/;
                        mDirection = (mChaseTarget - transform.position);
                        mDirection.x +=transform.localScale.x;
                        if (ladder.transform.Find("TopLadder"))
                            ladder.transform.Find("TopLadder").gameObject.layer = 9;
                    }
                }
                else if (!(transform.position.x > startPoint.position.x && transform.position.x < endPoint.position.x)) {
                    mDirection = (startPosition - transform.position);
                }
                else {
                    mDirection = (mDestination.position - transform.position);
                    if (Mathf.Abs(mDirection.x) < 1)
                        mDirection.x += 0.03f * Mathf.Sign(mDirection.x);
                }

                if (shootingLights)
                    mDirection = Vector2.zero;

                if (!changingStatus && player != null)
                    if (InLineOfSight(player, sightRange) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(transform.position + new Vector3(0, 0, -10)) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(player.position + new Vector3(0, 0, -10))) {
                        animator.SetBool("PlayerInSight", true);
                        playerInSight = true;
                        mChaseTarget = player.position;
                    }
                //Debug.Log(mDirection.normalized);
                enemy.SetDirectionalInput(mDirection.normalized);
            }
            else if (changingStatus || enemy.controlling)
                enemy.SetDirectionalInput(Vector2.zero);

            yield return null;
        }
    }

    IEnumerator Chase() {

        while (true) {
            //gestisce la perdita di visione del player
            if (!controlled && !changingStatus && player) {
                if (player != null) {
                    if (!InLineOfSight(player, sightRange) && !enemy.isClimbing && !((player.position - transform.position).y > 1.6f && (player.position - transform.position).y < 6f)) {
                        if(!losingTarget)
                            StartCoroutine("ReturnToPatrol");
                        losingTarget = true;
                    }
                    else {
                        StopCoroutine("ReturnToPatrol");
                        losingTarget = false;
                    }
                }

                //gestisce il chasing e i casi un sui deve salire le scale durante il chasing
                mDirection = (player.position - transform.position);
                if (mDirection.y > 1.6f || isClimbing) {
                    RaycastHit2D hit;
                    if (transform.localScale.x > 0) {
                        hit = Physics2D.Raycast(new Vector2(boxCollider2D.bounds.max.x + 0.01f, boxCollider2D.bounds.center.y), transform.localScale.x * Vector2.right, 35);
                        //Debug.DrawRay(new Vector2(boxCollider2D.bounds.max.x + 0.01f, boxCollider2D.bounds.center.y), transform.localScale.x * Vector2.right * 35,Color.cyan);
                    }
                    else {
                        hit = Physics2D.Raycast(new Vector2(boxCollider2D.bounds.min.x - 0.01f, boxCollider2D.bounds.min.y), -transform.localScale.x*Vector2.left, 35);
                        //Debug.DrawRay(new Vector2(boxCollider2D.bounds.min.x - 0.01f, boxCollider2D.bounds.min.y), -transform.localScale.x*Vector2.left*35, Color.cyan);
                    }

                    if(hit && (hit.collider.CompareTag(Tags.ladder) || hit.collider.CompareTag(Tags.baseLadder) || hit.collider.CompareTag(Tags.topLadder))) {
                        if (hit.collider.transform.name.StartsWith("Ladder"))
                            mLadder = hit.collider.gameObject;
                        else
                            mLadder = hit.collider.transform.parent.gameObject;
                    }
                    else {
                        StartCoroutine("ReturnToPatrol");
                        losingTarget = true;
                    }

                    if (mLadder != null) {
                        foreach (Collider2D obj in Physics2D.OverlapCircleAll(boxCollider2D.bounds.min, 0.1f)) {
                            if ((obj.CompareTag(Tags.ladder) || obj.CompareTag(Tags.baseLadder) || obj.CompareTag(Tags.topLadder))) {
                                isClimbing = true;
                                break;
                            }
                            else
                                isClimbing = false;
                        }
                        if (isClimbing) {
                            mChaseTarget = mLadder.GetComponent<Collider2D>().bounds.center + new Vector3(0, 4f, 0);
                            mDirection = (mChaseTarget - transform.position);
                        }
                        else {
                            mChaseTarget = mLadder.GetComponent<Collider2D>().bounds.center;
                            mDirection = (mChaseTarget - transform.position);
                        }
                    }
                }
                else {
                    mChaseTarget = player.position;
                    mDirection = (mChaseTarget - transform.position);
                }

                if (!enemy.isClimbing && player != null && InLineOfSight(player, weaponRange) && enemy.controller.collisions.below && !changingStatus && !gettingShoot)
                    StartCoroutine("ShootPlayer");

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

                if (shootingLights || gettingShoot)
                    mDirection = Vector2.zero;

                enemy.SetDirectionalInput(mDirection.normalized);
            }
            else if (changingStatus || gettingShoot)
                enemy.SetDirectionalInput(Vector2.zero);

            yield return null;
        }
    }

    IEnumerator ConrtolledOn(Transform gun) {
        Transform pointOfOrigin = null;
        changingStatus = true;
        gettingShoot = true;
        if (gun.GetComponentInParent<Player>() != null) {
            pointOfOrigin = gun.GetComponent<GunController>().barrel;
            shooter = gun.GetComponentInParent<Player>().transform;
        }
        else {
            pointOfOrigin = gun.GetComponent<EnemyWeapon>().barrel;
            shooter = gun.GetComponentInParent<Enemy>().transform;
        }
        
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
            EventManager.TriggerEvent("EnemyControlled");
        }
        else {
            shooter.GetComponent<Enemy>().controlling = false;
            shooter.GetComponent<EnemyController>().controlled = false;
            shooter.GetComponent<EnemyController>().playerInSight = false;
            shooter.GetComponent<Animator>().SetBool("PlayerInSight", false);
            shooter.GetComponent<EnemyController>().StartPatrol();
            gun.GetComponent<EnemyWeapon>().untraversableLayers = gun.GetComponent<EnemyWeapon>().groundLayer;
            gun.GetComponent<EnemyWeapon>().currentCharge -= controlCost;
            gun.GetComponent<EnemyWeapon>().isLocked = true;
            shooter.GetComponent<EnemyInput>().enabled = false;
            shooter.GetComponent<EnemyController>().autodestruct = false;
        }
        enemy.controlling = false;
        enemy.GetComponent<EnemyInput>().enabled = true;
        enemy.GetComponentInChildren<EnemyWeapon>().untraversableLayers = enemy.GetComponentInChildren<EnemyWeapon>().groundLayer+enemy.GetComponentInChildren<EnemyWeapon>().gunLayer;
        autodestruct = true;
        changingStatus = false;
        gettingShoot = false;
    }

    IEnumerator TransitionEffects() {
        changingStatus = true;
        int seconds = 1;
        
        while (seconds > 0) {
            Instantiate(alert, transform.position+Vector3.up*1.6f, transform.rotation);
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        weapon.mLineRenderer.material = weapon.aimMaterial;
        changingStatus = false;
    }

    IEnumerator ReturnToPatrol() {
        //Debug.Log("Loosing Target...");
        yield return new WaitForSeconds(timeToReturnPatrol);
        //Debug.Log("...Target Lost, Returning!");
        SoundManager.Instance.ChangeSoundtrack();
        animator.SetBool("PlayerInSight", false);
        playerInSight = false;
        losingTarget = false;
    }

    IEnumerator ShootPlayer() {
        if(!shootingLights)
        StartCoroutine("TrailingEffectOff", player.transform);
        shootingLights = true;
        player.GetComponent<Player>().controlling = true;
        player.GetComponent<Player>().SetDirectionalInput(Vector2.zero);

        player.GetComponent<PlayerInput>().enabled = false;
        yield return new WaitForSeconds(switchTime);
        EventManager.TriggerEvent("PlayerControlled");
        StopCoroutine("TrailingEffectOff");       
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
            weapon.soundManager.Gunshot((Time.time - startTime));
            weapon.lightning.Trigger();
        }
    }

    private void OnDestroy() {

        Instantiate(explosion, transform.position, transform.rotation);
        if (controlled && player != null) {
            EventManager.TriggerEvent("EnemyDestroyed");
            Player mPlayer = player.GetComponent<Player>();
            if (mPlayer != null) {
                mPlayer.controlling = false;
                mPlayer.GetComponentInChildren<GunController>().isLocked = false;
            }
        }

        if (shooter != null)
            if (controlled && shooter.gameObject.CompareTag(Tags.enemy)) {
                shooter.GetComponentInChildren<EnemyController>().controlled = false;
                shooter.GetComponent<Enemy>().controlling = true;
                enemy.GetComponentInChildren<EnemyWeapon>().untraversableLayers = enemy.GetComponentInChildren<EnemyWeapon>().groundLayer;
            }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(endPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.DrawWireSphere(transform.position, 16);
       
       
    }
}
