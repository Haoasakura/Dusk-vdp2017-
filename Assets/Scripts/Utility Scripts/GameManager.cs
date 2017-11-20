using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private UnityAction unityAction;
    private GameObject player;
    
    private Vector3 playerPosition = new Vector3 (0f, 0f);

    private void Awake()
    {        
        DontDestroyOnLoad(transform.gameObject);
        LoadGame();
        unityAction = new UnityAction(SaveGame);
    }

    private void OnEnable()
    {
        EventManager.StartListening("CheckpointReached", SaveGame);
        EventManager.StartListening("PlayerDied", LoadGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CheckpointReached", SaveGame);
        EventManager.StartListening("PlayerDied", LoadGame);
    }

    void SaveGame()
    {
        Debug.Log(player.transform.position);

        playerPosition = new Vector3 (player.transform.position.x, player.transform.position.y);
        Debug.Log("Saving");
    }

    void LoadGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        StartCoroutine("SearchPlayer");
    }

    IEnumerator SearchPlayer()
    {
        yield return new WaitForSeconds(0.01f);
        player = GameObject.FindWithTag("Player");
        if (!(player == null))
        {
            Debug.Log("PlayerFound");
        }
        Debug.Log(player.transform.position);
        player.transform.position = playerPosition;       
    }
}
