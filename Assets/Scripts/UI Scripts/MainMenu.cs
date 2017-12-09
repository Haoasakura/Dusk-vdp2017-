using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool ready = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") && gameObject.GetComponent<CanvasGroup>().alpha == 1)
        {
            GetComponentInChildren<Transform>().GetChild(2).gameObject.SetActive(false);
            GetComponentInChildren<Transform>().GetChild(3).gameObject.SetActive(true);
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