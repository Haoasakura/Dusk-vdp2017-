using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelFade : MonoBehaviour {

    public int newLevel;
    public Vector3 newPlayerPosition;

    private GameObject gm;

    private void Start()
    {
        gm = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(Tags.player))
        {
            collision.tag = "Untagged";
            collision.gameObject.GetComponent<PlayerInput>().enabled = false;
            collision.gameObject.GetComponentInChildren<GunController>().enabled = false;
            FadeMe();
        }
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
        gm.GetComponent<GameManager>().LoadNewLevel(newLevel, newPlayerPosition);
        cg.interactable = false;
        yield return null;
    }
}
