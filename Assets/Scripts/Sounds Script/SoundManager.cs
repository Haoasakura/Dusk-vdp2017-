﻿using System.Collections;
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
    public AudioClip ac_button;
    public AudioClip ac_trapdoor;
    public AudioClip ac_movingPlatform;
    public AudioClip ac_barrier;
    public AudioClip ac_chekpointReached;

    private float as_playerPitch;

    private float lowPitchRange = .95f;
    private float highPitchRange = 1.05f;


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
        as_player.pitch = Random.Range(lowPitchRange, highPitchRange);
        as_player.clip = ac_jump;
        as_player.Play();
    }

    public void Walk()
    {
        as_player.pitch = Random.Range(lowPitchRange, highPitchRange);
        if (!as_player.isPlaying)
        {
            as_player.PlayOneShot(ac_walk);
        }
    }

    internal void Climb()
    {
        as_player.pitch = Random.Range(lowPitchRange, highPitchRange);
        if (!as_player.isPlaying)
        {
            as_player.PlayOneShot(ac_climb);
        }
    }

    //Gun Sounds
    public void Gunshot(float pitch)
    {
        as_gun.pitch = Random.Range(pitch*lowPitchRange, pitch*highPitchRange);
        as_gun.PlayOneShot(ac_gunshot);
    }

    public void EmptyGunshot()
    {
        as_gun.pitch = Random.Range(lowPitchRange, highPitchRange);
        if (!as_gun.isPlaying)
        {
            as_player.PlayOneShot(ac_emptygunshot);
        }
    }

    //ObjectSounds
    public void Lever()
    {
        as_objects.pitch = Random.Range(lowPitchRange, highPitchRange);
        if (!as_objects.isPlaying)
        {
            as_objects.PlayOneShot(ac_lever);
        }
    }

    public void Barrier()
    {
        as_objects.pitch = Random.Range(lowPitchRange, highPitchRange);
        as_objects.PlayOneShot(ac_barrier);

    }

    public void Trapdoor(float pitch)
    {
        as_objects.pitch = pitch;
        as_objects.PlayOneShot(ac_trapdoor);

    }

    public void Button(float pitch)
    {
        as_objects.pitch = pitch;
        as_objects.PlayOneShot(ac_button);

    }

    public void Checkpoint()
    {
        as_objects.pitch = Random.Range(lowPitchRange, highPitchRange);
        as_objects.PlayOneShot(ac_chekpointReached);
    }
}