using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {

    public bool active = false;

    [Header("Frequency of activation of laser when active")]
    [Range(20f,120f)]
    public int frequency = 60;

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

        startingPosition = startPointTransform.localPosition;
        targetPosition = endPointTransform.localPosition;
        nextPosition = startingPosition;
    }

    void Update()
    {
        Move();
        IntermittentActivation();
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

    public void IntermittentActivation()
    {
        if (active)
        {
            if(Time.renderedFrameCount % frequency == 0 && GetComponentInChildren<Transform>().GetChild(0).GetComponent<LineRenderer>().enabled)
            {
                GetComponentInChildren<Transform>().GetChild(0).GetComponent<LineRenderer>().enabled = false;
                GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(2).
                    GetComponent<BoxCollider2D>().enabled = false;
            }
            else if (Time.renderedFrameCount % frequency == 0 && !GetComponentInChildren<Transform>().GetChild(0).GetComponent<LineRenderer>().enabled)
            {
                GetComponentInChildren<Transform>().GetChild(0).GetComponent<LineRenderer>().enabled = true;
                GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(2).
                    GetComponent<BoxCollider2D>().enabled = true;
            }
        }
        else
        {
            GetComponentInChildren<Transform>().GetChild(0).GetComponent<LineRenderer>().enabled = false;
            GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(2).
                GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void Activate()
    {
        active = (active ? false : true);
    }
}
