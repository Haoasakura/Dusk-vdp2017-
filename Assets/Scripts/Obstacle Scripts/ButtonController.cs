using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    public Sprite[] sprites;

    [Header("TRUE -> Pressure Plate; FALSE -> Set Reset Button")]
    [SerializeField]
    private bool isPressurePlate;

    [Header("GameObject associated with linked mechanism")]
    [SerializeField]
    private GameObject mechanism;

    private bool isFirstPass = true;
    private ObjectSoundManager osm;

    private void Start()
    {
        osm = GetComponent<ObjectSoundManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[1];
        if (!isPressurePlate && isFirstPass)
        {
            osm.PlaySound(0.95f);
            isFirstPass = false;
        }
        else if (!isPressurePlate && !isFirstPass)
        {
            osm.PlaySound(1.05f);
            isFirstPass = true;
        }
        else
        {
            osm.PlaySound(0.95f);
        }
        Activate();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[0];
        if (isPressurePlate)
        {
            osm.PlaySound(1.05f);
            Activate();
        }
    }

    private void Activate()
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
    }
}
