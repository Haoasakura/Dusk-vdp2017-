using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeWeapon : EnemyWeapon {

    protected override void Update() {

        RaycastHit2D hit = Physics2D.Linecast(barrel.position, laserDirection.position, untraversableLayers);
        if (hit.collider != null && !hit.collider.gameObject.layer.Equals(9)) {
            mTarget = hit.transform;
        }
        else {
            mTarget = null;
        }

        if (enemyController.controlled && !enemy.controlling) {

            float a = Input.GetAxis("HorizontalGun");
            float b = Input.GetAxis("VerticalGun");
            float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
            //rotazione della pistola
            if (Mathf.Abs(c) > 0.9 && !isLocked) {
                float angleRot = -Mathf.Sign(b) * Mathf.Rad2Deg * Mathf.Acos(a / c);

                if (GetComponentInParent<Enemy>().transform.localScale.x > 0)
                    transform.parent.rotation = Quaternion.Euler(0f, 0f, 73 + angleRot);
                else
                    transform.parent.rotation = Quaternion.Euler(0f, 0f, 93 + angleRot);

                //mantiene la sprite dell'arma nel verso giusto
                if (mTransform.rotation.eulerAngles.z % 270 < 90 && mTransform.rotation.eulerAngles.z % 270 > 0) {
                    GetComponent<SpriteRenderer>().flipY = false;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    GetComponent<SpriteRenderer>().flipY = true;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = true;
                }
            }

            if (Input.GetButtonDown("Fire1")) {
                StartCoroutine("MeleeAttack", 1f);
                if (mTarget != null) {
                    if (mTarget.CompareTag(Tags.enemy)) {
                        enemyControlled = mTarget.GetComponent<EnemyController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !enemyControlled.gettingShoot) {
                            StartCoroutine("AlertEnemies");
                            isLocked = true;
                        }
                    }
                }
            }
            if (Input.GetButtonUp("Fire1")) {
                isLocked = false;
                StopCoroutine("MeleeAttack");
            }
        }
    }

    IEnumerator MeleeAttack(Transform _target) {
        Debug.Log("Melee Attack");
        float armRotation = 159.1f;
        Collider2D weaponCollider = GetComponent<Collider2D>();
        armTransform.rotation = Quaternion.Euler(0.0f, 0.0f, enemy.transform.localScale.x * armRotation);
        float startTime = Time.time;
        while ((Time.time - startTime) < 0.3f) {
            float distCovered = (Time.time - startTime);
            float fracJourney = distCovered / 0.033f;
            armRotation -= fracJourney;
            armTransform.rotation = Quaternion.Euler(0.0f, 0.0f, enemy.transform.localScale.x * armRotation);
            if(_target)
                if (weaponCollider.IsTouching(_target.GetComponent<Collider2D>())) {
                    if (_target.CompareTag("Player"))
                        EventManager.TriggerEvent("PlayerDied");
                    else
                        Destroy(_target.parent.gameObject);
                }

            yield return null;
        }
        isLocked = false;
    }

}
