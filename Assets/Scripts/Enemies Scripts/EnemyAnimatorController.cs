using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{

    public Animator animator;
    public Animator shadowAnimator;

    private int currentState = 0;
    private int nextState = 0;
    private String animName;

    public void Animate(Vector3 velocity, bool isClimbing)
    {
        velocity.x = Mathf.Abs(velocity.x);
        velocity.y = Mathf.Abs(velocity.y);

        animName = "CharacterIdle";
        nextState = 0;

        if (!isClimbing && velocity.x > 0.2 && velocity.y < 0.2)
        {
            animName = "CharacterWalking";
            nextState = 1;
            if (velocity.x > 3)
            {
                animName = "CharacterRunning";

                nextState = 5;
            }

        }
        else if (!isClimbing && velocity.y != 0)
        {
            animName = "CharacterJumping";

            nextState = 2;
        }
        else if (isClimbing && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            animName = "CharacterClimbingIdle";

            nextState = 4;
        }
        else if (isClimbing && (velocity.x != 0 || velocity.y != 0))
        {
            animName = "CharacterClimbing";

            nextState = 3;
        }

        if (currentState != nextState)
        {
            currentState = nextState;
            animator.Play(animName);
            shadowAnimator.Play(animName);
            animator.SetInteger("State", nextState);
            shadowAnimator.SetInteger("State", nextState);
        }

    }

}

