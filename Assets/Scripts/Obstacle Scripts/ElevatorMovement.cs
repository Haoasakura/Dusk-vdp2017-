using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMovement : MonoBehaviour {

    public bool active = false;

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
        nextPosition = startingPosition;
	}
	
	void Update () {
        Move();
    }

    private void Move()
    {
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPosition, speed * Time.deltaTime);

        if (active && (childTransform.localPosition == startingPosition || childTransform.localPosition == targetPosition)) {
            active = false;
            ChangeDestination();
        }
    }

    public void ChangeDestination()
    {
        nextPosition = (nextPosition != startingPosition ? startingPosition : targetPosition);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        other.transform.parent = gameObject.GetComponentInChildren<Transform>(false).GetChild(0);
        /*if (other.transform.position.y -
            gameObject.GetComponentInChildren<Transform>(false).GetChild(0).transform.position.y < 0)
        {
            other.transform.parent = null;
        }*/

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.parent = null;
    }
}
