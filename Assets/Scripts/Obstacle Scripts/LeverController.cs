using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour {

    public bool isOneTime = false;

    [Header("GameObject associated with linked mechanism")]
    [SerializeField]
    private GameObject[] mechanisms;

    private bool isUsed = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isOneTime)
        {
            if (Input.GetButtonDown("Fire2") && other.gameObject.tag.Equals("Player") &&
                !Input.GetButton("Fire1"))
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = gameObject.GetComponent<SpriteRenderer>().flipX == true ? false : true;
                Activate();
                SoundManager.Instance.Lever();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire2") && other.gameObject.tag.Equals("Player") &&
                !Input.GetButton("Fire1") && !isUsed)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = gameObject.GetComponent<SpriteRenderer>().flipX == true ? false : true;
                Activate();
                SoundManager.Instance.Lever();
                isUsed = true;
            }
        }
    }

    private void Activate()
    {
        foreach (GameObject mechanism in mechanisms)
        {
            if (mechanism.GetComponent<ElevatorMovement>() != null)
            {
                mechanism.GetComponent<ElevatorMovement>().ChangeDestination();
            }
            else if (mechanism.GetComponent<BarrierController>() != null)
            {
                mechanism.GetComponent<BarrierController>().ChangeDestination();
            }
            else if (mechanism.GetComponent<TrapdoorController>() != null)
            {
                mechanism.GetComponent<TrapdoorController>().Activate();
            }
            else if (mechanism.GetComponent<DoorController>() != null)
            {
                mechanism.GetComponent<DoorController>().Activate();
            }
            else if (mechanism.GetComponent<CrusherController>() != null)
            {
                mechanism.GetComponent<CrusherController>().Activate();
            }
            else if (mechanism.GetComponent<LaserController>() != null)
            {
                mechanism.GetComponent<LaserController>().Activate();
            }
            else if (mechanism.GetComponent<PlatformController>() != null)
            {
                mechanism.GetComponent<PlatformController>().Activate();
            }
            else if (mechanism.GetComponent<SpawnEnemyOnEvent>() != null)
            {
                mechanism.GetComponent<SpawnEnemyOnEvent>().Spawn();
            }
            else if (mechanism.GetComponent<LightController>() != null)
            {
                mechanism.GetComponent<SpriteRenderer>().sprite = mechanism.GetComponent<LightController>().lightStates[0];
                mechanism.GetComponent<SpriteRenderer>().material = mechanism.GetComponent<LightController>().litMaterial;
                mechanism.GetComponent<LightController>().lightAttached.enabled = true;
                mechanism.GetComponent<LightController>().lightCollider.enabled = true;
                mechanism.GetComponent<LightController>().maskAttached.enabled = true;
                mechanism.GetComponent<LightController>().lightStatus = true;
            }
        }
    }
}
