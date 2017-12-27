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
    public AudioClip ac_fall;


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
    public float fadeTime1;
    public float fadeTime2;
    public int enemiesOnChase = 0;

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

    internal void PlayFallSound()
    {
        as_player.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
        if (!as_player.isPlaying)
        {
            as_player.PlayOneShot(ac_fall);
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

    public void PlayNormalSoundtrackFromDeath()
    {
        enemiesOnChase = 0;
        float t = fadeTime1;
        as_soundtrack1.mute = false;
        as_soundtrack1.volume = 0.5f;
        StartCoroutine(FadeSountrack(as_soundtrack2, as_soundtrack1, t));
    }

    public void EndSoundtrack()
    {
        float t = fadeTime2;
        StartCoroutine(EndSoundtrack(as_soundtrack1, t));
    }

    public void PlayNormalSoundtrack()
    {
        if (enemiesOnChase > 0)
        {
            enemiesOnChase--;
        }
        else
        {
            enemiesOnChase = 0;
        }

        Debug.Log("enemiesOnChase = " + enemiesOnChase);

        if (enemiesOnChase == 0)
        {
            float t = fadeTime1;
            as_soundtrack1.mute = false;
            as_soundtrack1.volume = 0.5f;
            StartCoroutine(FadeSountrack(as_soundtrack2, as_soundtrack1, t));
        }
    }

    public void PlayChaseSoundtrack()
    {
        enemiesOnChase++;

        Debug.Log("EnemiesHunting = " + enemiesOnChase);

        if (enemiesOnChase >= 0 && as_soundtrack2.mute)
        {
            float t = fadeTime1;
            as_soundtrack2.mute = false;
            as_soundtrack2.volume = 0.5f;
            StartCoroutine(FadeSountrack(as_soundtrack1, as_soundtrack2, t));
        }
    }

    public void ReturnSounds()
    {
        as_soundtrack1.volume = 1;
        as_gun.volume = 0.5f;
        as_player.volume = 0.5f;
    }

    private IEnumerator EndSoundtrack(AudioSource as_soundtrack1, float t)
    {
        if (!as_soundtrack1.mute)
        {
            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
                as_soundtrack1.volume -= Time.deltaTime / fadeTime2;
                as_gun.volume -= Time.deltaTime / fadeTime2;
                as_player.volume -= Time.deltaTime / fadeTime2;
            }
        }
        yield break;
    }

    private IEnumerator FadeSountrack(AudioSource s1, AudioSource s2, float t)
    {
        if (!s1.mute)
        {

            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
                s2.volume += Time.deltaTime / fadeTime1;
                s1.volume -= Time.deltaTime / fadeTime1;
            }
            //s1.mute = true;
        }
        yield break;
    }


}