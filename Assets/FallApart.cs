using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallApart : MonoBehaviour {

    public float delay = 0f;
    public AudioClip ac_fallApart;

    void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(ac_fallApart);
    }
}
