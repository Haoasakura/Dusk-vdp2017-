using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelFade : MonoBehaviour {

    private GameObject gm;

    private void Start()
    {
        gm = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeMe();
        GameObject.Find("UIChapterTitleScreen_Level1").SetActive(false);
    }

    public void FadeMe()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        CanvasGroup cg = gameObject.GetComponentInChildren<CanvasGroup>();
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime / 3;
            yield return null;
        }
        gm.GetComponent<GameManager>().loadedScene = 2;
        cg.interactable = false;
        yield return null;
    }
}
