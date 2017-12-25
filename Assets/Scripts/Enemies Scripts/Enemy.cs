using System;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
    //Settaggi per il personaggio (Utilizza Inspector per cambiarli)
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    public float moveMinSpeed = 3f;
    public float moveMaxSpeed = 7f;
    public float minClimbAngle = 0.6f;
    public float climbSpeed = 3f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public bool controlling = false;

    //Variabili per l'arrampicata e il doppio salto (non utilizzato)
    private bool canClimb = false;
    public bool isClimbing = false;
    private bool lastClimb = true;

    //Variabili di Contatto
    private ContactFilter2D contactFilter;
    public LayerMask contactMask;
    public GameObject pivotArm;
    public GameObject weapon;

    //Variabili di stato per la gravità
    private float originalGravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float velocityXSmoothing;
    private Vector3 velocity;

    //Link allo script Controller2D
    public EnemyController2D controller;
    private EnemyAnimatorController enemyAnimationController;
    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;

    private void Start() {
        //Setta le regole di gravità e trova il Controller2D
        controller = GetComponent<EnemyController2D>();
        enemyAnimationController = GetComponent<EnemyAnimatorController>();
        originalGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(originalGravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(originalGravity) * minJumpHeight);
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = contactMask;
        contactFilter.useTriggers = true;
    }

    private void Update() {
        CalculateVelocity();
        ClimbControl();
        controller.Move(velocity * Time.deltaTime, directionalInput);
        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0f;
        enemyAnimationController.Animate(velocity, isClimbing);
    }

    public void SetDirectionalInput(Vector2 input) {
            directionalInput = input;
    }

    public void OnJumpInputDown() {
        if (controller.collisions.below) {
            velocity.y = maxJumpVelocity;
            isClimbing = false;
            pivotArm.SetActive(true);

        }
        else if (isClimbing) {
            velocity.y = maxJumpVelocity;
            isClimbing = false;
            pivotArm.SetActive(true);
            foreach (SpriteRenderer sr in pivotArm.GetComponentsInChildren<SpriteRenderer>()) {
                sr.enabled = true;
            }
            weapon.GetComponent<EnemyWeapon>().aimsight.GetComponent<LineRenderer>().enabled = true;
        }
    }

    public void OnJumpInputUp() {
        if (velocity.y > minJumpVelocity) {
            velocity.y = minJumpVelocity;
        }
    }

    private void CalculateVelocity() {
        float targetVelocityX = 0f;

        if (Input.GetButton("Fire3") && (transform.GetComponent<EnemyController>() == null || transform.GetComponent<EnemyController>().controlled)) {
            targetVelocityX = directionalInput.x * moveMaxSpeed;
        }
        else {
            targetVelocityX = directionalInput.x * moveMinSpeed;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));

        if (!isClimbing) {
            velocity.y += originalGravity * Time.deltaTime;
        }
        else {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeGrounded);
        }
    }

    private void ClimbControl() {
        if (!canClimb && isClimbing) {
            isClimbing = false;
            foreach (SpriteRenderer sr in pivotArm.GetComponentsInChildren<SpriteRenderer>()) {
                sr.enabled = true;
            }
            weapon.GetComponent<EnemyWeapon>().aimsight.GetComponent<LineRenderer>().enabled = true;
            Collider2D[] results = new Collider2D[10];
            int i = GetComponent<Collider2D>().OverlapCollider(contactFilter, results);
            if (i > 0) {
                isClimbing = true;
            }

        }
        else if ((directionalInput.y > minClimbAngle) && canClimb && !isClimbing) {
            isClimbing = true;
            foreach (SpriteRenderer sr in pivotArm.GetComponentsInChildren<SpriteRenderer>()) {
                sr.enabled = false;
            }
            weapon.GetComponent<EnemyWeapon>().aimsight.GetComponent<LineRenderer>().enabled = false;
        }
        else if (directionalInput.y != 0 && (Mathf.Abs(directionalInput.y) > minClimbAngle) && canClimb && isClimbing) {
            velocity.y = climbSpeed * directionalInput.y;
        }
        else if ((Mathf.Abs(directionalInput.y) < minClimbAngle) && canClimb && isClimbing) {
            velocity.y = velocity.y * 0.5f;
        }
        else {
            isClimbing = false;
            foreach (SpriteRenderer sr in pivotArm.GetComponentsInChildren<SpriteRenderer>()) {
                sr.enabled = true;
            }
            weapon.GetComponent<EnemyWeapon>().aimsight.GetComponent<LineRenderer>().enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.name.Equals("TopLadder")) {
            if (isClimbing) {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
            }
            else {
                collision.gameObject.layer = 11;
                collision.gameObject.tag = "Through";
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Ladder")) {
            canClimb = true;
            if (isClimbing)
            {
                foreach(SpriteRenderer sr in pivotArm.GetComponentsInChildren<SpriteRenderer>()) {
                    sr.enabled = false;
                }
                weapon.GetComponent<EnemyWeapon>().aimsight.GetComponent<LineRenderer>().enabled = false;
            }
        }

        if (collision.gameObject.tag.Equals("BaseLadder") && isClimbing && directionalInput.y < 0) {
            isClimbing = false;
            foreach (SpriteRenderer sr in pivotArm.GetComponentsInChildren<SpriteRenderer>()) {
                sr.enabled = true;
            }
            weapon.GetComponent<EnemyWeapon>().aimsight.GetComponent<LineRenderer>().enabled = true;
        }
        
        if (collision.gameObject.name.Equals("TopLadder")) {
            if (isClimbing) {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
                foreach (SpriteRenderer sr in pivotArm.GetComponentsInChildren<SpriteRenderer>()) {
                    sr.enabled = false;
                }
                weapon.GetComponent<EnemyWeapon>().aimsight.GetComponent<LineRenderer>().enabled = false;
            }
            else
            {
                collision.gameObject.layer = 11;
                collision.gameObject.tag = "Through";
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.gameObject.tag.Equals("Ladder")) {
            canClimb = false;
        }
        
        if (collision.gameObject.name.Equals("TopLadder")) {
            collision.gameObject.layer = 11;
            collision.gameObject.tag = "Through";
        }
    }
}
