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
    public GameObject aimsight;
    public GameObject absorptionEffect;
    public SpriteRenderer arm;
    public SpriteRenderer armShadow;
    public Material aimMaterial;
    public Material idleMaterial;
    public LayerMask gunLayer;
    public LayerMask groundLayer;
    public LayerMask untraversableLayers;
    public EnemySoundManager soundManager;

    private GameObject particleEffect;
    private Enemy enemy;
    private Transform mTransform;
    private EnemyController enemyControlled;
    private LineRenderer mLineRenderer;
    private DigitalRuby.LightningBolt.LightningBoltScript lightning;

    private void Awake() {
        gunRange = Mathf.Abs((laserDirection.position - barrel.position).x);
    }
    void Start() {
        enemy = GetComponentInParent<Enemy>();
        mTransform = GetComponent<Transform>();
        mLineRenderer = aimsight.GetComponent<LineRenderer>();
        lightning = GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
    }

    void Update() {

        mLineRenderer.SetPosition(0, barrel.position);

        RaycastHit2D hit = Physics2D.Linecast(barrel.position, laserDirection.position, untraversableLayers);
        if (hit.collider != null && !hit.collider.gameObject.layer.Equals(9)) {
            mLineRenderer.SetPosition(1, hit.point);
            lightning.EndPosition = hit.point;
            mTarget = hit.transform;
        }
        else {
            mLineRenderer.SetPosition(1, laserDirection.position);
            lightning.EndPosition = laserDirection.position;
            mTarget = null;
        }

        if (!enemy.controlling) {

            float a = Input.GetAxis("HorizontalGun");
            float b = Input.GetAxis("VerticalGun");
            float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
            //rotazione della pistola

            if (Mathf.Abs(c) < 0.5) {
                mLineRenderer.material = idleMaterial;
            }
            else {
                mLineRenderer.material = aimMaterial;
            }

            if (Mathf.Abs(c) > 0.9 && !isLocked) {
                float angleRot = -Mathf.Sign(b) * Mathf.Rad2Deg * Mathf.Acos(a / c);

                mTransform.rotation = Quaternion.Euler(0f, 0f, angleRot);

                //mantiene la sprite dell'arma nel verso giusto
                if (mTransform.rotation.eulerAngles.z % 270 < 90 && mTransform.rotation.eulerAngles.z % 270 > 0) {
                    GetComponent<SpriteRenderer>().flipY = false;
                    arm.flipX = false;
                    armShadow.flipX = false;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    GetComponent<SpriteRenderer>().flipY = true;
                    arm.flipX = true;
                    armShadow.flipX = true;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = true;
                }
            }

            if (Input.GetButtonDown("Fire1")) {
                soundManager.EmptyGunshot();
                Debug.Log("Hello");
                lightning.Trigger();
                mLineRenderer.enabled = false;
                if (mTarget != null) {
                    if (mTarget.CompareTag(Tags.light)) {
                        LightController currentLight = mTarget.GetComponent<LightController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentLight.changingStatus)
                            if ((currentLight.lightStatus && (maxCharge - currentCharge) >= currentLight.lightCharge) || (!currentLight.lightStatus && currentCharge >= currentLight.lightCharge)) {
                                mTarget.GetComponent<LightController>().SwitchOnOff(transform);
                                StartCoroutine("LightningEffectOn", mTarget.GetComponent<LightController>().switchTime);
                                if (currentLight.lightStatus) {
                                    StartCoroutine("TrailingEffectOff", mTarget.GetComponent<LightController>().switchTime);
                                }
                                else {
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
                                StartCoroutine("LightningEffectOn", mTarget.GetComponent<MachineryController>().switchTime);
                                if (currentMachinery.powered) {
                                    StartCoroutine("TrailingEffectOff", mTarget.GetComponent<MachineryController>().switchTime);
                                }
                                else {
                                    StartCoroutine("TrailingEffectOn", mTarget.GetComponent<MachineryController>().switchTime);
                                }
                                isLocked = true;
                            }
                    }
                    else if (mTarget.CompareTag(Tags.enemy)) {
                        enemyControlled = mTarget.GetComponent<EnemyController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && currentCharge >= enemyControlled.controlCost) {
                            enemyControlled.ControlledOnOff(transform);
                            StartCoroutine("LightningEffectOn", mTarget.GetComponent<EnemyController>().switchTime);
                            StartCoroutine("TrailingEffectOn", mTarget.GetComponent<EnemyController>().switchTime);
                            isLocked = true;
                        }
                    }
                }
            }
            if (Input.GetButtonUp("Fire1")) {
                isLocked = false;
                mLineRenderer.enabled = true;
                StopCoroutine("LightningEffectOn");
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

    IEnumerator LightningEffectOn(float switchTime) {
        float startTime = Time.time;
        while ((Time.time - startTime) < switchTime) {
            soundManager.Gunshot((Time.time - startTime));
            lightning.Trigger();
            yield return null;
        }
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
