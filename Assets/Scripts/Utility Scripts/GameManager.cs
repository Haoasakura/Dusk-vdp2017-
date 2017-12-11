using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public GameObject UITitle;
    public GameObject UIChapTitle;
    public int loadedScene;
    public Vector3 cameraPosition = new Vector3(0f, 0f, -10f);
    public Vector3 playerPosition = new Vector3 (0f, 0f);
    public int duskCharge = 0;

    private UnityAction unityAction;
    private GameObject player;
    private new GameObject camera;
    private bool timerReached;
    private float timer = 0;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(UITitle);
            DontDestroyOnLoad(UIChapTitle);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UITitle = GameObject.Find("UITitleScreen");
        UIChapTitle = GameObject.Find("UIChapterTitleScreen");
    }

    private void Update()
    {
        if (UITitle.GetComponent<MainMenu>().ready)
        {
            UITitle.GetComponent<MainMenu>().ready = false;
            UITitle.GetComponent<MainMenu>().FadeMe();

            UIChapTitle.GetComponent<UIChapterTitle>().ready = true;
            UIChapTitle.GetComponent<UIChapterTitle>().finished = true;
        }
        if (UIChapTitle.GetComponent<UIChapterTitle>().finished) {
            UIChapTitle.GetComponent<UIChapterTitle>().finished = false;
            LoadGame();
            unityAction = new UnityAction(SaveGame);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("CheckpointReached", SaveGame);
        EventManager.StartListening("ReloadScene", ReloadGame);
        EventManager.StartListening("RestartGame", RestartGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CheckpointReached", SaveGame);
    }

    void SaveGame()
    {
        cameraPosition = new Vector3(camera.transform.position.x, camera.transform.position.y, -10f);
        playerPosition = new Vector3 (player.transform.position.x, player.transform.position.y);
        duskCharge = player.transform.Find("PivotArm").Find("Gun").gameObject.GetComponent<GunController>().currentCharge;
        Debug.Log("Saving");
    }

    void LoadGame()
    {
        SceneManager.LoadScene(loadedScene, LoadSceneMode.Single);
        StartCoroutine("SearchPlayer");
    }

    void ReloadGame()
    {
        StartCoroutine("WaitLoading");
    }

    void RestartGame()
    {
        cameraPosition = new Vector3(0f, 0f, -10f);
        playerPosition = new Vector3(-12f, -5f);
        duskCharge = 0;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        SoundManager.Instance.as_soundtrack1.Stop();
        Destroy(UIChapTitle);
        Destroy(UITitle);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {

        
    }

    IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(1);
        while (!Input.GetButton("Retry"))
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync(loadedScene);
        SceneManager.LoadScene(loadedScene, LoadSceneMode.Single);
        StartCoroutine("SearchPlayer");
    }

    IEnumerator SearchPlayer()
    {
        yield return new WaitForSeconds(0.01f);
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        if (!(player == null))
        {
            Debug.Log("PlayerFound");
        }
        UITitle.GetComponent<MainMenu>().optionsText.SetActive(false);
        UITitle.GetComponent<MainMenu>().loadingText.SetActive(true);
        Debug.Log(player.transform.position);
        UIChapTitle.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
        UITitle.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();

        camera.transform.position = cameraPosition;
        player.transform.position = playerPosition;
        camera.GetComponent<CameraController>().ActivateEnemies();
        player.transform.Find("PivotArm").Find("Gun").gameObject.GetComponent<GunController>().currentCharge = duskCharge;
        SaveGame();
    }


}
