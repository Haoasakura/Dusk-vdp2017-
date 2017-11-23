using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGunCharge : MonoBehaviour {

    public GameObject gun;

    private Text text;

	// Use this for initialization
	void Start () {
        gun = GameObject.Find("Player").transform.Find("PivotArm").Find("Gun").gameObject;
        text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        text.text = "Dusk Gun Charge: ";
        text.text += gun.GetComponent<GunController>().currentCharge.ToString();
        text.text += " %";
	}
}
