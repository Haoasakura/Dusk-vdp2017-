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
    public bool playerInSight;

    public GameObject absorptionEffect;
    public DecisionTree chasingDT;
    public BehaviourTree patrolBT;
    public Animator animator;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private GameObject particleEffect;
    private Player enemy;
    private Player shooter = null;
    private GunController gun;
    private Vector3 mDirection;
    private Transform mDestination;
    private Transform mChaseTarget;

    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;

    private void Awake() {
        enemy = GetComponent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gun = GetComponentInChildren<GunController>();
        animator = GetComponent<Animator>(); 
    }
    void Start () {
        startPosition = transform.position;
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

        /*if (!controlled && !changingStatus) {
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
                gun.transform.rotation = Quaternion.Euler(0, 0, 180 - gun.transform.rotation.eulerAngles.z);
                gun.GetComponent<SpriteRenderer>().flipY = !gun.GetComponent<SpriteRenderer>().flipY;

            }
            if (gun.mTarget!=null && gun.mTarget.CompareTag(Tags.player) && gun.InLineOfSight(gun.mTarget.GetComponent<Collider2D>()))
                Debug.Log("Die Player, Die!");
        }
        else */{
            if (autodestruct && Input.GetKey(KeyCode.F))
                Destroy(gameObject);
        }

    }

    public void StartPatrol() {
        Debug.Log("Start Patrol");
        StartCoroutine("Patrol");
    }

    public void StartChase() {
        Debug.Log("Start Chase");
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

    IEnumerator ConrtolledOn(Transform gun) {
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

    IEnumerator Patrol() {
        SpriteRenderer gunSpriteRenderer = gun.GetComponent<SpriteRenderer>();
        while (true) {
            if (!controlled && !changingStatus) {
                float lastX = transform.localPosition.x;

                //enemy.controller.Move(mDirection * Time.deltaTime, mDirection);
                if (Mathf.Abs(mDirection.x) < 1) {
                    mDirection.x += 0.03f*Mathf.Sign(mDirection.x);
                }
                enemy.SetDirectionalInput(mDirection);
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
                if (gun.mTarget != null && gun.mTarget.CompareTag(Tags.player) && gun.InLineOfSight(gun.mTarget.GetComponent<Collider2D>())) {
                    animator.SetBool("PlayerInSight", true);
                    mChaseTarget = gun.mTarget;
                }
                
            }


            
            yield return null;
        }
    }

    IEnumerator Chase() {
        SpriteRenderer gunSpriteRenderer = gun.GetComponent<SpriteRenderer>();
        while (true) {
            if (!controlled && !changingStatus) {
                /*if (gun.mTarget != null && gun.mTarget.CompareTag(Tags.player) && gun.InLineOfSight(gun.mTarget.GetComponent<Collider2D>()))
                    animator.SetBool("PlayerInSight", true);*/

                float lastX = transform.localPosition.x;
                //enemy.controller.Move(mDirection * Time.deltaTime, mDirection);
                /*if (Mathf.Abs(mDirection.x) < 1) {
                    mDirection.x += 0.03f * Mathf.Sign(mDirection.x);
                }*/
                mDirection = (mChaseTarget.position - transform.position).normalized;
                enemy.SetDirectionalInput(mDirection);
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

                /*if (startPoint.position.x - transform.position.x > 0 || endPoint.position.x - transform.position.x < 0) {
                    //setDestination(mDestination == startPoint ? endPoint : startPoint);
                    gun.transform.rotation = Quaternion.Euler(0, 0, 180 - gun.transform.rotation.eulerAngles.z);
                    gun.GetComponent<SpriteRenderer>().flipY = !gun.GetComponent<SpriteRenderer>().flipY;

                }*/
                

            }



            yield return null;
        }
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
    }
}
