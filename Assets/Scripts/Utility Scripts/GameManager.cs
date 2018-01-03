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

    private UnityAction unityAction;
    private GameObject player;
    private new GameObject camera;
    private int[] level2Data = {0, 0, 0, 0};
    private bool timerReached;
    private float timer = 0;



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
    }

    private void Update()
    {
        UIChapTitles[loadedScene - 1].SetActive(true);
        if (UITitle.GetComponent<MainMenu>().ready)
        {
            UITitle.GetComponent<MainMenu>().ready = false;
            UITitle.GetComponent<MainMenu>().FadeMe();

            UIChapTitles[loadedScene-1].GetComponent<UIChapterTitle>().ready = true;
            UIChapTitles[loadedScene-1].GetComponent<UIChapterTitle>().finished = true;
        }
        if (UIChapTitles[loadedScene-1].GetComponent<UIChapterTitle>().finished) {
            UIChapTitles[loadedScene-1].GetComponent<UIChapterTitle>().finished = false;
            
            if (loadedScene == 1) {
                StartCoroutine(LoadNewGame());
            }
            else
            {
                LoadGame();
            }
            unityAction = new UnityAction(SaveGame);
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

    IEnumerator LoadNewGame()
    {
        yield return new WaitForSeconds(2);
        PlayerPrefs.DeleteAll();
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
        yield return new WaitForSeconds(1);
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
        if (!(player == null))
        {
            Debug.Log("PlayerFound");
        }

        Debug.Log(player.transform.position);
        UIChapTitles[loadedScene-1].GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
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
    }

    IEnumerator SearchPlayerFromSave()
    {

        yield return new WaitForSeconds(0.01f);
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");

        UIChapTitles[loadedScene - 1].GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
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
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        SaveGame();
    }
}
