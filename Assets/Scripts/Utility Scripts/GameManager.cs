using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public GameObject UITitle;
    public GameObject[] UIChapTitles;
    public GameObject UIPause;
    public int loadedScene;
    public Vector3 cameraPosition = new Vector3(0f, 0f, -10f);
    public Vector3 playerPosition = new Vector3 (0f, 0f);
    public int duskCharge = 0;

    public bool isNewGame = false;
    public bool isSavedGame = false;

    private UnityAction unityAction;
    private GameObject player;
    private new GameObject camera;
    private int[] level2Data = {0, 0, 0, 0};
    private bool timerReached;
    private float timer = 0;
    private bool isChangingLevel = false;
    private bool firstTime = true;
    public int sceneToLoad = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(UITitle);
            DontDestroyOnLoad(UIPause);
            DontDestroyOnLoad(GameObject.Find("EventSystem"));
            UIPause.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIChapTitles = new GameObject[3];
        UITitle = GameObject.Find("UITitleScreen");
        UIChapTitles[0] = GameObject.Find("UIChapterTitleScreen_Level1");
        UIChapTitles[1] = GameObject.Find("UIChapterTitleScreen_Level2");
        UIChapTitles[2] = GameObject.Find("UIChapterTitleScreen_Level3");
        DontDestroyOnLoad(UIChapTitles[0]);
        DontDestroyOnLoad(UIChapTitles[1]);
        DontDestroyOnLoad(UIChapTitles[2]);
        UIChapTitles[0].SetActive(false);
        UIChapTitles[1].SetActive(false);
        UIChapTitles[2].SetActive(false);
        firstTime = true;
    }

    private void Update()
    {
        if (firstTime)
        {
            firstTime = false;
            sceneToLoad = 1;
        }
        if (isNewGame || isChangingLevel)
        {
            sceneToLoad = loadedScene - 1;
        }
        else if (isSavedGame)
        {
            sceneToLoad = PlayerPrefs.GetInt("Scene") - 1;
        }
        int i = 0;
        foreach (GameObject chapTitle in UIChapTitles)
        {
            if (i == sceneToLoad)
                chapTitle.SetActive(true);
            else
                chapTitle.SetActive(false);
            i++;
        }


        if (!isChangingLevel)
        {
            if (UITitle.GetComponent<MainMenu>().ready || isChangingLevel)
            {
                UITitle.GetComponent<MainMenu>().ready = false;
                UITitle.GetComponent<MainMenu>().FadeMe();
                UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().StopAllCoroutines();
                UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().timer = 0;
                UIChapTitles[sceneToLoad].GetComponent<CanvasGroup>().alpha = 1;
                UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().ready = true;

                UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().finished = true;
            }
            if (UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().finished)
            {

                UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().finished = false;

                if (isNewGame)
                {
                    isNewGame = false;
                    Debug.Log("LoadNew");
                    StartCoroutine(LoadNewGame());
                }
                else if (isSavedGame)
                {
                    isSavedGame = false;
                    loadedScene = PlayerPrefs.GetInt("Scene");
                    Debug.Log("LoadSaved");
                    StartCoroutine(LoadGameFromSave());
                }
                unityAction = new UnityAction(SaveGame);
            }
        }
        else
        {
            UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().ready = true;
            UIChapTitles[sceneToLoad].GetComponent<UIChapterTitle>().finished = true;
            isChangingLevel = false;
            StartCoroutine(LoadNewLevel());
        }

        if (PlayerPrefs.HasKey("Scene") && !UIPause.activeInHierarchy)
        {
            if (Input.GetButtonDown("Submit") && UITitle.GetComponent<CanvasGroup>().alpha == 0)
            {
                Time.timeScale = 0;
                UIPause.SetActive(true);
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("ResumeButton"));
            }
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

    internal void LoadNewLevel(int newLevel, Vector3 newPlayerPosition)
    {
        loadedScene = newLevel;
        playerPosition = newPlayerPosition;
        cameraPosition = new Vector3 (0, 0, -10f);
        UITitle.GetComponent<MainMenu>().ready = true;
        isChangingLevel = true;
    }

    void SaveGame()
    {
        PlayerPrefs.SetInt("Scene", loadedScene);
        cameraPosition = new Vector3(camera.transform.position.x, camera.transform.position.y, -10f);
        PlayerPrefs.SetFloat("CameraX", camera.transform.position.x);
        PlayerPrefs.SetFloat("CameraY", camera.transform.position.y);
        playerPosition = new Vector3 (player.transform.position.x, player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        duskCharge = player.transform.Find("PivotArm").Find("Gun").gameObject.GetComponent<GunController>().currentCharge;
        PlayerPrefs.SetInt("GunCharge", duskCharge);

        GameObject[] finalMachineries = GameObject.FindGameObjectsWithTag("FinalMachineries");

        if (finalMachineries != null)
        {
            int i = 0;
            foreach (GameObject f in finalMachineries)
            {
                if (f.GetComponentInChildren<MachineryController>().powered)
                {
                    level2Data[i] = 1;
                }
                else
                {
                    level2Data[i] = 0;
                }
                PlayerPrefs.SetInt("FinalMachinery" + i, level2Data[i]);
                i++;
            }
        }
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
        loadedScene = 1;
        duskCharge = 0;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        SoundManager.Instance.as_soundtrack1.Stop();
        foreach(GameObject o in UIChapTitles)
        {
            Destroy(o);
        }
        Destroy(UITitle);
        Destroy(gameObject);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        player = GameObject.FindWithTag("Player");
        player.gameObject.GetComponent<PlayerInput>().enabled = false;
        foreach (LineRenderer r in player.gameObject.GetComponentsInChildren<LineRenderer>())
            r.enabled = false;
        player.gameObject.GetComponentInChildren<GunController>().enabled = false;
        UIPause = GameObject.Find("UIPauseScreen");
        UIPause.SetActive(false);
        EventManager.TriggerEvent("RestartGame");
    }

    IEnumerator LoadNewGame()
    {
        yield return new WaitForSeconds(2);
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(loadedScene, LoadSceneMode.Single);
        StartCoroutine("SearchPlayer");
    }

    IEnumerator LoadNewLevel()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(loadedScene, LoadSceneMode.Single);
        StartCoroutine("SearchPlayer");
    }

    IEnumerator LoadGameFromSave()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(PlayerPrefs.GetInt("Scene"), LoadSceneMode.Single);
        StartCoroutine("SearchPlayerFromSave");
    }

    IEnumerator WaitLoading()
    {
        //yield return new WaitForSeconds(1);
        Debug.Log("OkiDoki");
        while (!Input.GetButton("Retry"))
        {
            yield return null;
        }
        SoundManager.Instance.PlayOkSound();
        SceneManager.UnloadSceneAsync(loadedScene);
        SceneManager.LoadScene(loadedScene, LoadSceneMode.Single);
        StartCoroutine("SearchPlayer");
    }

    IEnumerator SearchPlayer()
    {
        yield return new WaitForSeconds(0.01f);
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");

        foreach (GameObject chapTitle in UIChapTitles)
        {
            chapTitle.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
        }
        UITitle.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
        camera.transform.position = cameraPosition;
        player.transform.position = playerPosition;
        camera.GetComponent<CameraController>().ActivateEnemies();
        camera.GetComponent<CameraController>().SaveCameraPosition();
        player.transform.Find("PivotArm").Find("Gun").gameObject.GetComponent<GunController>().currentCharge = duskCharge;
        GameObject[] finalMachineries = GameObject.FindGameObjectsWithTag("FinalMachineries");
        if (finalMachineries != null)
        {
            int i = 0;
            foreach (GameObject f in finalMachineries)
            {
                if (level2Data[i] == 1)
                    f.GetComponentInChildren<MachineryController>().InstantSwitchOn();
                i++;
            }
        }
        SaveGame();
        StopAllCoroutines();
    }

    IEnumerator SearchPlayerFromSave()
    {
        yield return new WaitForSeconds(0.01f);
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");

        foreach (GameObject chapTitle in UIChapTitles)
        {
            chapTitle.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
        }

        UITitle.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
        camera.transform.position = new Vector3(PlayerPrefs.GetFloat("CameraX"), PlayerPrefs.GetFloat("CameraY"), -10);
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), 0);
        camera.GetComponent<CameraController>().ActivateEnemies();
        camera.GetComponent<CameraController>().SaveCameraPosition();
        player.transform.Find("PivotArm").Find("Gun").gameObject.GetComponent<GunController>().currentCharge = PlayerPrefs.GetInt("GunCharge");

        GameObject[] finalMachineries = GameObject.FindGameObjectsWithTag("FinalMachineries");
        if (finalMachineries != null)
        {
            int i = 0;
            foreach (GameObject f in finalMachineries)
            {
                if (PlayerPrefs.GetInt("FinalMachinery" + i) == 1)
                    f.GetComponentInChildren<MachineryController>().InstantSwitchOn();
                i++;
            }
        }
        SaveGame();
    }

    public void ResumeButton()
    {
        UIPause = GameObject.Find("UIPauseScreen");
        UIPause.SetActive(false);
        Time.timeScale = 1;
    }

    public void SaveButton()
    {
        StartCoroutine(SearchPlayerFromSave());
        cameraPosition = new Vector3(0f, 0f, -10f);
        playerPosition = new Vector3(0f, 5f, 0f);
        Application.Quit();
    }
}
