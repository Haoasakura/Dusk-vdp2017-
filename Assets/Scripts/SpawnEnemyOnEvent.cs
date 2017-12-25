using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyOnEvent : MonoBehaviour {


    [Header("Enemy associated")]
    [SerializeField]
    private GameObject enemy;

    private GameObject e;

    [Header("Spawn Point associated")]
    [SerializeField]
    private Transform t;

    public void Spawn()
    {
        if (e == null)
        {
            e = Instantiate(enemy, t.position, Quaternion.identity, null);
        }

        if (e.transform.GetChild(0).GetComponent<Animator>() != null)
        {
            e.transform.GetChild(0).GetComponent<Animator>().SetBool("Idle", false);
        }
    }
}
