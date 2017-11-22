using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

    public Animator animator;

    public float runVelocity;
    public float walkVelocity;

    private int currentState = 0;
    private int nextState = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Animate (Vector3 velocity, bool isClimbing)
    {
        velocity.x = Mathf.Abs(velocity.x);
        velocity.y = Mathf.Abs(velocity.y);

        nextState = 0;

        if (!isClimbing && velocity.x > 0.2 && velocity.y < 0.2)
        {
            nextState = 1;
            if (Input.GetButton("Fire3"))
            {
                nextState = 5;
            }

        }
        else if (!isClimbing && velocity.y != 0)
        {
            nextState = 2;
        }
        else if (isClimbing && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            nextState = 4;
        }
        else if (isClimbing && (velocity.x != 0 || velocity.y != 0))
        {
            nextState = 3;
        }


        if (currentState != nextState)
        {
            currentState = nextState;
            animator.SetInteger("State", nextState);
        }

    }

}
