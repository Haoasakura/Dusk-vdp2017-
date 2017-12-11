﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool ready = false;

    public GameObject optionsText;
    public GameObject loadingText;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") && gameObject.GetComponent<CanvasGroup>().alpha == 1)
        {
            ready = true;
        }
    }

    public void FadeMe()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        AudioSource audio = GetComponent<AudioSource>();
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        while(cg.alpha > 0)
        {
            audio.volume -= Time.deltaTime / 2;
            cg.alpha -= Time.deltaTime / 2;
            yield return null;
        }
        audio.volume = 0;
        cg.interactable = false;
        yield return null;
    }
    
}