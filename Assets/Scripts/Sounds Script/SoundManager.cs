using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    [Header("Player Audio")]
    public AudioSource as_player;
    public AudioClip ac_jump;
    public AudioClip ac_walk;
    public AudioClip ac_climb;

    [Header("Gun Audio")]
    public AudioSource as_gun;
    public AudioClip ac_gunshot;
    public AudioClip ac_emptygunshot;

    [Header("Objects Audio")]
    public AudioSource as_objects;
    public AudioClip ac_lever;
    public AudioClip ac_movingPlatform;
    public AudioClip ac_chekpointReached;

    private float as_playerPitch;

    private float lowPitchRange = .95f;
    private float highPitchRange = 1.05f;

    [Header("Soundtrack")]
    public AudioSource as_soundtrack1;
    public AudioSource as_soundtrack2;
    public AudioClip ac_level11;
    public AudioClip ac_level12;


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

    //Player Sounds
    public void Jump()
    {
        as_player.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
        as_player.clip = ac_jump;
        as_player.Play();
    }

    public void Walk()
    {
        as_player.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
        if (!as_player.isPlaying)
        {
            as_player.PlayOneShot(ac_walk);
        }
    }

    internal void Climb()
    {
        as_player.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
        if (!as_player.isPlaying)
        {
            as_player.PlayOneShot(ac_climb);
        }
    }

    //Gun Sounds
    public void Gunshot(float pitch)
    {
        as_gun.pitch = UnityEngine.Random.Range(pitch*lowPitchRange, pitch*highPitchRange);
        as_gun.PlayOneShot(ac_gunshot);
    }

    public void EmptyGunshot()
    {
        as_gun.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
        if (!as_gun.isPlaying)
        {
            as_gun.PlayOneShot(ac_emptygunshot);
        }
    }

    internal void GunshotStop()
    {
        as_gun.Stop();
    }

    //ObjectSounds
    public void Lever()
    {
        as_objects.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
        if (!as_objects.isPlaying)
        {
            as_objects.PlayOneShot(ac_lever);
        }
    }

    public void Checkpoint()
    {
        as_objects.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
        as_objects.PlayOneShot(ac_chekpointReached);
    }

    public void ReturnToNormalSoundtrack()
    {
            as_soundtrack1.mute = false;
            as_soundtrack1.volume = 1;
            while (as_soundtrack2.volume > 0.1f)
            {
                as_soundtrack2.volume -= Time.deltaTime / 3;
            }
            as_soundtrack2.mute = true;
    }

    public void ChangeSoundtrack()
    {
        if (!as_soundtrack1.mute)
        {
            as_soundtrack2.mute = false;
            as_soundtrack2.volume = 1;
            while (as_soundtrack1.volume > 0.1f)
            {
                as_soundtrack1.volume -= Time.deltaTime / 3;
            }
            as_soundtrack1.mute = true;
        }
        else
        {
            as_soundtrack1.mute = false;
            as_soundtrack1.volume = 1;
            while (as_soundtrack2.volume > 0.1f)
            {
                as_soundtrack2.volume -= Time.deltaTime / 3;
            }
            as_soundtrack2.mute = true;
        }
    }

}