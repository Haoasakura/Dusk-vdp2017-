using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContinueButtonAbilitation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetInt("Scene", -1) == -1)
        {
            GameObject.Find("ContinueButton").GetComponent<Button>().interactable = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (PlayerPrefs.GetInt("Scene", -1) == -1)
        {
            GameObject.Find("ContinueButton").GetComponent<Button>().interactable = false;
        }
    }
}
