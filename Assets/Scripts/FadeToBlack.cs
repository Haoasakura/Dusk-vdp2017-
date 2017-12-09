using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToBlack : MonoBehaviour {

    public GameObject lever;

    private bool isOver = false;

    private void Update()
    {
        if (isOver)
        {
            if (Input.GetButton("Submit"))
            {
                EventManager.TriggerEvent("RestartGame");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.player))
        {
            if (!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();

            lever.GetComponent<BoxCollider2D>().enabled = false;
            FadeMe();
        }
    }

    public void FadeMe()
    {
        isOver = true;
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(30);
        CanvasGroup cg = gameObject.GetComponentInChildren<CanvasGroup>();
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime / 10;
            yield return null;
        }
        cg.interactable = false;
        yield return null;
    }

}
