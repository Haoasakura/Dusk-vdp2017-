using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameOver : MonoBehaviour {

    public GameObject UIText;
    public GameObject UIOverlay;


	// Use this for initialization
	void Start () {
        UIText.SetActive(false);
        UIOverlay.SetActive(false);
        EventManager.StartListening("PlayerDied", StartGameOver);
    }

    private void StartGameOver()
    {
        StartCoroutine(ShowUI());
    }

    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(1);
        UIText.SetActive(true);
        UIOverlay.SetActive(true);
    }
}
