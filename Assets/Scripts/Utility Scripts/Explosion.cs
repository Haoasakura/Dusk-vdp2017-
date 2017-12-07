using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    public float delay = 0f;
    public AudioClip ac_explosion;

    // Use this for initialization
    void Start()
    {
        this.GetComponent<AudioSource>().PlayOneShot(ac_explosion);
        Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }
}
