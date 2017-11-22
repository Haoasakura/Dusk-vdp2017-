using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRandom : MonoBehaviour
{

    public float range;
    public float maxLight;
    public float minLight;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Light l = GetComponent<Light>();
        if (l.intensity > maxLight)
        {
            if (Random.Range(0, range) < 0.5)
            {
                l.intensity -= 0.1f;
            }

        }
        else if (l.intensity < minLight)
        {
            if (Random.Range(0, range) < 0.5)
            {
                l.intensity += 0.1f;
            }
        }
        else
        {
            if (Random.Range(0, range) < range / 2)
            {
                l.intensity += 0.1f;
            }
            else
            {
                l.intensity -= 0.1f;
            }
        }

    }
}