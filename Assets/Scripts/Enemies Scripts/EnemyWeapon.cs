using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {

    public float TurnSpeed = 20f;
    public int currentCharge = 50;
    public int maxCharge = 100;
    public float gunRange;
    public bool controlling = false;
    public bool isLocked = false;

    [HideInInspector]
    public Transform mTarget;
    public Transform barrel;
    public Transform laserDirection;
    public Transform armTransform;
    public Transform pivot;
    public GameObject aimsight;
    public GameObject absorptionEffect;
    public GameObject dotsight;
    public SpriteRenderer arm;
    public SpriteRenderer armShadow;
    public Material aimMaterial;
    public Material idleMaterial;
    public LayerMask gunLayer;
    public LayerMask groundLayer;
    public LayerMask untraversableLayers;
    public EnemySoundManager soundManager;
    public GameObject particleEffect;
    protected Enemy enemy;
    protected EnemyController enemyController;
    protected Transform mTransform;
    public Coroutine lightningCoroutine = null;

    public EnemyController enemyControlled;
    public LineRenderer mLineRenderer;
    [HideInInspector]
    public DigitalRuby.LightningBolt.LightningBoltScript lightning;

    private void Awake() {
        gunRange = Mathf.Abs((laserDirection.position - barrel.position).x);
    }

    void Start() {
        enemy = GetComponentInParent<Enemy>();
        enemyController = GetComponentInParent<EnemyController>();
        mTransform = GetComponent<Transform>();
        mLineRenderer = aimsight.GetComponent<LineRenderer>();
        lightning = GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
        armTransform.position = pivot.position;
        dotsight.SetActive(false);
    }

    protected virtual void Update() {
        mLineRenderer.SetPosition(0, barrel.position);

        RaycastHit2D hit = Physics2D.Linecast(barrel.position, laserDirection.position, untraversableLayers);
        if (hit.collider != null && !hit.collider.gameObject.layer.Equals(9)) {
            dotsight.transform.position = hit.point;
            mLineRenderer.SetPosition(1, hit.point);
            lightning.EndPosition = hit.point;
            mTarget = hit.transform;
        }
        else {
            dotsight.transform.position = laserDirection.position;
            mLineRenderer.SetPosition(1, laserDirection.position);
            lightning.EndPosition = laserDirection.position;
            mTarget = null;
        }

        if (enemyController.controlled && !enemy.controlling) {
            float a = Input.GetAxis("HorizontalGun");
            float b = Input.GetAxis("VerticalGun");
            float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
            //rotazione della pistola
            if (Mathf.Abs(c) < 0.5)
            {
                dotsight.SetActive(false);
                mLineRenderer.material = idleMaterial;
            }
            else
            {
                dotsight.SetActive(true);
                mLineRenderer.material = aimMaterial;
            }

            if (Mathf.Abs(c) > 0.9 && !isLocked) {
                float angleRot = -Mathf.Sign(b) * Mathf.Rad2Deg * Mathf.Acos(a / c);

                if(GetComponentInParent<Enemy>().transform.localScale.x>0)
                    transform.parent.rotation = Quaternion.Euler(0f, 0f, 73.4f + angleRot);
                else
                    transform.parent.rotation = Quaternion.Euler(0f, 0f, 93+angleRot);
                
                //mantiene la sprite dell'arma nel verso giusto
                if (mTransform.rotation.eulerAngles.z % 270 < 90 && mTransform.rotation.eulerAngles.z % 270 >= 0) {
                    GetComponent<SpriteRenderer>().flipY = false;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    GetComponent<SpriteRenderer>().flipY = true;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = true;
                }
            }

            if (Input.GetButtonDown("Fire1")) {
                soundManager.EmptyGunshot();
                lightning.Trigger();
                mLineRenderer.enabled = false;
                if (mTarget != null) {
                    if (mTarget.CompareTag(Tags.light)) {
                        LightController currentLight = mTarget.GetComponent<LightController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentLight.changingStatus)
                            if ((currentLight.lightStatus && (maxCharge - currentCharge) >= currentLight.lightCharge) || (!currentLight.lightStatus && currentCharge >= currentLight.lightCharge)) {
                                mTarget.GetComponent<LightController>().SwitchOnOff(transform);
                                if (currentLight.lightStatus) {
                                    lightningCoroutine = StartCoroutine(LightningEffectOn(mTarget.GetComponent<LightController>().switchTime, true));
                                    StartCoroutine("TrailingEffectOff", mTarget.GetComponent<LightController>().switchTime);
                                }
                                else {
                                    lightningCoroutine = StartCoroutine(LightningEffectOn(mTarget.GetComponent<LightController>().switchTime, false));
                                    StartCoroutine("TrailingEffectOn", mTarget.GetComponent<LightController>().switchTime);
                                }
                                isLocked = true;
                            }
                    }
                    else if (mTarget.CompareTag(Tags.machinery)) {
                        MachineryController currentMachinery = mTarget.GetComponent<MachineryController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentMachinery.changingStatus)
                            if ((currentMachinery.powered && (maxCharge - currentCharge) >= currentMachinery.powerCharge) || (!currentMachinery.powered && currentCharge >= currentMachinery.powerCharge)) {
                                currentMachinery.SwitchOnOff(transform);
                                if (currentMachinery.powered) {
                                    lightningCoroutine = StartCoroutine(LightningEffectOn(mTarget.GetComponent<MachineryController>().switchTime, true));
                                    StartCoroutine("TrailingEffectOff", mTarget.GetComponent<MachineryController>().switchTime);
                                }
                                else {
                                    lightningCoroutine = StartCoroutine(LightningEffectOn(mTarget.GetComponent<MachineryController>().switchTime, false));
                                    StartCoroutine("TrailingEffectOn", mTarget.GetComponent<MachineryController>().switchTime);
                                }
                                isLocked = true;
                            }
                    }
                    else if (mTarget.CompareTag(Tags.enemy)) {
                        enemyControlled = mTarget.GetComponent<EnemyController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && currentCharge >= enemyControlled.controlCost && !enemyControlled.gettingShoot) {
                            enemyControlled.ControlledOn(transform);
                            lightningCoroutine = StartCoroutine(LightningEffectOn(mTarget.GetComponent<EnemyController>().switchTime, false));
                            StartCoroutine("TrailingEffectOn", mTarget.GetComponent<EnemyController>().switchTime);
                            StartCoroutine("AlertEnemies");
                            isLocked = true;
                        }
                    }
                }
            }
            if (Input.GetButtonUp("Fire1")) {
                isLocked = false;
                mLineRenderer.enabled = true;
                StopCoroutine(lightningCoroutine);
                StopCoroutine("TrailingEffectOn");
                StopCoroutine("TrailingEffectOff");
                Destroy(particleEffect);
            }
        }
    }

    public bool InLineOfSight(Collider2D target) {
        if (target != null) {
            RaycastHit2D hit = Physics2D.Raycast(barrel.position, (target.transform.position - transform.position), gunRange, gunLayer);
            if (hit.collider != null && hit.collider.gameObject.name == target.gameObject.name)
                return true;
        }
        return false;
    }

    IEnumerator AlertEnemies() {
        while (enemyController.controlled) {
            BoxCollider2D coll = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<BoxCollider2D>();
            foreach (GameObject _enemy in GameObject.FindGameObjectsWithTag(Tags.enemy)) {
                if (coll.bounds.Contains(_enemy.transform.position + new Vector3(0, 0, -10)) && !_enemy.transform.GetComponent<Animator>().GetBool("EnemyTraitor") && Mathf.Abs(enemy.transform.position.y-_enemy.transform.position.y)<1.6f) {
                    if (!_enemy.transform.GetComponent<Animator>().GetBool("PlayerInSight") && !_enemy.transform.GetComponent<EnemyController>().changingStatus && !_enemy.transform.GetComponent<EnemyController>().controlled) {
                        _enemy.gameObject.GetComponent<Animator>().SetBool("EnemyTraitor", true);
                    }
                }
            }
            yield return null;
        }
    }

    public IEnumerator LightningEffectOn(float switchTime, bool isSucking) {
        float startTime = Time.time;
        if (isSucking)
        {
            while ((Time.time - startTime) < switchTime)
            {
                SoundManager.Instance.Gunshot((Time.time - startTime));
                lightning.Trigger();
                yield return null;
            }
        }
        else
        {
            while ((Time.time - startTime) < switchTime)
            {
                SoundManager.Instance.Gunshot(switchTime - (Time.time - startTime));
                lightning.Trigger();
                yield return null;
            }
        }
        SoundManager.Instance.GunshotStop();
        SoundManager.Instance.LightOnSound(isSucking);
        isLocked = false;
    }

     IEnumerator TrailingEffectOn(float switchTime) {
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while ((Time.time - startTime) < switchTime) {
            particleEffect.transform.position = Vector3.Lerp(barrel.transform.position, lightning.EndPosition, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
        Destroy(particleEffect);
    }

    IEnumerator TrailingEffectOff(float switchTime) {
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while ((Time.time - startTime) < switchTime) {
            particleEffect.transform.position = Vector3.Lerp(lightning.EndPosition, barrel.transform.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
        Destroy(particleEffect);
    }
}
