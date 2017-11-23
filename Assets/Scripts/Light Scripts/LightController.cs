using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    public float switchTime = 3f;
    public int lightCharge = 25;
    public bool lightStatus = true;
    public bool changingStatus = false;

    public SpriteRenderer lightAttached;
    public Collider2D lightCollider;
    public SpriteMask maskAttached;

    public Sprite[] lightStates;
    public GameObject absorptionEffect;

    private SpriteRenderer spriteRenderer;

    private GameObject particleEffect;
    public Transform shooter=null;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (changingStatus)
        {
            if (!Input.GetButton("Fire1") && (shooter!=null && shooter.CompareTag(Tags.player)))
            {
                StopCoroutine("SwitchingOn");
                StopCoroutine("SwitchingOff");
                StopCoroutine("TrailingEffectOn");
                StopCoroutine("TrailingEffectOff");
                Destroy(particleEffect);
                changingStatus = false;
                shooter = null;
            }
        }
    }

    public void SwitchOnOff(Transform gun)
    {
        if (!lightStatus)
            StartCoroutine("SwitchingOn", gun);
        else
            StartCoroutine("SwitchingOff", gun);
    }

    IEnumerator SwitchingOn(Transform gun)
    {
        Transform pointOfOrigin = null;
        if (gun.GetComponentInParent<Player>() != null) {
            shooter = gun.GetComponentInParent<Player>().transform;
            pointOfOrigin = gun.GetComponent<GunController>().barrel;
        }
        else {
            shooter = gun.GetComponentInParent<Enemy>().transform;
            pointOfOrigin = gun.GetComponent<EnemyWeapon>().barrel;
        }
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOn", pointOfOrigin);
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOn");
        Destroy(particleEffect);
        spriteRenderer.sprite = lightStates[0];
        lightAttached.enabled = true;
        lightCollider.enabled = true;
        maskAttached.enabled = true;
        lightStatus = true;
        if (shooter.GetComponent<Player>() != null) {
            gun.GetComponent<GunController>().currentCharge -= lightCharge;
        }
        else {
            gun.GetComponent<EnemyWeapon>().currentCharge -= lightCharge;
            shooter.GetComponent<EnemyController>().shootingLights = false;
        }
        changingStatus = false;
        shooter = null;
    }

    IEnumerator SwitchingOff(Transform gun)
    {
        Transform pointOfOrigin = null;
        if (gun.GetComponentInParent<Player>() != null) {
            shooter = gun.GetComponentInParent<Player>().transform;
            pointOfOrigin = gun.GetComponent<GunController>().barrel;
        }
        else {
            shooter = gun.GetComponentInParent<Enemy>().transform;
            pointOfOrigin = gun.GetComponent<EnemyWeapon>().barrel;
        }
        changingStatus = true;
        int seconds = (int)switchTime;
        StartCoroutine("TrailingEffectOff", pointOfOrigin);
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StopCoroutine("TrailingEffectOff");
        Destroy(particleEffect);
        spriteRenderer.sprite = lightStates[1];
        lightAttached.enabled = false;
        lightCollider.enabled = false;
        maskAttached.enabled = false;
        lightStatus = false;
        if (shooter.GetComponent<Player>() != null) {
            gun.GetComponent<GunController>().currentCharge += lightCharge;
        }
        else {
            gun.GetComponent<EnemyWeapon>().currentCharge += lightCharge;
        }
        changingStatus = false;
    }

    IEnumerator TrailingEffectOn(Transform gun)
    {
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true)
        {
            particleEffect.transform.position = Vector3.Lerp(gun.position, transform.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }

    IEnumerator TrailingEffectOff(Transform gun)
    {
        float startTime = Time.time;
        particleEffect = Instantiate(absorptionEffect, transform.position, transform.rotation) as GameObject;
        while (true)
        {
            particleEffect.transform.position = Vector3.Lerp(transform.position, gun.position, Mathf.SmoothStep(0, 1, (Time.time - startTime) / switchTime));
            yield return null;
        }
    }
}