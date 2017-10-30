using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //Settaggi per il personaggio (Utilizza Inspector per cambiarli
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    public float moveMinSpeed = 3f;
    public float moveMaxSpeed = 7f;
    public float climbSpeed = 3f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    //Variabili per l'arrampicata e il doppio salto (non utilizzato)
    private bool canClimb = false;
    private bool isClimbing = false;
    private bool lastClimb = true;

    private float originalGravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        originalGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(originalGravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(originalGravity) * minJumpHeight);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("BaseLadder") && isClimbing)
        {
            isClimbing = false;
        }
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            if (isClimbing)
            {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
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
        }
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            if (isClimbing)
            {
                collision.gameObject.layer = 9;
                collision.gameObject.tag = "Ladder";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag.Equals("Ladder"))
        {
            canClimb = false;
        }
        if (collision.gameObject.name.Equals("TopLadder"))
        {
            collision.gameObject.layer = 8;
            collision.gameObject.tag = "Through";
        }

    }

    private void Update()
    {
        CalculateVelocity();
        ClimbControl();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;
        }
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

        }
        else if (isClimbing)
        {
            velocity.y = maxJumpVelocity;
            isClimbing = false;
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
        if ((Input.GetAxis("Vertical") > 0) && canClimb && !isClimbing)
        {
            isClimbing = true;
        }
        else if ((Input.GetAxis("Vertical") != 0) && canClimb && isClimbing)
        {
            velocity.y = climbSpeed * Input.GetAxis("Vertical");
        }
        else if ((Input.GetAxis("Vertical") == 0) && canClimb && isClimbing)
        {
            velocity.y = velocity.y * 0.5f;
        } else
        {
            isClimbing = false;
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = 0f;

        if (Input.GetKey(KeyCode.LeftShift))
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
