using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeWeapon : EnemyWeapon {

    public ContactFilter2D contactFilter;
    protected bool CR_running = false;

    protected override void Update() {

        RaycastHit2D hit = Physics2D.Linecast(barrel.position, laserDirection.position+new Vector3(-10,0,0), untraversableLayers);
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

            if (Input.GetButtonDown("Fire1") && !enemyController.gettingShoot) {
                isLocked = true;
                if (!CR_running)
                    StartCoroutine("MeleeAttack", enemy.transform);
            }
            if (Input.GetButtonUp("Fire1")) {
                isLocked = false;
            }
        }
    }

    IEnumerator MeleeAttack(Transform _target) {
        CR_running = true;
        float startTime = Time.time;
        while ((Time.time - startTime) < 0.3f) {
            foreach (Collider2D result in Physics2D.OverlapCircleAll(transform.position, 1f)) {
                if (result && _target && _target.CompareTag(Tags.player) && result.transform != null && result.transform.CompareTag(Tags.player)) {
                    EventManager.TriggerEvent("PlayerDied");
                    break;
                }
                else if (result && result.transform && result.transform.CompareTag(Tags.enemy) && result.transform != enemy.transform) {
                    result.GetComponent<EnemyController>().Kill();
                    StartCoroutine("AlertEnemies");
                    break;
                }
            }

            yield return null;
        }
        isLocked = false;
        CR_running = false;
    }
}
