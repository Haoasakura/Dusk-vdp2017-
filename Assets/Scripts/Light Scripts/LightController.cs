using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    public float switchTime = 3f;
    public float flickeringRange = 1.5f; 
    public int lightCharge = 25;
    public bool lightStatus = true;
    public bool changingStatus = false;

    public SpriteRenderer lightAttached;
    public Collider2D lightCollider;
    public SpriteMask maskAttached;
    public Material litMaterial;
    public Material unlitMaterial;
    public Sprite[] lightStates;

    private SpriteRenderer spriteRenderer;


    public Transform shooter=null;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (changingStatus)
        {
            if (!Input.GetButton("Fire1") && ((shooter!=null && shooter.CompareTag(Tags.player)) || (shooter != null && shooter.CompareTag(Tags.enemy) && shooter.GetComponent<EnemyController>().controlled)))
            {
                StopCoroutine("SwitchingOn");
                StopCoroutine("SwitchingOff");
                StopCoroutine("FlickeringLightOn");
                StopCoroutine("FlickeringLightOff");
                changingStatus = false;
                shooter = null;
                if (lightStatus)
                {
                    spriteRenderer.sprite = lightStates[0];
                    lightAttached.enabled = true;
                    lightCollider.enabled = true;
                    maskAttached.enabled = true;
                }
                else
                {
                    spriteRenderer.sprite = lightStates[1];
                    lightAttached.enabled = false;
                    lightCollider.enabled = false;
                    maskAttached.enabled = false;
                }
            }
            
        }
    }

    public void SwitchOnOff(Transform gun)
    {
        if (!lightStatus)
        {
            StartCoroutine("SwitchingOn", gun);
            StartCoroutine("FlickeringLightOn");
        }
        else
        {
            StartCoroutine("SwitchingOff", gun);
            StartCoroutine("FlickeringLightOff");
        }
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
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.sprite = lightStates[0];
        spriteRenderer.material = litMaterial;
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
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.sprite = lightStates[1];
        spriteRenderer.material = unlitMaterial;
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

    IEnumerator FlickeringLightOn()
    {
        float startTime = Time.time;
        while ((Time.time - startTime) < switchTime)
        {
            if (Random.Range (Time.time - startTime, switchTime) > switchTime / flickeringRange)
            {
                spriteRenderer.sprite = lightStates[0];
                spriteRenderer.material = litMaterial;
                lightAttached.enabled = true;
                lightCollider.enabled = true;
                maskAttached.enabled = true;
            }
            else
            {
                spriteRenderer.sprite = lightStates[1];
                spriteRenderer.material = unlitMaterial;
                lightAttached.enabled = false;
                lightCollider.enabled = false;
                maskAttached.enabled = false;
            }
            yield return null;
        }
    }

    IEnumerator FlickeringLightOff()
    {
        float startTime = Time.time;
        while ((Time.time - startTime) < switchTime)
        {
            if (Random.Range(Time.time - startTime, switchTime) > switchTime / flickeringRange)
            {
                spriteRenderer.sprite = lightStates[1];
                spriteRenderer.material = unlitMaterial;
                lightAttached.enabled = false;
                lightCollider.enabled = false;
                maskAttached.enabled = false;
            }
            else
            {
                spriteRenderer.sprite = lightStates[0];
                spriteRenderer.material = litMaterial;
                lightAttached.enabled = true;
                lightCollider.enabled = true;
                maskAttached.enabled = true;
            }
            yield return null;
        }
    }
}