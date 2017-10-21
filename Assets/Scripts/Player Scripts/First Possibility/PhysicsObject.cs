﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    public float minGroundNormalY = .65f;

    [Header("Gravity modifier")]
    public float gravityModifier = 1f;

    [Header("Gravity on fall")]
    public float gravityOnFall = 1f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected bool canClimb;
    protected bool isClimbing;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D> (16);
    protected float originalGravity;

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D> ();
    }

    void Start () 
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer));
        contactFilter.useLayerMask = true;
        originalGravity = gravityOnFall;
        canClimb = false;
        isClimbing = false;
    }
    
    void Update () 
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity (); 
    }

    protected virtual void ComputeVelocity()
    {
    
    }

    void FixedUpdate()
    {
        if (!isClimbing)
        {
            velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        }
        else
        {
            velocity += 0f * Vector2.up;
        }

        if (grounded || (!grounded && velocity.x > 0 && targetVelocity.x > 0) 
            || (!grounded && velocity.x > 0 && targetVelocity.x > 0) || isClimbing)
        {
            velocity.x = targetVelocity.x;
        }

        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2 (groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement (move, false);

        if (deltaPosition.y >= 0)
        {
            move = Vector2.up * deltaPosition.y;
        }
        else if (deltaPosition.y < 0 && !isClimbing)
        {
            move = Vector2.up * gravityOnFall * deltaPosition.y;
        }
        else if (deltaPosition.y < 0 && isClimbing)
        {
            move = Vector2.up * deltaPosition.y;
        }

        Movement (move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        //Calcolo della lunghezza del vettore movimento
        float distance = move.magnitude;

        //Se la distanza è maggiore della distanza di movimento minima allora mi muovo
        if (distance > minMoveDistance) 
        {
            int count = rb2d.Cast (move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear ();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add (hitBuffer [i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++) 
            {
                Vector2 currentNormal = hitBufferList [i].normal;
                if (currentNormal.y > minGroundNormalY) 
                {
                    grounded = true;
                    if (yMovement) 
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot (velocity, currentNormal);
                if (projection < 0) 
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList [i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }

}