using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGunCharge : MonoBehaviour {

    public GameObject gun;
    int charge;

    private Image duskMeter;

    public Sprite[] sprites = new Sprite[5];

	// Use this for initialization
	void Start () {
        gun = GameObject.Find("Player").transform.Find("PivotArm").Find("Gun").gameObject;
        duskMeter = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
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
