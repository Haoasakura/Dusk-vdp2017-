using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool ready = false;
    private bool buttonsOut = false;

    public GameObject optionsText;
    public GameObject loadingText;
    public GameObject buttonBox;
    public GameObject title;
    public GameObject background;
    public GameObject settings;
    public GameObject credits;

    // Update is called once per frame
    void Update()
    {
        bool anyKeyPressed = false;
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                anyKeyPressed = true;
                break;
            }
        }
        if(anyKeyPressed && !buttonsOut)
        {
            GameObject.Find("OptionText").SetActive(false);
            StartCoroutine(MoveFromTo(title, title.GetComponent<RectTransform>().position,
                                      title.GetComponentInChildren<RectTransform>().GetChild(0).position, 2f));
            StartCoroutine(MoveFromTo(background, background.GetComponent<RectTransform>().position,
                                      background.GetComponentInChildren<RectTransform>().GetChild(0).position, 2f));
            buttonBox.SetActive(true);
            StartCoroutine(FadeButtons());
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find(null));
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("PlayButton"));
            buttonsOut = true;
        }
        if (credits.activeInHierarchy && Input.GetButtonDown("Submit"))
        {
            credits.SetActive(false);
            buttonBox.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("CreditsButton"));
        }
    }

    public void PlayButton() {
        Debug.Log("Hello");
        SoundManager.Instance.PlayOkSound();
        loadingText.SetActive(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().isSavedGame = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().isNewGame = true;
        ready = true;
    }

    public void PlayButtonLevel1()
    {
        Debug.Log("Hello");
        SoundManager.Instance.PlayOkSound();
        loadingText.SetActive(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().loadedScene = 1;
        GameObject.Find("GameManager").GetComponent<GameManager>().cameraPosition = new Vector3(0f, 0f, -10f);
        GameObject.Find("GameManager").GetComponent<GameManager>().playerPosition = new Vector3(0f, 0f, 0f);
        GameObject.Find("GameManager").GetComponent<GameManager>().duskCharge = 0;


        GameObject.Find("GameManager").GetComponent<GameManager>().isSavedGame = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().isNewGame = true;
        ready = true;
    }

    public void PlayButtonLevel2()
    {
        Debug.Log("Hello");
        SoundManager.Instance.PlayOkSound();
        loadingText.SetActive(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().loadedScene = 2;
        GameObject.Find("GameManager").GetComponent<GameManager>().cameraPosition = new Vector3(0f, 0f, -10f);
        GameObject.Find("GameManager").GetComponent<GameManager>().playerPosition = new Vector3(0f, 5f, 0f);
        GameObject.Find("GameManager").GetComponent<GameManager>().duskCharge = 0;
        GameObject.Find("GameManager").GetComponent<GameManager>().isSavedGame = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().isNewGame = true;
        ready = true;
    }

    public void PlayButtonLevel3()
    {
        Debug.Log("Hello");
        SoundManager.Instance.PlayOkSound();
        loadingText.SetActive(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().loadedScene = 3;
        GameObject.Find("GameManager").GetComponent<GameManager>().cameraPosition = new Vector3(0f, 0f, -10f);
        GameObject.Find("GameManager").GetComponent<GameManager>().playerPosition = new Vector3(0f, 0f, 0f);
        GameObject.Find("GameManager").GetComponent<GameManager>().duskCharge = 0;
        GameObject.Find("GameManager").GetComponent<GameManager>().isSavedGame = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().isNewGame = true;
        ready = true;
    }

    public void ContinueButton()
    {
        SoundManager.Instance.PlayOkSound();
        loadingText.SetActive(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().isNewGame = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().isSavedGame = true;
        ready = true;
        Debug.Log("Ready: "+ready);
    }

    public void SettingsButton() {
        SoundManager.Instance.PlayOkSound();
        buttonBox.SetActive(false);
        settings.SetActive(true);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("BackButton"));
    }

    public void BackButton()
    {
        SoundManager.Instance.PlayOkSound();
        settings.SetActive(false);
        buttonBox.SetActive(true);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("SettingsButton"));
    }

    public void QuitGameButton() {
        SoundManager.Instance.PlayOkSound();
        EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void CreditsButton() {
        SoundManager.Instance.PlayOkSound();
        buttonBox.SetActive(false);
        credits.SetActive(true);
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

    IEnumerator FadeButtons()
    {
        CanvasGroup cg = GameObject.Find("ButtonBox").GetComponent<CanvasGroup>();
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime / 3;
            yield return null;
        }
        yield return null;
    }

    IEnumerator MoveFromTo(GameObject toBeMoved, Vector3 pointA, Vector3 pointB, float duration)
    {
        bool moving = false;

        if (!moving)
        {
            moving = true;
            float t = 0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime / duration;
                toBeMoved.GetComponent<RectTransform>().position = Vector3.Lerp(pointA, pointB, t);
                yield return null;
            }
            moving = false;
        }
    }
}