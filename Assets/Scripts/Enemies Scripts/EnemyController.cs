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
            if (transform.position.x > lastX) {
                transform.localScale = new Vector3(1, 1, 1);
                //weapon.transform.rotation = Quaternion.Euler(0, 0, 0);
                //weapon.armTransform.rotation = Quaternion.Euler(0f, 0f, -21.1f);
            }
            else {
                transform.localScale = new Vector3(-1, 1, 1);
                //weapon.armTransform.rotation = Quaternion.Euler(0f, 180f, -21.1f);
                //weapon.transform.rotation = Quaternion.Euler(0, 0, 180 /*- weapon.transform.rotation.eulerAngles.z);
            }

        }
            
        lastX = transform.position.x;
    }

    public void StartPatrol() {
        Debug.Log("Start Patrol");
        StopCoroutine("Patrol");
        StopCoroutine("Chase");
        enemy.moveMinSpeed = 2f;
        animator.SetBool("PlayerInSight", false);
        //mettere qui la coroutine per fare le cose di transizione
        //StartCoroutine("TransitionEffects");
        if (flipStartDir)
        {
            setDestination(endPoint);
        }
        StartCoroutine("Patrol");
    }

    public void StartChase() {
        Debug.Log("Start Chase");
        StopCoroutine("Patrol");
        StopCoroutine("Chase");
        enemy.moveMinSpeed = 4f;
        animator.SetBool("PlayerInSight", true);
        StartCoroutine("TransitionEffects");
        StartCoroutine("Chase");
    }

    public void ControlledOnOff(Transform gun) {  
        //if (!controlled)
            StartCoroutine("ConrtolledOn", gun);
    }

    private void setDestination(Transform destination) {
        mDestination = destination;
        mDirection = (mDestination.position - transform.position).normalized/10f;
    }

    private bool InLineOfSight(Collider2D target, float range) {
        if (target != null) {
            Vector3 dir = (weapon.laserDirection.position - transform.position);
            dir.y = 0;
            //Debug.DrawRay(weapon.barrel.position, dir, Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(weapon.barrel.position, dir, range, sightLayerMask);
            if(hit.transform)
                if (hit.collider != null && hit.collider.gameObject.name == target.gameObject.name && target.GetComponent<Player>() != null && target.gameObject.GetComponent<Player>().isVisible) {
                    return true;
                }
        }
        return false;
    }

    IEnumerator Patrol() {
        SpriteRenderer gunSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
        while (true) {
            if (!animator.GetBool("PlayerInSight") && transform.position.x > startPoint.position.x && transform.position.x < endPoint.position.x) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(weapon.pivot.position.x, transform.GetComponent<BoxCollider2D>().bounds.max.y+0.01f), Vector2.up);
                if(hit && hit.collider.CompareTag(Tags.light)) {
                    LightController lightController = hit.collider.gameObject.GetComponent<LightController>();
                    if (!lightController.lightStatus && !lightController.changingStatus && weapon.currentCharge > 0) {
                        lightController.SwitchOnOff(weapon.transform);
                        weapon.armTransform.rotation = Quaternion.Euler(0.0f, 0.0f, enemy.transform.localScale.x * 159.1f);
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
                }*/
                if ((startPoint.position.x - transform.position.x > 0 || enemyController2D.collisions.left) && !controlled && !animator.GetBool("PlayerInSight")) {
                    setDestination(endPoint);
                    //setDestination(mDestination == startPoint ? endPoint : startPoint);
                    //weapon.armTransform.rotation = Quaternion.Euler(0, 180 - weapon.armTransform.rotation.eulerAngles.y,0);
                    //weapon.GetComponent<SpriteRenderer>().flipY = !gun.GetComponent<SpriteRenderer>().flipY;
                }
                else if ((endPoint.position.x - transform.position.x < 0 || enemyController2D.collisions.right) && !controlled && !animator.GetBool("PlayerInSight")) {
                    setDestination(startPoint);
                    //weapon.armTransform.rotation = Quaternion.Euler(0, 180 - weapon.armTransform.rotation.eulerAngles.y, 0);
                    //setDestination(mDestination == startPoint ? endPoint : startPoint);
                    //weapon.transform.rotation = Quaternion.Euler(0, 0, 180 - gun.transform.rotation.eulerAngles.z);
                    //weapon.GetComponent<SpriteRenderer>().flipY = !gun.GetComponent<SpriteRenderer>().flipY;
                }
                /*if (gun.mTarget != null && gun.mTarget.CompareTag(Tags.player) && InLineOfSight(GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Collider2D>())) {
                    animator.SetBool("PlayerInSight", true);
                    mChaseTarget = gun.mTarget;
                }*/
                if (!changingStatus && !(player == null))
                    if (InLineOfSight(player.GetComponent<Collider2D>(), sightRange) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(transform.position + new Vector3(0, 0, -10))) {
                        animator.SetBool("PlayerInSight", true);
                        mChaseTarget = player.position;
                    }
            }
            else if (changingStatus || enemy.controlling)
                enemy.SetDirectionalInput(Vector2.zero);

            yield return null;
        }
    }

    IEnumerator Chase() {
        SpriteRenderer gunSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
        while (true) {
            if (!controlled && !changingStatus && player) {
                if (player != null) {
                    if (!InLineOfSight(player.GetComponent<Collider2D>(), sightRange) && !enemy.isClimbing && !((player.position - transform.position).y > 1.6f && (player.position - transform.position).y < 6f)) {
                        if(!losingTarget)
                            StartCoroutine("ReturnToPatrol");
                        losingTarget = true;
                    }
                    else {
                        Debug.Log("stop coroutine");
                        StopCoroutine("ReturnToPatrol");
                        losingTarget = false;
                    }
                }

                // gestisce il chasing e i casi un sui deve salire le scale durante il chasing
                mDirection = (player.position - transform.position);
               //Debug.Log(mDirection.y);
                if (mDirection.y > 1.6f || isClimbing) {
                    float closestLadder = 10000f;
                    GameObject ladder = null;
                    RaycastHit2D hit;
                    if (transform.localScale.x > 0) {
                        hit = Physics2D.Raycast(new Vector2(boxCollider2D.bounds.max.x + 0.01f, boxCollider2D.bounds.center.y /*+ 1.11f*/), transform.localScale.x * Vector2.right, 35);
                        Debug.DrawRay(new Vector2(boxCollider2D.bounds.max.x /*+ 1.01f*/, boxCollider2D.bounds.center.y/* + 1.11f*/), transform.localScale.x * Vector2.right * 35,Color.cyan);
                    }
                    else {
                        hit = Physics2D.Raycast(new Vector2(boxCollider2D.bounds.min.x - 0.01f, boxCollider2D.bounds.min.y /*+ 1.11f*/), -transform.localScale.x*Vector2.left, 35);
                        Debug.DrawRay(new Vector2(boxCollider2D.bounds.min.x /*- 0.01f*/, boxCollider2D.bounds.min.y /* 1.11f*/), -transform.localScale.x*Vector2.left*35, Color.cyan);
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
                    /*foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, sightRange)) {
                        if (obj.gameObject.CompareTag(Tags.ladder)) {
                            if (Vector3.Distance(obj.transform.position, mChaseTarget) < closestLadder) {
                                closestLadder = Vector3.Distance(obj.transform.position, mChaseTarget);
                                ladder = obj.gameObject;
                            }
                        }
                    }*/
                    if (mLadder != null) {
                        //bool climbing = false;
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
                            mChaseTarget = mLadder.GetComponent<Collider2D>().bounds.center/*+new Vector3(0,2f,0)*/;
                            mDirection = (mChaseTarget - transform.position);
                        }
                    }
                }
                else {
                    /*GameObject ladder = null;
                    bool climbing = false;
                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 1f)) {
                        if (obj.gameObject.CompareTag(Tags.ladder)) {
                            climbing = true;
                            ladder = obj.gameObject;
                        }
                    }

                    if (ladder != null && climbing && !enemy.GetComponent<Enemy>().controller.collisions.below) {
                        mChaseTarget = ladder.GetComponent<Collider2D>().bounds.max + new Vector3(0, 4f, 0);
                        mDirection = (mChaseTarget - transform.position);
                    }
                    else {*/
                        mChaseTarget = player.position;
                        mDirection = (mChaseTarget - transform.position);
                    //}
                }
                if (!enemy.isClimbing && player != null && InLineOfSight(player.GetComponent<Collider2D>(), weaponRange) && enemy.controller.collisions.below) {
                    StartCoroutine("ShootPlayer");
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

                /*float rotationZ = Mathf.Atan2(mDirection.y, mDirection.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

                if (mDirection.x>0) {
                    weapon.GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    weapon.GetComponent<SpriteRenderer>().flipY = true;
                }*/
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
            EventManager.TriggerEvent("EnemyControlled");
        }
        else {
            shooter.GetComponent<Enemy>().controlling = true;
            gun.GetComponent<EnemyWeapon>().untraversableLayers = gun.GetComponent<EnemyWeapon>().groundLayer;
            gun.GetComponent<EnemyWeapon>().currentCharge -= controlCost;
            gun.GetComponent<EnemyWeapon>().isLocked = true;
        }
        enemy.controlling = false;
        enemy.GetComponent<EnemyInput>().enabled = true;
        enemy.GetComponentInChildren<EnemyWeapon>().untraversableLayers = enemy.GetComponentInChildren<EnemyWeapon>().groundLayer+enemy.GetComponentInChildren<EnemyWeapon>().gunLayer;
        changingStatus = false;
    }

    IEnumerator TransitionEffects() {
        changingStatus = true;
        int seconds = 1;
        
        while (seconds > 0) {
            Instantiate(explosion, transform.position+Vector3.up, transform.rotation);
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        changingStatus = false;
    }

    IEnumerator ReturnToPatrol() {
        Debug.Log("Loosing Target...");
        yield return new WaitForSeconds(1/*timeToReturnPatrol*/);
        Debug.Log("...Target Lost, Returning!");
        animator.SetBool("PlayerInSight", false);
        losingTarget = false;
        
        //StartPatrol();
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
        EventManager.TriggerEvent("PlayerControlled");
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

    private void OnDestroy() {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.player);
        Instantiate(explosion, transform.position, transform.rotation);
        if (controlled && player != null) {
            EventManager.TriggerEvent("EnemyDestroyed");
            Player mPlayer = player.GetComponent<Player>();
            if (mPlayer != null) {
                mPlayer.controlling = false;
                mPlayer.GetComponentInChildren<GunController>().isLocked = false;
            }
        }
        //questo dovrebbe essere inutile devo farlo quando sparo a un altro nemico non on destroy
        if (shooter != null)
            if (controlled && shooter.gameObject.CompareTag(Tags.enemy)) {
                shooter.GetComponentInChildren<EnemyController>().controlled = false;
                shooter.GetComponent<Enemy>().controlling = true;
                enemy.GetComponentInChildren<EnemyWeapon>().untraversableLayers = enemy.GetComponentInChildren<EnemyWeapon>().groundLayer;
            }
    }

    /*private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.patrolSwitch) && collision.transform.IsChildOf(transform.parent) && !controlled && !animator.GetBool("PlayerInSight")) {
            setDestination(mDestination == startPoint ? endPoint : startPoint);
            //weapon.transform.rotation = Quaternion.Euler(0, 0, 180 - weapon.transform.rotation.eulerAngles.z);
            weapon.GetComponent<SpriteRenderer>().flipY = !weapon.GetComponent<SpriteRenderer>().flipY;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }*/

    /*private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.topLadder))
            collision.gameObject.layer = 1;
    }*/

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(endPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.DrawWireSphere(transform.position, 16);
       
       
    }
}
