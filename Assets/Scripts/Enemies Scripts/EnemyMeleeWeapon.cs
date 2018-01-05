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

            if (Input.GetButtonDown("Fire1")) {
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
        float armRotation = 159.1f;
        Collider2D weaponCollider = GetComponent<Collider2D>();
        armTransform.rotation = Quaternion.Euler(0.0f, 0.0f, enemy.transform.localScale.x * armRotation);
        float startTime = Time.time;
        while ((Time.time - startTime) < 0.3f) {
            float distCovered = (Time.time - startTime);
            float fracJourney = distCovered / 0.03f;
            armRotation -= fracJourney;
            armTransform.rotation = Quaternion.Euler(0.0f, 0.0f, enemy.transform.localScale.x * armRotation);
            int n = GameObject.FindGameObjectsWithTag(Tags.enemy).Length;
            Collider2D[] results = new Collider2D[n+1];
            weaponCollider.OverlapCollider(contactFilter, results);
            foreach (Collider2D result in results) {
                if (result && _target && _target.CompareTag(Tags.player) && result.transform != null && result.transform.CompareTag(Tags.player))
                        EventManager.TriggerEvent("PlayerDied");
                else if (result && result.transform && result.transform.CompareTag(Tags.enemy) && result.transform!=enemy.transform) {
                    result.GetComponent<EnemyController>().Kill();
                    StartCoroutine("AlertEnemies");
                }
                
            }

            yield return null;
        }
        transform.parent.rotation = Quaternion.Euler(0f, 0f, enemyController.transform.localScale.x > 0 ? 73.4f : -73.4f);
        isLocked = false;
        CR_running = false;
    }

}
