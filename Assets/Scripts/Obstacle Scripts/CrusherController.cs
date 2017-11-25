using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherController : MonoBehaviour {

    public bool active = false;

    private Vector3 restPosition;
    private Vector3 originalRestPosition;
    private Vector3 smashPosition;

    private Vector3 nextPosition;

    [Header("Transform associated with child crusher")]
    [SerializeField]
    private Transform childTransform;

    [Header("Transform associated with smash point")]
    [SerializeField]
    private Transform targetPointTransform;

    private float returnSpeed = 3.5f;
    private float smashSpeed = 8;

    // Use this for initialization
    void Start()
    {

        restPosition = childTransform.localPosition;
        originalRestPosition = restPosition;
        smashPosition = targetPointTransform.localPosition;
        nextPosition = restPosition;

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (active)
        {
            childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPosition,
            (nextPosition == smashPosition ? smashSpeed * Time.deltaTime : returnSpeed * Time.deltaTime));
        }

        if (active && (childTransform.localPosition == restPosition || childTransform.localPosition == smashPosition))
        {
            ChangeDestination();
        }

        if (!active)
        {
            childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition,
                originalRestPosition, returnSpeed * Time.deltaTime);
        }
    }

    public void ChangeDestination()
    {
        nextPosition = (nextPosition != restPosition ? restPosition : smashPosition);
    }

    public void Activate()
    {
        active = (active ? false : true);
    }
}
