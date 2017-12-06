using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectSoundManager : MonoBehaviour {

    public AudioSource as_object;
    public AudioClip ac_clip;

    private float lowPitchRange = .95f;
    private float highPitchRange = 1.05f;

    public void PlaySound(float pitch)
    {
        as_object.pitch = pitch;
        as_object.PlayOneShot(ac_clip);
    }

    public void PlaySound()
    {
        as_object.pitch = Random.Range(lowPitchRange, highPitchRange);
        as_object.PlayOneShot(ac_clip);
    }
}
