using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthLaserController : MonoBehaviour {

    private Player player;

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    private Vector3 nextPosition;

    [Header("Laser gate Speed")]
    [SerializeField]
    private float speed;

    [Header("Transform associated with child laser")]
    [SerializeField]
    private Transform childTransform;

    [Header("Transforms associated with extreme points")]
    [SerializeField]
    private Transform startPointTransform;
    [SerializeField]
    private Transform endPointTransform;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        startingPosition = startPointTransform.localPosition;
        targetPosition = endPointTransform.localPosition;
        nextPosition = startingPosition;
    }

    void Update()
    {
        Move();
        if (!player.isVisible)
        {
            GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(2).
                    GetComponent<BoxCollider2D>().enabled = false;
            GetComponentInChildren<Transform>().GetChild(0).GetComponent<LineRenderer>().enabled = false;


        }
        else
        {
            GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(2).
                    GetComponent<BoxCollider2D>().enabled = true;
            GetComponentInChildren<Transform>().GetChild(0).GetComponent<LineRenderer>().enabled = true;

        }
    }

    private void Move()
    {
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPosition, speed * Time.deltaTime);

        if (childTransform.localPosition == startingPosition || childTransform.localPosition == targetPosition)
        {
            ChangeDestination();
        }
    }

    public void ChangeDestination()
    {
        nextPosition = (nextPosition != startingPosition ? startingPosition : targetPosition);
    }
}
