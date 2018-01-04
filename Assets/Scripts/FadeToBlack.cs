using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToBlack : MonoBehaviour {

    private bool isOver = false;

    private void Update()
    {
        if (isOver)
        {
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick button " + i))
                {
                    EventManager.TriggerEvent("RestartGame");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.player))
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                SoundManager.Instance.EndSoundtrack();
                GetComponent<AudioSource>().Play();
            }
            collision.gameObject.GetComponent<PlayerInput>().enabled = false;
            collision.gameObject.GetComponentInChildren<GunController>().enabled = false;
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
        yield return new WaitForSeconds(77);
        CanvasGroup cg = gameObject.GetComponentInChildren<CanvasGroup>();
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime / 5;
            yield return null;
        }
        cg.interactable = false;
        GameObject.Find("UIChapterTitleScreen_Level3").GetComponent<CanvasGroup>().alpha = 1;
        yield return null;
    }

}
