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

    private bool isClimbing;
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
        Debug.Log("Culochilegge");
        Debug.Log(collision.gameObject.layer);

        if (collision.gameObject.layer.Equals("9"))
        {
            Debug.Log(collision.gameObject.layer);
        }
    
    }


    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
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