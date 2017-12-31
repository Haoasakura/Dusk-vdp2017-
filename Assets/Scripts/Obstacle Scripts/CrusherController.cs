using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherController : MonoBehaviour {

    public bool active = false;

    private Vector3 restPosition;
    private Vector3 originalRestPosition;
    private Vector3 smashPosition;
    private bool isWaiting = false;
    private Vector3 nextPosition;

    [Header("Transform associated with child crusher")]
    [SerializeField]
    private Transform childTransform;

    [Header("Transform associated with smash point")]
    [SerializeField]
    private Transform targetPointTransform;

    [Header("Crusher speed up and down")]
    public float returnSpeed = 3.5f;
    public float smashSpeed = 8;
    public float waitTime = 1;

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
        if (!active)
        {
            gameObject.GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void Move()
    {
        if (active && !isWaiting)
        {
            childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPosition,
            (nextPosition == smashPosition ? smashSpeed * Time.deltaTime : returnSpeed * Time.deltaTime));
        }

        if (active && (childTransform.localPosition == restPosition || childTransform.localPosition == smashPosition) && !isWaiting)
        {
            isWaiting = true;
            StartCoroutine(ChangeDestination());
        }

        if (active && (nextPosition == restPosition))
        {
            gameObject.GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().GetChild(0).GetComponentInChildren<Transform>().GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
        }

        if (!active)
        {
            childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition,
                originalRestPosition, returnSpeed * Time.deltaTime);
        }
    }

    IEnumerator ChangeDestination()
    {
        if (nextPosition == restPosition)
        {
            yield return new WaitForSeconds(waitTime);
            nextPosition = (nextPosition != restPosition ? restPosition : smashPosition);
            GetComponentInChildren<AudioSource>().Play();
        }
        else
        {
            nextPosition = (nextPosition != restPosition ? restPosition : smashPosition);
        }
        isWaiting = false;
    }

    public void Activate()
    {
        active = (active ? false : true);
    }
}
