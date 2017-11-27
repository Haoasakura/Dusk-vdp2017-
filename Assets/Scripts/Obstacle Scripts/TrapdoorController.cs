using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorController : MonoBehaviour {

    private bool active = false;

    private bool isOpen;

    [Header("Transform for the center of rotation")]
    [SerializeField]
    private Transform rotationCenterTransform;

    // Use this for initialization
    void Start () {

        isOpen = false;

	}
	
	// Update is called once per frame
	void Update () {
        if (isOpen)
        {
            gameObject.GetComponentInChildren<Transform>().GetChild(0).gameObject.layer = 0;
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().GetChild(0).gameObject.layer = 8;
        }
        if (active) {
            Move();
        }
    }

    public void Move() {
        if (!isOpen)
        {
            gameObject.transform.RotateAround(rotationCenterTransform.position, new Vector3(0, 0, 1), -90);
            isOpen = true;
            active = false;
        }
        else
        {
            gameObject.transform.RotateAround(rotationCenterTransform.position, new Vector3(0, 0, 1), 90);
            isOpen = false;
            active = false;
        }
    }

    public void Activate() {
        active = true;
    }
}
