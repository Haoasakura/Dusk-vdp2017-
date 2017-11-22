using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour {

    [Header("GameObject associated with linked mechanism")]
    [SerializeField]
    private GameObject mechanism;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetButtonDown("Fire2") && other.gameObject.tag.Equals("Player"))
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = gameObject.GetComponent<SpriteRenderer>().flipX == true ? false : true;
            Activate();
        }
    }

    private void Activate()
    {
        if (mechanism.GetComponent<ElevatorMovement>() != null) {
            mechanism.GetComponent<ElevatorMovement>().ChangeDestination();
        }
        else if (mechanism.GetComponent<BarrierController>() != null) {
            mechanism.GetComponent<BarrierController>().ChangeDestination();
        }
        else if (mechanism.GetComponent<TrapdoorController>() != null) {
            mechanism.GetComponent<TrapdoorController>().Activate();
        }
        else if (mechanism.GetComponent<DoorController>() != null) {
            mechanism.GetComponent<DoorController>().Activate();
        }
    }
}
