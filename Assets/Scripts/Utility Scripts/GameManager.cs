using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int loadedScene;

    private UnityAction unityAction;
    private GameObject player;
    private GameObject camera;

    public Vector3 cameraPosition = new Vector3(0f, 0f, -10f);
    public Vector3 playerPosition = new Vector3 (0f, 0f);
    private int duskCharge = 0;

    private void Awake()
    {        
        DontDestroyOnLoad(transform.gameObject);
        LoadGame();
        unityAction = new UnityAction(SaveGame);
    }

    private void OnEnable()
    {
        EventManager.StartListening("CheckpointReached", SaveGame);
        EventManager.StartListening("ReloadScene", LoadGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CheckpointReached", SaveGame);
        EventManager.StartListening("PlayerDied", LoadGame);
    }

    void SaveGame()
    {
        Debug.Log(player.transform.position);

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
        camera.transform.position = cameraPosition;
        player.transform.position = playerPosition;
        player.transform.Find("PivotArm").Find("Gun").gameObject.GetComponent<GunController>().currentCharge = duskCharge;
    }
}
