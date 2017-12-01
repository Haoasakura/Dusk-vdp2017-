using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public static GameObject[] CheckpointList;

    private Animator animator;
    private bool isUsed = false;

	// Use this for initialization
	void Start () {
        CheckpointList = GameObject.FindGameObjectsWithTag("Checkpoint");
        animator = GetComponent<Animator>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && !isUsed)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        foreach (GameObject cp in CheckpointList)
        {
            cp.GetComponent<Checkpoint>().isUsed = false;
            cp.GetComponent<Animator>().SetInteger("State", 2);
        }
        animator.SetInteger("State", 1);
        SoundManager.Instance.Checkpoint();
        EventManager.TriggerEvent("CheckpointReached");
        isUsed = true;
    }
}
