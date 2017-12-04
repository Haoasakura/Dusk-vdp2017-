using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChapterTitle : MonoBehaviour {

    public GameObject title;

    private bool timerReached = false;
    private float timer = 0;

     // Update is called once per frame
    void Update () {
        FadeText();
        if (!timerReached)
        {
            timer += Time.deltaTime;
        }
        if (!timerReached && timer > 4)
        {
            Debug.Log("Done waiting");
            FadeMe();
        }
	}

    void FadeMe()
    {
        StartCoroutine(FadeOutAll());
    }

    void FadeText()
    {
        StartCoroutine(FadeInTitle());
    }

    IEnumerator Stop() {
        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
    }

    IEnumerator FadeOutAll()
    {
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime / 100;
            yield return null;
        }
        cg.interactable = false;
        yield return null;
    }

    IEnumerator FadeInTitle()
    {
        CanvasGroup cg = title.GetComponent<CanvasGroup>();
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime / 50;
            yield return null;
        }
        cg.interactable = false;
        yield return null;
    }
}
