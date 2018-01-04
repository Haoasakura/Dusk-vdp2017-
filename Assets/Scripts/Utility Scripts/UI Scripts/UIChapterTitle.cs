using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChapterTitle : MonoBehaviour {

    public GameObject title;
    public bool ready;
    public bool finished;
    public bool textDone;
    public AudioClip ac_soundtrack1;

    private bool timerReached = false;
    private float timer = 0;

    private void Start()
    {
        ready = false;
        finished = false;
        textDone = false;
    }

    // Update is called once per frame
    void Update () {
        if (ready)
        {
            if (!textDone)
            {
                if (!timerReached)
                {
                    timer += Time.deltaTime;
                }
                if (!timerReached && timer > 3)
                {
                    FadeText();
                }
            }
            if(!textDone && title.GetComponent<CanvasGroup>().alpha == 1)
            {
                textDone = true;
                timer = 0;
            }
            if (textDone)
            {
                if (!timerReached)
                {
                    timer += Time.deltaTime;
                }
                if (!timerReached && timer > 3)
                {
                    FadeMe();
                }
            }
        }
        if(gameObject.GetComponent<CanvasGroup>().alpha == 0)
        {
            ready = false;
        }
    }

    public void FadeMe()
    {
        StartCoroutine(FadeOutAll());
    }

    public void FadeText()
    {
        StartCoroutine(FadeInTitle());
    }

    IEnumerator FadeOutAll()
    {

        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime / 100;
            yield return null;
        }
        SoundManager.Instance.ReturnPlayerSound();
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

        SoundManager.Instance.as_soundtrack1.clip = ac_soundtrack1;

        SoundManager.Instance.as_soundtrack1.Play();
        SoundManager.Instance.ReturnSounds();
        yield return null;
    }
}
