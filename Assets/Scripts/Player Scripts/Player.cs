using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    public float moveSpeed = 3f;
    public float climbSpeed = 3f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    

    public bool canDoubleJump;
    private bool isDoubleJumping = false;
    private bool canClimb = false;
    private bool isClimbing = false;

    private float originalGravity;
    private float gravityOnFall;
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
        gravityOnFall = originalGravity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Ladder"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Ladder"))
        {
            canClimb = false;
            gravityOnFall = originalGravity;
            isClimbing = false;
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
            isDoubleJumping = false;
        }
        if (canDoubleJump && !controller.collisions.below && !isDoubleJumping && !wallSliding)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = true;
        }
        else if (isClimbing)
        {
            velocity.y = maxJumpVelocity;
            gravityOnFall = originalGravity;
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
            gravityOnFall = 0f;
            isClimbing = true;
        }
        else if ((Input.GetAxis("Vertical") != 0) && canClimb && isClimbing)
        {
            velocity.y = climbSpeed * Input.GetAxis("Vertical");
        }
        else if ((Input.GetAxis("Vertical") == 0) && canClimb && isClimbing)
        {
            velocity.y = velocity.y * 0.5f;
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));

        if (!isClimbing)
        {
            velocity.y += originalGravity * Time.deltaTime;
        }
    }
}
