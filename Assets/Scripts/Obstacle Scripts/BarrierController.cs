using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour {

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    private Vector3 nextPosition;

    [Header("Transform associated with child barrier")]
    [SerializeField]
    private Transform childTransform;

    [Header("Transform associated with target point")]
    [SerializeField]
    private Transform targetPointTransform;

    private float speed = 6;

    // Use this for initialization
    void Start () {

        startingPosition = childTransform.localPosition;
        targetPosition = targetPointTransform.localPosition;
        nextPosition = targetPosition;
        ChangeDestination();

    }
	
	// Update is called once per frame
	void Update () {
        Move();
	}

    private void Move()
    {
        
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPosition, speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Return) && (childTransform.localPosition == startingPosition || childTransform.localPosition == targetPosition))
        {
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
        if (System.Math.Abs(other.transform.position.x -
            gameObject.GetComponentInChildren<Transform>(false).GetChild(0).transform.position.x) > 1.25f) {
            other.transform.parent = null;
        }
        //else {
            
        //}
        //other.transform.position.y > gameObject.GetComponentInChildren<Transform>(false).GetChild(0).transform.position.y


    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.parent = null;
    }
}
