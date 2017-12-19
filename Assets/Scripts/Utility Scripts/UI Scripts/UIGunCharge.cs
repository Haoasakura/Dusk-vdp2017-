using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGunCharge : MonoBehaviour {

    public GameObject gun;
    int charge;
    private GameObject currentControl;

    private Image duskMeter;
    private bool isDead = false;

    public Sprite[] sprites = new Sprite[5];

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDied", PlayerDied);
        EventManager.StartListening("PlayerControlled", PlayerDied);
    }

    private void PlayerDied()
    {
        isDead = true;
    }

    // Use this for initialization
    void Start () {
        gun = GameObject.Find("Player").transform.Find("PivotArm").Find("Gun").gameObject;
        duskMeter = GetComponent<Image>();
        currentControl = gun.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if ( gun != null && gun.gameObject.transform.parent.parent.GetComponent<Player>().controlling)
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy))
            {
                if (enemy.GetComponent<EnemyController>().controlled)
                {
                    currentControl = enemy.transform.Find("PivotArm").Find("Gun").gameObject;
                    break;
                }
                else
                {
                    currentControl = gun.gameObject;
                }
            }
        }
        else
        {
            currentControl = gun.gameObject;
        }
        if (currentControl == gun.gameObject)
        {
            if (isDead == false)
            {
                charge = gun.GetComponent<GunController>().currentCharge;
                if (charge == 0)
                {
                    duskMeter.sprite = sprites[0];
                }
                else if (charge == 25)
                {
                    duskMeter.sprite = sprites[1];
                }
                else if (charge == 50)
                {
                    duskMeter.sprite = sprites[2];
                }
                else if (charge == 75)
                {
                    duskMeter.sprite = sprites[3];
                }
                else if (charge == 100)
                {
                    duskMeter.sprite = sprites[4];
                }
            }
        }
        else
        {
            charge = currentControl.GetComponent<EnemyWeapon>().currentCharge;
            if (charge == 0)
            {
                duskMeter.sprite = sprites[0];
            }
            else if (charge == 25)
            {
                duskMeter.sprite = sprites[1];
            }
            else if (charge == 50)
            {
                duskMeter.sprite = sprites[2];
            }
            else if (charge == 75)
            {
                duskMeter.sprite = sprites[3];
            }
            else if (charge == 100)
            {
                duskMeter.sprite = sprites[4];
            }
        }
    }
}
