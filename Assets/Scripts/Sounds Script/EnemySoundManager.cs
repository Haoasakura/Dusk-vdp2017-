﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour {

    [Header("Enemy Audio")]
    public AudioSource as_enemy;
    public AudioClip ac_walk;
    public AudioClip ac_climb;

    [Header("Gun Audio")]
    public AudioSource as_gun;
    public AudioClip ac_gunshot;
    public AudioClip ac_emptygunshot;

    private float lowPitchRange = .95f;
    private float highPitchRange = 1.05f;

    //Enemy Sounds
    public void Walk()
    {
        as_enemy.pitch = Random.Range(lowPitchRange, highPitchRange);
        if (!as_enemy.isPlaying)
        {
            as_enemy.PlayOneShot(ac_walk);
        }
    }

    internal void Climb()
    {
        as_enemy.pitch = Random.Range(lowPitchRange, highPitchRange);
        if (!as_enemy.isPlaying)
        {
            as_enemy.PlayOneShot(ac_climb);
        }
    }

    //Gun Sounds
    public void Gunshot(float pitch)
    {
        as_gun.pitch = Random.Range(pitch * lowPitchRange, pitch * highPitchRange);
        as_gun.PlayOneShot(ac_gunshot);
    }

    public void EmptyGunshot()
    {
        as_gun.pitch = Random.Range(lowPitchRange, highPitchRange);
        if (!as_gun.isPlaying)
        {
            as_gun.PlayOneShot(ac_emptygunshot);
        }
    }
}