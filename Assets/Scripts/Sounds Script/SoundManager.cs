using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    [Header("Player Audio")]
    public AudioSource as_player;
    public AudioClip ac_jump;

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
        as_player.PlayOneShot(ac_jump);
    }

    public void Walk()
    {

    }

}