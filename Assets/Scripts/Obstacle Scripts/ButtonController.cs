﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    [Header("TRUE -> Pressure Plate; FALSE -> Set Reset Button")]
    [SerializeField]
    private bool isPressurePlate;

    [Header("GameObject associated with linked mechanism")]
    [SerializeField]
    private GameObject mechanism;

    private bool isFirstPass = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPressurePlate && isFirstPass)
        {
            SoundManager.Instance.Button(0.95f);
            isFirstPass = false;
        }
        else if (!isPressurePlate && !isFirstPass)
        {
            SoundManager.Instance.Button(1.05f);
            isFirstPass = true;
        }
        else
        {
            SoundManager.Instance.Button(0.95f);
        }
        Activate();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isPressurePlate)
        {
            SoundManager.Instance.Button(1.05f);
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
