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

        if (Input.GetKeyDown(KeyCode.H) && (childTransform.localPosition == startingPosition || childTransform.localPosition == targetPosition)) {
            ChangeDestination();
        }
    }

    private void ChangeDestination()
    {
        nextPosition = (nextPosition != startingPosition ? startingPosition : targetPosition);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        other.transform.parent = gameObject.GetComponentInChildren<Transform>(false).GetChild(0);
        /*if (System.Math.Abs(other.transform.position.x -
            gameObject.GetComponentInChildren<Transform>(false).GetChild(0).transform.position.x) > 1.25f)
        {
            other.transform.parent = null;
        }*/

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.parent = null;
    }
}
