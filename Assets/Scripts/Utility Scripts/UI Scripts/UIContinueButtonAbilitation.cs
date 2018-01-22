using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContinueButtonAbilitation : MonoBehaviour {

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Scene"))
        {
            GameObject.Find("ContinueButton").GetComponent<Button>().interactable = false;
        }
    }
}
