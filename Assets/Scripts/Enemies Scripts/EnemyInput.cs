using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyInput : MonoBehaviour
{
    private Enemy enemy;
    private EnemyWeapon gunController2D;

    private void Start() {
        enemy = GetComponent<Enemy>();
        gunController2D = transform.GetComponentInChildren<EnemyWeapon>();
    }

    private void Update() {
        if (!enemy.controlling) {
            Vector2 directionalInput;
            if (gunController2D.isLocked) {
                directionalInput = new Vector2(0f, 0f);
                enemy.SetDirectionalInput(directionalInput);
            }
            else {
                directionalInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                enemy.SetDirectionalInput(directionalInput);
            }

        }
        //for playtest purpose
        if (transform.GetComponent<EnemyController>().autodestruct && (Input.GetKey(KeyCode.F) || Input.GetButton("Fire4"))) {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
