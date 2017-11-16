using UnityEngine;

//[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
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
    private bool isClimbing = false;
    private bool lastClimb = true;

    //Variabili di Contatto
    private ContactFilter2D contactFilter;
    public LayerMask contactMask;

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

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;
    private GameObject gun;

    private void Start()
    {
        //Setta le regole di gravità e trova il Controller2D
        controller = GetComponent<Controller2D>();
        gun = transform.GetChild(1).gameObject;
        originalGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(originalGravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(originalGravity) * minJumpHeight);
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = contactMask;
        contactFilter.useTriggers = true;
        Debug.Log(contactFilter.isFiltering);
    }

    private void Update()
    {
        CalculateVelocity();
        ClimbControl();
        controller.Move(velocity * Time.deltaTime, directionalInput);
        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0f;
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
            gun.SetActive(true);

        }
        else if (isClimbing)
        {
            velocity.y = maxJumpVelocity;
            isClimbing = false;
            gun.SetActive(true);

        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void ClimbControl()
    {
        if ((directionalInput.y > minClimbAngle) && canClimb && !isClimbing)
        {
            gun.SetActive(false);
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
            gun.SetActive(true);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("BaseLadder") && isClimbing)
        {
            isClimbing = false;
            gun.SetActive(true);

        }

        //TODO: Estrarre questo codice nell'oggetto TopLadder
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            if (isClimbing)
            {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
                gun.SetActive(false);

            }
            else
            {
                collision.gameObject.layer = 8;
                collision.gameObject.tag = "Through";
            }
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Ladder"))
        {
            canClimb = true;
            if (isClimbing)
            {
                gun.SetActive(false);
            }
        }

        //TODO: Estrarre questo codice nell'oggetto TopLadder
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            if (isClimbing)
            {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
                gun.SetActive(false);
            }
        }
        if (collision.gameObject.name.Equals("LightCollider"))
        {
            isVisible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag.Equals("Ladder"))
        {
            canClimb = false;
            gun.SetActive(true);
            Collider2D[] overlappingColliders = new Collider2D[] { };
            GetComponent<Collider2D>().OverlapCollider(contactFilter, overlappingColliders);
            Debug.Log(overlappingColliders.Length);
            foreach (Collider2D c in overlappingColliders)
            {
                Debug.Log("Here");
                Debug.Log(c.gameObject.name);
                if (c.gameObject.layer.Equals(9))
                {
                    Debug.Log("Here1");
                    canClimb = true;
                    isClimbing = true;
                    gun.SetActive(false);
                }
            }
        }

        //TODO: Estrarre questo codice nell'oggetto TopLadder
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            collision.gameObject.layer = 8;
            collision.gameObject.tag = "Through";
        }

        if (collision.gameObject.name.Equals("LightCollider"))
        {
            isVisible = false;
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = 0f;

        if (Input.GetButton("Fire3"))
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
}
