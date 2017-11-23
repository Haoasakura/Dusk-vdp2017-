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
    public LayerMask sightLayerMask;

    public GameObject absorptionEffect;
    public DecisionTree chasingDT;
    public BehaviourTree patrolBT;
    public Animator animator;

    private float weaponRange;
    private bool losingTarget=false;
    public bool shootingLights = false;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private GameObject particleEffect;
    private Enemy enemy;
    private Transform shooter = null;
    private GunController gun;
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
        gun = GetComponentInChildren<GunController>();      
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
    }
    void Start () {
        startPosition = transform.position;
        setDestination(startPoint);
        weaponRange = gun.gunRange;
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
        //for playtest purpose
        if (autodestruct && Input.GetKey(KeyCode.F))
                Destroy(gameObject);

    }

    public void StartPatrol() {
        Debug.Log("Start Patrol");
        StopCoroutine("Chase");
        StartCoroutine("Patrol");
    }

    public void StartChase() {
        Debug.Log("Start Chase");
        StopCoroutine("Patrol");
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
            RaycastHit2D hit = Physics2D.Raycast(gun.barrel.position, dir, range, sightLayerMask);
            if (hit.collider != null && hit.collider.gameObject.name == target.gameObject.name && target.gameObject.GetComponent<Player>().isVisible)
                return true;
        }
        return false;
    }

    IEnumerator Patrol() {
        SpriteRenderer gunSpriteRenderer = gun.GetComponent<SpriteRenderer>();
        while (true) {
            foreach (Collider2D obj in Physics2D.OverlapCircleAll(transform.position, sightRange)) {
                if (obj.gameObject.CompareTag(Tags.light)) {
                    if (!obj.gameObject.GetComponent<LightController>().lightStatus && !obj.gameObject.GetComponent<LightController>().changingStatus) {
                        obj.GetComponent<LightController>().SwitchOnOff(gun.transform);
                        shootingLights = true;

                    }
                }
            }

            if (!controlled && !changingStatus) {
                float lastX = transform.localPosition.x;
                if (Mathf.Abs(mDirection.x) < 1) {
                    mDirection.x += 0.03f * Mathf.Sign(mDirection.x);
                }
                if (shootingLights)
                    mDirection = Vector2.zero;

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
        SpriteRenderer gunSpriteRenderer = gun.GetComponent<SpriteRenderer>();

        while (true) {
            if (!controlled && !changingStatus) {
                if (InLineOfSight(player.GetComponent<Collider2D>(),weaponRange))
                    Debug.Log("Die, Die, Die!!!");
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

                    if(ladder != null && climbing && !enemy.GetComponent<Player>().controller.collisions.below) {
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

                enemy.SetDirectionalInput(mDirection.normalized);

                float rotationZ = Mathf.Atan2(mDirection.y, mDirection.x) * Mathf.Rad2Deg;
                gun.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                if (mDirection.x>0) {
                    spriteRenderer.flipX = false;
                    gun.GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    spriteRenderer.flipX = true;
                    gun.GetComponent<SpriteRenderer>().flipY = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator ConrtolledOn(Transform gun) {

        if(gun.GetComponentInParent<SpriteRenderer>().gameObject.CompareTag(Tags.player))
            shooter = gun.GetComponentInParent<Player>().transform;
        else
            shooter = gun.GetComponentInParent<Enemy>().transform;
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
        if(shooter.GetComponent<Player>()!=null)
            shooter.GetComponent<Player>().controlling = true;
        else
            shooter.GetComponent<Enemy>().controlling = true;
        enemy.controlling = false;
        enemy.GetComponent<EnemyInput>().enabled = true;
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
            if (controlled && shooter.gameObject.CompareTag(Tags.enemy)) {
                shooter.GetComponent<EnemyController>().controlled = false;
                if (shooter.GetComponent<Player>() != null)
                    shooter.GetComponent<Player>().controlling = true;
                else
                    shooter.GetComponent<Enemy>().controlling = true;
            }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.patrolSwitch) && !animator.GetBool("PlayerInSight")) {
            setDestination(mDestination == startPoint ? endPoint : startPoint);
            gun.transform.rotation = Quaternion.Euler(0, 0, 180 - gun.transform.rotation.eulerAngles.z);
            gun.GetComponent<SpriteRenderer>().flipY = !gun.GetComponent<SpriteRenderer>().flipY;
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
