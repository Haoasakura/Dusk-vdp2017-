using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMovement : MonoBehaviour {

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    private Vector3 nextPosition;

    [Header("Elevator Speed")]
    [SerializeField]
    private float speed;

    [Header("Transform associated with child platform")]
    [SerializeField]
    private Transform childTransform;

    [Header("Transform associated with target point")]
    [SerializeField]
    private Transform targetPointTransform;

	void Start () {

        startingPosition = childTransform.localPosition;
        targetPosition = targetPointTransform.localPosition;
        nextPosition = targetPosition;
	}
	
	void Update () {
        Move();
    }

    private void Move()
    {
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPosition, speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Return) && (childTransform.localPosition == startingPosition || childTransform.localPosition == targetPosition)) {
            ChangeDestination();
        }
    }

    private void ChangeDestination()
    {
        nextPosition = (nextPosition != startingPosition ? startingPosition : targetPosition);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player")) {
            other.gameObject.transform.position = gameObject.transform.position;
        }
    }
}
