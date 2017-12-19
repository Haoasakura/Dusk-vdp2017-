using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorController : MonoBehaviour {

    public bool active = false;
    private ObjectSoundManager osm;
    private bool isOpen = false;

    [Header("Transform for the center of rotation")]
    [SerializeField]
    private Transform rotationCenterTransform;

    // Use this for initialization
    void Start () {
        osm = GetComponent<ObjectSoundManager>();
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
            osm.PlaySound(0.95f);
            gameObject.transform.RotateAround(rotationCenterTransform.position, new Vector3(0, 0, 1), -90 * Mathf.Sign(transform.localScale.x));
            isOpen = true;
            active = false;
        }
        else
        {
            osm.PlaySound(0.95f);
            gameObject.transform.RotateAround(rotationCenterTransform.position, new Vector3(0, 0, 1), 90 * Mathf.Sign(transform.localScale.x));
            isOpen = false;
            active = false;
        }
    }

    public void Activate() {
        active = true;
    }
}
