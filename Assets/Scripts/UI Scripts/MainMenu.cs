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
        if(Input.GetButtonDown("Submit") && gameObject.GetComponent<CanvasGroup>().alpha != 0)
        {
            ready = true;
        }
    }

    public void FadeMe()
    {
        StartCoroutine(Fade());
        //yield return new WaitForSeconds(3f);
        ready = false;
    }

    IEnumerator Fade()
    {
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        while(cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime / 2;
            yield return null;
        }
        cg.interactable = false;
        yield return null;
    }
    
}