using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiscovered : MonoBehaviour {

    public float delay = 0f;
    public AudioClip ac_alert;
    
    void Start() {
        transform.GetComponent<AudioSource>().PlayOneShot(ac_alert);
        Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }
}
