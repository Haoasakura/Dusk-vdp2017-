using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public bool ranged = true;
    public float secondsForOneLength = 5f;
    public float speed = 10f;
    public float huntSpeed = 0f;
    public float switchTime = 3f;
    public float sightRange=10f;
    public int controlCost = 25;
    public int timeToReturnPatrol = 3;
    public int transitionDuration = 1;
    public bool controlled = false;
    public bool changingStatus = false;
    public bool autodestruct = true;
    public bool playerInSight;
    public bool flipStartDir = false;
    public LayerMask sightLayerMask;

    public GameObject explosion;
    public GameObject alert;
    public GameObject absorptionEffect;
    public GameObject enemySoundManager;
    public DecisionTree chasingDT;
    public BehaviourTree patrolBT;
    public Animator animator;

    private float weaponRange;
    private float lastX;
    public bool losingTarget=false;
    public bool shootingLights = false;
    public bool gettingShoot=false;

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
    private Transform enemyTarget;
    private bool isClimbing = false;
    private bool isChasing = false;
    private bool inTransition = false;
    public bool pitInPath = false;


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
                Destroy(particleEffect);
                changingStatus = false;
                controlled = false;
                gettingShoot = false;
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
        //Debug.Log("Start Patrol")        
        StopCoroutine("Patrol");
        StopCoroutine("HuntTraitor");
        StopCoroutine("Chase");
        StopCoroutine("RandomlyBark");
        enemy.moveMinSpeed = 2f;
        enemyTarget = null;
        animator.SetBool("PlayerInSight", false);
        animator.SetBool("EnemyTraitor", false);
        playerInSight = false;
        weapon.StopCoroutine("TrailingEffectOn");
        if (weapon.lightningCoroutine != null)
        {
            weapon.StopCoroutine(weapon.lightningCoroutine);
        }
        Destroy(weapon.particleEffect);
        if (flipStartDir)
            setDestination(endPoint);
        weapon.transform.parent.rotation = Quaternion.Euler(0f, 0f, transform.localScale.x>0?73.4f:-73.4f);
        if (isChasing)
        {
            SoundManager.Instance.PlayNormalSoundtrack();
            isChasing = false;
        }
        StartCoroutine("Patrol");
        weapon.mLineRenderer.material = weapon.idleMaterial;
    }

    public void StartChase() {
        //Debug.Log("Start Chase");
        StopCoroutine("Patrol");
        StopCoroutine("HuntTraitor");
        StopCoroutine("Chase");
        StopCoroutine("RandomlyBark");
        enemyTarget = null;
        enemy.moveMinSpeed = huntSpeed;
        animator.SetBool("PlayerInSight", true);
        animator.SetBool("EnemyTraitor", false);
        playerInSight = true;
        SoundManager.Instance.PlayChaseSoundtrack();
        StartCoroutine("TransitionEffects");
        isChasing = true;
        StartCoroutine("Chase");
        StartCoroutine("RandomlyBark");
    }

    public void StartTraitorHunt() {
        //Debug.Log("Start Tritor Hunt");
        StopCoroutine("Patrol");
        StopCoroutine("HuntTraitor");
        StopCoroutine("Chase");
        StopCoroutine("RandomlyBark");
        enemy.moveMinSpeed = huntSpeed;
        animator.SetBool("EnemyTraitor", true);
        StartCoroutine("TransitionEffects");
        StartCoroutine("HuntTraitor");
        StartCoroutine("RandomlyBark");
    }

    IEnumerator RandomlyBark()
    {
        float init = Time.deltaTime;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            float t = Time.deltaTime;
            float r = Random.Range(0, t - init);
            if (r > 0)
            {
                enemySoundManager.GetComponent<EnemySoundManager>().Hunt();
                init = Time.deltaTime;
            }
        }
    }

    public void ControlledOn(Transform gun) {
        StartCoroutine("ConrtolledOn", gun);
    }

    private void setDestination(Transform destination) {
        mDestination = destination;
        mDirection = (mDestination.position - transform.position).normalized/10f;
    }

    private bool InLineOfShot(Transform target, float range) {
            Vector3 dir = (weapon.laserDirection.position - weapon.barrel.position);
            dir.y = 0;
            RaycastHit2D hit = Physics2D.Raycast(weapon.barrel.position, dir, range, sightLayerMask);
        if (hit.transform) {
            if (target.CompareTag(Tags.player)) {
                if (hit.collider != null && hit.collider.gameObject.name == target.name && target.GetComponent<Player>() != null && target.GetComponent<Player>().isVisible)
                    return true;
            }
            else if (target.CompareTag(Tags.enemy)) {
                if (hit.collider != null && hit.collider.gameObject.name == target.name && hit.collider.GetComponent<EnemyController>().controlled)
                    return true;
            }

        }
        return false;
    }

    public bool InLineOfSight(Transform target, float range) {
        Vector3 dir = (weapon.laserDirection.position - weapon.barrel.position);
        dir.y = 0;
       // RaycastHit2D hit = Physics2D.Raycast(weapon.barrel.position, dir, range, sightLayerMask);
        RaycastHit2D[] hits = Physics2D.RaycastAll(weapon.barrel.position, dir, range, sightLayerMask);
        foreach (RaycastHit2D hit in hits) {
            if (hit.transform) {
                if (target.CompareTag(Tags.player)) {
                    if (hit.collider != null && hit.collider.gameObject.layer == 8)
                        return false;
                    if (hit.collider != null && hit.collider.gameObject.name == target.name && target.GetComponent<Player>() != null && target.GetComponent<Player>().isVisible)
                        return true;
                }
                else if (target.CompareTag(Tags.enemy)) {
                    if (hit.collider != null && hit.collider.gameObject.name == target.name && hit.collider.GetComponent<EnemyController>().controlled)
                        return true;
                }

            }
        }
        return false;
    }

    IEnumerator Patrol() {

        while (true) {

            if ((startPoint.position.x - transform.position.x > 0 || enemyController2D.collisions.left) && !controlled && !animator.GetBool("PlayerInSight"))
                setDestination(endPoint);

            else if ((endPoint.position.x - transform.position.x < 0 || enemyController2D.collisions.right) && !controlled && !animator.GetBool("PlayerInSight"))
                setDestination(startPoint);

            if (ranged && !animator.GetBool("PlayerInSight") && !controlled &&transform.position.x > startPoint.position.x && transform.position.x < endPoint.position.x) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(weapon.pivot.position.x, transform.GetComponent<BoxCollider2D>().bounds.max.y+0.01f), Vector2.up);
                if(hit && hit.collider.CompareTag(Tags.light)) {
                    LightController lightController = hit.collider.gameObject.GetComponent<LightController>();
                    if (!lightController.lightStatus && !lightController.changingStatus && weapon.currentCharge > 0) {
                        weapon.armTransform.rotation = Quaternion.Euler(0.0f, 0.0f, enemy.transform.localScale.x * 159.1f);
                        lightController.SwitchOnOff(weapon.transform);
                        weapon.StartCoroutine("TrailingEffectOn", lightController.switchTime);
                        weapon.StartCoroutine(weapon.LightningEffectOn(lightController.switchTime, true));
                        shootingLights = true;
                    }
                }
            }

            if (!controlled && !changingStatus) {
                //questo è il caso del return to patrol
                mDirection = (mDestination.position - transform.position);
                if (mDirection.y > 0.3f || isClimbing) {
                    
                    float closestLadder = 10000f;
                    GameObject ladder = null;
                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 100f)) {
                        if ((transform.position.y - startPoint.position.y) > 0) {
                            if (((obj.CompareTag(Tags.ladder) && !obj.name.Contains("MidLadder")) || obj.CompareTag(Tags.baseLadder) || obj.CompareTag(Tags.topLadder)) && (obj.transform.position.y - boxCollider2D.bounds.min.y > -15)) {
                                if (Vector3.Distance(obj.transform.position, transform.position) < closestLadder) {
                                    closestLadder = Vector3.Distance(obj.transform.position, transform.position);
                                    ladder = obj.transform.gameObject;
                                }
                            }
                        }
                        else {
                            if (((obj.CompareTag(Tags.ladder) && !obj.name.Contains("MidLadder")) || obj.CompareTag(Tags.baseLadder) || obj.CompareTag(Tags.topLadder)) && (obj.transform.position.y - boxCollider2D.bounds.min.y > 0 && obj.transform.position.y - boxCollider2D.bounds.min.y < 5)) {
                                if (Vector3.Distance(obj.transform.position, transform.position) < closestLadder) {
                                    closestLadder = Vector3.Distance(obj.transform.position, transform.position);
                                    ladder = obj.transform.gameObject;
                                }
                            }
                        }
                    }
                    if (ladder != null) {

                        foreach (Collider2D obj in Physics2D.OverlapCircleAll(new Vector2(boxCollider2D.bounds.center.x, boxCollider2D.bounds.min.y), 0.1f)) {
                            if ((obj.CompareTag(Tags.ladder) && !obj.name.Contains("MidLadder")) || obj.CompareTag(Tags.baseLadder) || obj.CompareTag(Tags.topLadder)) {
                                isClimbing = true;
                                break;
                            }
                            else {
                                isClimbing = false;
                            }
                        }

                        if (isClimbing) {
                            if(!ladder.CompareTag(Tags.topLadder))
                                if(ladder.transform.parent.CompareTag(Tags.ladder))
                                    ladder=ladder.transform.parent.Find("TopLadder").gameObject;
                                else
                                    ladder = ladder.transform.Find("TopLadder").gameObject;
                            mChaseTarget = new Vector3(ladder.GetComponent<Collider2D>().bounds.center.x, ladder.GetComponent<Collider2D>().bounds.max.y) + new Vector3(0, 10f, 0);
                            mDirection = (mChaseTarget - transform.position);
                            mDirection.y += 100;
                            transform.position += new Vector3(0f, 0.75f, 0f);
                        }
                        else {
                            mChaseTarget = ladder.GetComponent<Collider2D>().bounds.center;
                            mDirection = (mChaseTarget - transform.position);  
                        }
                        
                        if (ladder.transform.Find("TopLadder"))
                            ladder.transform.Find("TopLadder").gameObject.layer = 9;
                    }
                }
                else if(-mDirection.y > 0.3f) {
                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(boxCollider2D.bounds.center, 0.1f)) {
                        if ((obj.CompareTag(Tags.ladder) && !obj.name.Contains("MidLadder")) || obj.CompareTag(Tags.baseLadder) || obj.CompareTag(Tags.topLadder)) {
                            obj.gameObject.layer = 9;
                            break;
                        }
                    }
                    mDirection = (startPosition - transform.position);
                }
                else if (!(transform.position.x > startPoint.position.x && transform.position.x < endPoint.position.x)) {
                    mDirection = (startPosition - transform.position);
                }
                else {
                    mDirection = (mDestination.position - transform.position);
                    if (Mathf.Abs(mDirection.x) < 1)
                        mDirection.x += 0.03f * Mathf.Sign(mDirection.x);
                }

                if (shootingLights || gettingShoot)
                    mDirection = Vector2.zero;

                if (!changingStatus && player != null)
                    if (InLineOfSight(player, sightRange) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(transform.position + new Vector3(0, 0, -10)) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(player.position + new Vector3(0, 0, -10))) {
                        animator.SetBool("PlayerInSight", true);
                        playerInSight = true;
                        mChaseTarget = player.position;
                    }
                enemy.SetDirectionalInput(mDirection.normalized);
            }
            else if (changingStatus || enemy.controlling || gettingShoot)
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
                    }
                    else {
                        hit = Physics2D.Raycast(new Vector2(boxCollider2D.bounds.min.x - 0.01f, boxCollider2D.bounds.min.y), -transform.localScale.x*Vector2.left, 35);
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
                        foreach (Collider2D obj in Physics2D.OverlapCircleAll(new Vector2(boxCollider2D.bounds.center.x, boxCollider2D.bounds.min.y), 0.1f)) {
                            if ((obj.CompareTag(Tags.ladder) && !obj.name.Contains("MidLadder")) || obj.CompareTag(Tags.baseLadder) || obj.CompareTag(Tags.topLadder)) {
                                isClimbing = true;
                                break;
                            }
                            else
                                isClimbing = false;
                        }
                        if (isClimbing) {
                            if (!mLadder.CompareTag(Tags.topLadder))
                                if (mLadder.transform.parent.CompareTag(Tags.ladder))
                                    mLadder = mLadder.transform.parent.Find("TopLadder").gameObject;
                                else
                                    mLadder = mLadder.transform.Find("TopLadder").gameObject;

                            mChaseTarget = new Vector3(mLadder.GetComponent<Collider2D>().bounds.center.x, mLadder.GetComponent<Collider2D>().bounds.max.y) + new Vector3(0, 10f, 0);
                            mDirection = (mChaseTarget - transform.position);
                            mDirection.y += 100;
                            transform.position += new Vector3(0f,0.75f,0f);
                        }
                        else {
                            mChaseTarget = mLadder.GetComponent<Collider2D>().bounds.center;
                            mDirection = (mChaseTarget - transform.position);
                        }

                        if (mLadder.transform.Find("TopLadder"))
                            mLadder.transform.Find("TopLadder").gameObject.layer = 9;
                    }
                }
                else if (-mDirection.y > 1.6f) {

                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(boxCollider2D.bounds.center, 0.1f)) {
                        if ((obj.CompareTag(Tags.ladder) && !obj.name.Contains("MidLadder")) || obj.CompareTag(Tags.baseLadder) || obj.CompareTag(Tags.topLadder)) {
                            obj.gameObject.layer = 9;
                            break;
                        }
                    }

                    float closestLadder = 10000f;
                    GameObject ladder = null;
                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 100f)) {
                        if ((transform.position.y - player.position.y) > 0) {
                            if (obj.CompareTag(Tags.baseLadder) && (obj.transform.position.y - boxCollider2D.bounds.center.y < -1.6f)) {
                                if (Vector3.Distance(obj.transform.position, transform.position) < closestLadder) {
                                    closestLadder = Vector3.Distance(obj.transform.position, transform.position);
                                    ladder = obj.transform.gameObject;
                                }
                            }
                        }

                        if (ladder != null) {
                            mChaseTarget = ladder.GetComponent<Collider2D>().bounds.center;
                            mDirection = (mChaseTarget - transform.position);
                        }

                    }
                    
                }
                else {
                    mChaseTarget = player.position;
                    mDirection = (mChaseTarget - transform.position);
                }
                bool collidingWithTarget = false;
                foreach (Collider2D obj in Physics2D.OverlapCircleAll(boxCollider2D.bounds.min, 0.1f)) {
                    if (player && obj.CompareTag(Tags.player) && player.GetComponent<Player>().isVisible) {
                        collidingWithTarget = true;
                        break;
                    }
                    else
                        collidingWithTarget = false;
                }

                if (!enemy.isClimbing && player != null && (InLineOfShot(player, weaponRange) || collidingWithTarget ) && enemy.controller.collisions.below && !changingStatus && !gettingShoot)
                    if (ranged)
                        StartCoroutine("ShootPlayer");
                    else
                        StartCoroutine("MeleeAttack",player);
                    

                //controllo per evitare di cadere nella sua morte
                pitInPath = false;
                foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 1.3f)) {
                    if (obj.gameObject.CompareTag(Tags.trapdoor) && obj.gameObject.transform.parent.GetComponent<TrapdoorController>().isOpen) {
                        pitInPath = true;
                    }
                }
                foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, 2.5f)) {
                    if (obj.gameObject.CompareTag(Tags.deathCollider)) {
                        if (pitInPath && (player.position - transform.position).x > (obj.gameObject.transform.position - transform.position).x) {
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
            else if (changingStatus || enemy.controlling || gettingShoot)
                enemy.SetDirectionalInput(Vector2.zero);

            yield return null;
        }
    }

    IEnumerator HuntTraitor() {
        //da mettere condizioni di uscita, lo sparo al nemico per uccidere e fare lo stato nell'animator
        while (true) {
            Transform _enemyTarget = null;

            if (!controlled && !changingStatus && !shootingLights) {
                foreach (GameObject _enemy in GameObject.FindGameObjectsWithTag(Tags.enemy)) {
                    if (_enemy.GetComponent<EnemyController>().controlled) {
                        _enemyTarget = _enemy.transform;
                        break;
                    }
                }
                if (_enemyTarget != null) {
                    if (enemyTarget != null && enemyTarget.parent != _enemyTarget.parent) {
                        StopCoroutine("ShootEnemy");
                        enemyTarget.GetComponent<EnemyController>().gettingShoot = false;
                        enemy.gameObject.GetComponent<Animator>().SetBool("EnemyTraitor", false);
                    }
                    enemyTarget = _enemyTarget;
                    mChaseTarget = enemyTarget.position;
                    mDirection = (mChaseTarget - transform.position);

                    if (Mathf.Abs(enemy.transform.position.y - enemyTarget.transform.position.y) > 1.6f) {
                        animator.SetBool("EnemyTraitor", false);
                        shootingLights = false;
                    }

                    bool collidingWithTarget = false;
                    foreach (Collider2D obj in Physics2D.OverlapCircleAll(boxCollider2D.bounds.center, 0.1f)) {
                        if (enemyTarget && obj.transform.parent==enemyTarget.parent && enemyTarget.GetComponent<EnemyController>().controlled) {
                            collidingWithTarget = true;
                            break;
                        }
                        else
                            collidingWithTarget = false;
                    }

                    if (!enemy.isClimbing && (InLineOfShot(enemyTarget, weaponRange) || collidingWithTarget) && enemy.controller.collisions.below && !changingStatus && !inTransition && !gettingShoot && enemy.gameObject.GetComponent<Animator>().GetBool("EnemyTraitor")) {
                        EnemyController enemyTargetC = enemyTarget.GetComponent<EnemyController>();
                        enemyTargetC.StopCoroutine("ShootEnemy");
                        EnemyWeapon _enemyWeapon = enemyTargetC.GetComponentInChildren<EnemyWeapon>();
                        Debug.Log(_enemyWeapon.GetType().Equals(typeof(EnemyWeapon)));
                        if (_enemyWeapon.GetType().Equals(typeof(EnemyWeapon))) {
                            _enemyWeapon.enemyControlled.changingStatus = false;
                            _enemyWeapon.enemyControlled.gettingShoot = false;
                            _enemyWeapon.enemyControlled.StopCoroutine("ConrtolledOn");

                            _enemyWeapon.mLineRenderer.enabled = true;
                            if (_enemyWeapon.lightningCoroutine != null) {
                                _enemyWeapon.StopCoroutine(_enemyWeapon.lightningCoroutine);
                            }
                            _enemyWeapon.StopCoroutine("TrailingEffectOn");
                            _enemyWeapon.StopCoroutine("TrailingEffectOff");
                            Destroy(_enemyWeapon.particleEffect);
                        }
                        if(ranged)
                            StartCoroutine("ShootEnemy", enemyTarget);
                        else
                            StartCoroutine("MeleeAttack",enemyTarget);

                    }

                    if (shootingLights || gettingShoot || inTransition)
                        mDirection = Vector2.zero;

                    if (!changingStatus && player != null)
                        if (InLineOfSight(player, sightRange) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(transform.position + new Vector3(0, 0, -10)) && GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Collider2D>().bounds.Contains(player.position + new Vector3(0, 0, -10))) {
                            animator.SetBool("PlayerInSight", true);
                            playerInSight = true;
                            mChaseTarget = player.position;
                        }

                    enemy.SetDirectionalInput(mDirection.normalized);
                }
                else {
                    enemy.gameObject.GetComponent<Animator>().SetBool("EnemyTraitor", false);
                }
            }
            else if (changingStatus || enemy.controlling || gettingShoot || inTransition)
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
        
        int seconds = (int) switchTime;
        /*if (gun.GetType().Equals("EnemyMeleeWeapon"))
            seconds = 0.3f;*/

        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        controlled = true;
        if (shooter.GetComponent<Player>() != null) {
            shooter.GetComponent<Player>().controlling = true;
            player.GetComponent<Player>().SetDirectionalInput(Vector2.zero);
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
        inTransition = true;
        int seconds = 1;
        
        while (seconds > 0) {
            Instantiate(alert, transform.position+Vector3.up*1.6f, transform.rotation);
            yield return new WaitForSeconds(transitionDuration);
            seconds--;
        }
        //weapon.mLineRenderer.material = weapon.aimMaterial;
        weapon.transform.parent.rotation = Quaternion.Euler(0f, 0f, transform.localScale.x > 0 ? 73.4f : -73.4f);
        changingStatus = false;
        inTransition = false;
        if(shootingLights) {
            weapon.StopCoroutine("TrailingEffectOn");
            if (weapon.lightningCoroutine != null)
            {
                weapon.StopCoroutine(weapon.lightningCoroutine);
            }
            Destroy(weapon.particleEffect);
            shootingLights = false;
        }
    }

    IEnumerator ReturnToPatrol() {
        //Debug.Log("Loosing Target...");
        yield return new WaitForSeconds(timeToReturnPatrol);
        //Debug.Log("...Target Lost, Returning!");
        animator.SetBool("PlayerInSight", false);
        playerInSight = false;
        losingTarget = false;
    }

    IEnumerator ShootPlayer() {
        if (!shootingLights) {
            weapon.StopCoroutine("TrailingEffectOn");
            if (weapon.lightningCoroutine != null)
            {
                weapon.StopCoroutine(weapon.lightningCoroutine);
            }
            Destroy(weapon.particleEffect);
            weapon.StartCoroutine("TrailingEffectOn", switchTime);
            weapon.StartCoroutine(weapon.LightningEffectOn(switchTime, false));
        }
        
        shootingLights = true;
        player.GetComponent<Player>().controlling = true;
        player.GetComponent<Player>().SetDirectionalInput(Vector2.zero);
        player.GetComponent<Player>().moveMinSpeed = 0;
        player.GetComponent<Player>().moveMaxSpeed = 0;
        player.GetComponent<Player>().accelerationTimeAirborne = 0;
        player.GetComponent<Player>().accelerationTimeGrounded = 0;
        player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponent<Controller2D>().gravityOnFall = 0f;
        yield return new WaitForSeconds(switchTime);
        EventManager.TriggerEvent("PlayerControlled");
        weapon.StopCoroutine("TrailingEffectOn");
        if (weapon.lightningCoroutine != null)
        {
            weapon.StopCoroutine(weapon.lightningCoroutine);
        }
    }

    IEnumerator ShootEnemy(Transform _enemy) {
        if (!shootingLights) {
            weapon.StopCoroutine("TrailingEffectOn");
            if (weapon.lightningCoroutine != null)
            {
                weapon.StopCoroutine(weapon.lightningCoroutine);
            }
            Destroy(weapon.particleEffect);
            weapon.StartCoroutine("TrailingEffectOn", switchTime);
            weapon.StartCoroutine(weapon.LightningEffectOn(switchTime, false));
        }
        shootingLights = true;
        _enemy.GetComponent<EnemyController>().gettingShoot = true;
        EnemyWeapon _enemyWeapon = _enemy.GetComponentInChildren<EnemyWeapon>();
        if (_enemyWeapon.lightningCoroutine != null)
        {
            _enemyWeapon.StopCoroutine(_enemyWeapon.lightningCoroutine);
        }
        _enemyWeapon.StopCoroutine("TrailingEffectOn");
        _enemyWeapon.StopCoroutine("TrailingEffectOff");
        SoundManager.Instance.GunshotStop();
        Destroy(_enemyWeapon.particleEffect);
        _enemy.GetComponent<EnemyController>().controlled = true;
        _enemy.GetComponent<Enemy>().SetDirectionalInput(Vector2.zero);
        _enemy.GetComponent<EnemyInput>().enabled = false;
        yield return new WaitForSeconds(switchTime);
        if(_enemy && _enemy.parent.gameObject)
            _enemy.GetComponent<EnemyController>().Kill();
        weapon.StopCoroutine("TrailingEffectOn");
        if (weapon.lightningCoroutine != null)
        {
            weapon.StopCoroutine(weapon.lightningCoroutine);
        }
        weapon.StopCoroutine("LightningEffectOn");
    }

    IEnumerator MeleeAttack(Transform _target) {
        if (!shootingLights) {
            weapon.StartCoroutine("MeleeAttack", _target);
        }

        shootingLights = true;
        
        yield return new WaitForSeconds(1f);
        weapon.transform.parent.rotation = Quaternion.Euler(0f, 0f, transform.localScale.x > 0 ? 73.4f : -73.4f);
        playerInSight = false;
        shootingLights = false;
        animator.SetBool("PlayerInSight", false);
    }

    private void OnDestroy() {

        if (isChasing)
        {
            SoundManager.Instance.PlayNormalSoundtrack();
        }

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

    public void Kill()
    {
        if (GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<BoxCollider2D>()) {
            BoxCollider2D coll = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<BoxCollider2D>();
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy)) {
                if (coll.bounds.Contains(enemy.transform.position + new Vector3(0, 0, -10))) {
                    if (!enemy.transform.GetComponent<Animator>().GetBool("PlayerInSight") && !enemy.transform.GetComponent<EnemyController>().changingStatus && !enemy.transform.GetComponent<EnemyController>().controlled) {
                        enemy.gameObject.GetComponent<Animator>().SetBool("EnemyTraitor", false);
                        enemy.GetComponent<EnemyController>().shootingLights = false;
                    }
                }
            }
        }
        
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(transform.parent.gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(endPoint.position, transform.GetComponent<BoxCollider2D>().size);

        Gizmos.DrawWireSphere(transform.position, 1);
        
        Gizmos.DrawWireSphere(new Vector2(GetComponent<BoxCollider2D>().bounds.center.x, GetComponent<BoxCollider2D>().bounds.center.y), 1.1f);
       
       
    }
}
