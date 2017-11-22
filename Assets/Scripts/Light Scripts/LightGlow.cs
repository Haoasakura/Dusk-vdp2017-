using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGlow : MonoBehaviour {

    public float maxLight;
    public float minLight;
    [Range(0f, 0.1f)]
    public float velocity;

    private bool isDim = true;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Light l = GetComponent<Light>();
        if (l.intensity > (minLight - 1) && isDim == true)
        {
            l.intensity -= velocity;
        }
        else if (l.intensity < (maxLight + 1) && isDim == false)
        {
            l.intensity += velocity;
        }

        if (l.intensity < minLight)
        {
            isDim = false;
        }

        if (l.intensity > maxLight)
        {
            isDim = true;
        }

    }

}
