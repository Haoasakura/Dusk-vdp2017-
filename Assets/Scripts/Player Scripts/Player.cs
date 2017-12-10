using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //Settaggi per il personaggio (Utilizza Inspector per cambiarli)
    public GameObject explosionDeath;
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    public float moveMinSpeed = 3f;
    public float moveMaxSpeed = 7f;
    public float minClimbAngle = 0.7f;
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
    public GameObject gun;

    //Variabili per la visibilità
    public bool isVisible = false;

    //Variabili di stato per la gravità
    private float originalGravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float velocityXSmoothing;
    private Vector3 velocity;
     
    //Link allo script Controller2D
    public Controller2D controller;
    public PlayerAnimationController playerAnimationController;

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;
    private bool isLighted = false;

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDied", DeathProcess);
        EventManager.StartListening("PlayerControlled", DelayedDeath);
    }

    private void Start()
    {
        //Setta le regole di gravità e trova il Controller2D
        controller = GetComponent<Controller2D>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        originalGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(originalGravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(originalGravity) * minJumpHeight);
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = contactMask;
        contactFilter.useTriggers = true;
    }

    private void Update()
    {
        CalculateVelocity();
        ClimbControl();
        VisibilityControl();
        controller.Move(velocity * Time.deltaTime, directionalInput);
        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0f;
        if (controller.collisions.below)
        {
            gun.GetComponent<GunController>().canFire = true;
        }
        playerAnimationController.Animate(velocity, isClimbing, controller);
    }

    private void VisibilityControl()
    {
        //isVisible = false;
        bool seenByEnemies = false;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy)) {
            if (enemy.GetComponent<EnemyController>().playerInSight && Vector2.Distance(transform.position,enemy.transform.position)<enemy.GetComponent<EnemyController>().sightRange) {
                seenByEnemies = true;
            }
        }

        if (velocity.magnitude > 1f || isLighted || seenByEnemies) {
            isVisible = true;
        }
        else if (Input.GetButton("Fire1")) {
            isVisible = true;
        }
        else if (controlling) {
            isVisible = true;
        }
        else
            isVisible = false;
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
            isClimbing = false;
            pivotArm.SetActive(true);
            SoundManager.Instance.Jump();
            gun.GetComponent<GunController>().canFire = false;

        }
        else if (isClimbing)
        {
            velocity.y = maxJumpVelocity;
            isClimbing = false;
            pivotArm.SetActive(true);
            SoundManager.Instance.Jump();
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = 0f;

        if (Input.GetButton("Fire3") && (transform.GetComponent<EnemyController>() == null || transform.GetComponent<EnemyController>().controlled))
        {
            targetVelocityX = directionalInput.x * moveMaxSpeed;
        }
        else
        {
            targetVelocityX = directionalInput.x * moveMinSpeed;
        }

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));

        if (!isClimbing)
        {
            velocity.y += originalGravity * Time.deltaTime;
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeGrounded);
        }
    }

    private void ClimbControl()
    {
        if (!canClimb && isClimbing)
        {
            pivotArm.SetActive(true);
            isClimbing = false;
            Collider2D[] results =  new Collider2D [10];
            int i = GetComponent<Collider2D>().OverlapCollider(contactFilter, results);
            if (i > 0)
            {
                pivotArm.SetActive(false);
                isClimbing = true;
            }
            
        }
        else if ((directionalInput.y > minClimbAngle) && canClimb && !isClimbing)
        {
            pivotArm.SetActive(false);
            isClimbing = true;
        }
        else if (directionalInput.y != 0 && (Mathf.Abs(directionalInput.y) > minClimbAngle) && canClimb && isClimbing)
        {
            velocity.y = climbSpeed * directionalInput.y;
        }
        else if ((Mathf.Abs(directionalInput.y) < minClimbAngle) && canClimb && isClimbing)
        {
            velocity.y = velocity.y * 0.5f;
        } else
        {
            isClimbing = false;
            pivotArm.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: Estrarre questo codice nell'oggetto TopLadder
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            if (isClimbing)
            {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
                pivotArm.SetActive(false);
            }
            else
            {
                collision.gameObject.layer = 11;
                collision.gameObject.tag = "Through";
            }
        }

        if (collision.gameObject.tag.Equals("ResetGun"))
        {
            if (gun.GetComponent<GunController>().currentCharge > 0)
            {
                collision.GetComponent<AudioSource>().Play();
            }
            gun.GetComponent<GunController>().currentCharge = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Ladder"))
        {
            canClimb = true;
            if (isClimbing)
            {
                pivotArm.SetActive(false);
            }
        }

        if (collision.gameObject.tag.Equals("BaseLadder") && isClimbing && directionalInput.y < 0)
        {
            isClimbing = false;
            pivotArm.SetActive(true);
        }

        //TODO: Estrarre questo codice nell'oggetto TopLadder
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            if (isClimbing)
            {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
                pivotArm.SetActive(false);
            }
        }
        if (collision.gameObject.tag.Equals("LightCollider"))
        {
            isLighted = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag.Equals("Ladder"))
        {
            canClimb = false;
        }

        //TODO: Estrarre questo codice nell'oggetto TopLadder
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            collision.gameObject.layer = 11;
            collision.gameObject.tag = "Through";
        }

        if (collision.gameObject.tag.Equals("LightCollider"))
        {
            isLighted = false;
        }
    }

    private void DelayedDeath()
    {
        StartCoroutine("PuppetAnimation");
    }

    IEnumerator PuppetAnimation()
    {
        yield return null;//new WaitForSeconds(1);
        EventManager.TriggerEvent("PlayerDied");
    }

    private void DeathProcess()
    {
        Instantiate(explosionDeath, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventManager.TriggerEvent("ReloadScene");
    }

}
