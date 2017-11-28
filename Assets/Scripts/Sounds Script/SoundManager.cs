using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    [Header("Player Audio")]
    public AudioSource as_player;
    public AudioClip ac_jump;
    private float as_playerPitch;

    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Jump()
    {
        as_playerPitch = as_player.pitch;
        as_player.pitch += Random.Range(-1, 1);
        as_player.PlayOneShot(ac_jump);
        as_player.pitch = as_playerPitch;
    }

    public void Walk()
    {

    }

}