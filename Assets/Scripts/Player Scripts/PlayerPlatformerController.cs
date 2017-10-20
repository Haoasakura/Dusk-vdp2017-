using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{

    [Header("Player Min Speed")]
    public float minSpeed = 3;

    [Header("Player Max Speed")]
    public float maxSpeed = 7;

    [Header("Max Jump")]
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    //private Animator animator;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator> ();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(9))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(9))
        {
            canClimb = false;
            gravityOnFall = originalGravity;
            isClimbing = false;
        }
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
            gravityOnFall = originalGravity;
        }
        else if (Input.GetButtonUp("Jump") && !isClimbing)
        {
            gravityOnFall = originalGravity;
            isClimbing = false;
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }
        else if (Input.GetButtonDown("Jump") && isClimbing)
        {
            velocity.y = jumpTakeOffSpeed;
            gravityOnFall = originalGravity;
            isClimbing = false;
        }

        if ((Input.GetAxis("Vertical") > 0) && canClimb && !isClimbing)
        {
            gravityOnFall = 0f;
            isClimbing = true;
        }
        else if ((Input.GetAxis("Vertical") > 0) && canClimb && isClimbing)
        {
            move.y = Input.GetAxis("Vertical");
        }

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        //animator.SetBool ("grounded", grounded);
        //animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetVelocity = move * maxSpeed;
        }
        else
        {
            targetVelocity = move * minSpeed;
        }
    }
}