﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour {

    public bool active = false;


    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private ObjectSoundManager osm;
    private Vector3 nextPosition;

    [Header("Transform associated with child barrier")]
    [SerializeField]
    private Transform childTransform;

    [Header("Transform associated with target point")]
    [SerializeField]
    private Transform targetPointTransform;

    private float speed = 20;

    // Use this for initialization
    void Start () {

        osm = GetComponent<ObjectSoundManager>();
        startingPosition = childTransform.localPosition;
        targetPosition = targetPointTransform.localPosition;
        nextPosition = startingPosition;

    }
	
	// Update is called once per frame
	void Update () {
        Move();
	}

    private void Move()
    {
        
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPosition, speed * Time.deltaTime);

        if (active && (childTransform.localPosition == startingPosition || childTransform.localPosition == targetPosition))
        {
            active = false;
            ChangeDestination();
        }
    }

    public void ChangeDestination()
    {
        nextPosition = (nextPosition != startingPosition ? startingPosition : targetPosition);
        osm.PlaySound();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Gun") && !other.gameObject.CompareTag(Tags.enemy))
        {
            other.transform.parent = gameObject.GetComponentInChildren<Transform>(false).GetChild(0);
        }
        else if(other.gameObject.CompareTag(Tags.enemy)) {
            other.transform.parent.parent= gameObject.GetComponentInChildren<Transform>(false).GetChild(0);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Gun") && !other.gameObject.CompareTag(Tags.enemy))
        {
            other.transform.parent = null;
        }
        else if (other.gameObject.CompareTag(Tags.enemy)) {
            other.transform.parent.parent = null;
        }
    }
}
