using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorController : MonoBehaviour {

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
		
        if(Input.GetKeyDown(KeyCode.G) && !isOpen)
        {
            gameObject.transform.RotateAround(rotationCenterTransform.position, new Vector3(0,0,1), -90);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.G) && isOpen)
        {
            gameObject.transform.RotateAround(rotationCenterTransform.position, new Vector3(0, 0, 1), 90);
            isOpen = false;
        }

    }
}
