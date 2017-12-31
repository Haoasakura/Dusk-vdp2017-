using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    public float switchTime = 1.5f;
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
    public GameObject[] mechanisms;

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
        Quaternion initRot= Quaternion.Euler(0f, 0f, 73.4f); ;
        if (gun.GetComponentInParent<Player>() != null) {
            shooter = gun.GetComponentInParent<Player>().transform;
            pointOfOrigin = gun.GetComponent<GunController>().barrel;
        }
        else {
            shooter = gun.GetComponentInParent<Enemy>().transform;
            pointOfOrigin = gun.GetComponent<EnemyWeapon>().barrel;
            initRot = gun.transform.rotation;
        }
        changingStatus = true;
        int seconds = (int)switchTime;

        yield return new WaitForSeconds(switchTime);

        changingStatus = false;
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
            gun.parent.rotation = Quaternion.Euler(0f, 0f, shooter.transform.localScale.x * 73.4f); ;

        }
        shooter = null;
        Activate();
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

        yield return new WaitForSeconds(switchTime);

        changingStatus = false;
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
        Activate();
    }

    IEnumerator FlickeringLightOn()
    {
        float startTime = Time.time;
        while ((Time.time - startTime) < switchTime)
        {
            if (changingStatus && Random.Range (Time.time - startTime, switchTime) > switchTime / flickeringRange)
            {
                spriteRenderer.sprite = lightStates[0];
                spriteRenderer.material = litMaterial;
                lightAttached.enabled = true;
                lightCollider.enabled = true;
                maskAttached.enabled = true;

                yield return new WaitForSeconds(0.05f);
                if (!lightStatus)
                {
                    spriteRenderer.sprite = lightStates[1];
                    spriteRenderer.material = unlitMaterial;
                    lightAttached.enabled = false;
                    lightCollider.enabled = false;
                    maskAttached.enabled = false;
                }
            }
            yield return null;
        }
    }

    IEnumerator FlickeringLightOff()
    {
        float startTime = Time.time;
        while ((Time.time - startTime) < switchTime)
        {
            if (changingStatus && Random.Range(Time.time - startTime, switchTime) > switchTime / flickeringRange)
            {
                spriteRenderer.sprite = lightStates[1];
                spriteRenderer.material = unlitMaterial;
                lightAttached.enabled = false;
                lightCollider.enabled = false;
                maskAttached.enabled = false;

                yield return new WaitForSeconds(0.05f);
                if (lightStatus)
                {
                    spriteRenderer.sprite = lightStates[0];
                    spriteRenderer.material = litMaterial;
                    lightAttached.enabled = true;
                    lightCollider.enabled = true;
                    maskAttached.enabled = true;
                }
            }
            yield return null;
        }
    }

    private void Activate()
    {
        foreach (GameObject mechanism in mechanisms)
        {
            if (mechanism.GetComponent<ElevatorMovement>() != null)
            {
                mechanism.GetComponent<ElevatorMovement>().ChangeDestination();
            }
            else if (mechanism.GetComponent<BarrierController>() != null)
            {
                mechanism.GetComponent<BarrierController>().ChangeDestination();
            }
            else if (mechanism.GetComponent<TrapdoorController>() != null)
            {
                mechanism.GetComponent<TrapdoorController>().Activate();
            }
            else if (mechanism.GetComponent<DoorController>() != null)
            {
                mechanism.GetComponent<DoorController>().Activate();
            }
            else if (mechanism.GetComponent<CrusherController>() != null)
            {
                mechanism.GetComponent<CrusherController>().Activate();
            }
            else if (mechanism.GetComponent<LaserController>() != null)
            {
                mechanism.GetComponent<LaserController>().Activate();
            }
            else if (mechanism.GetComponent<PlatformController>() != null)
            {
                mechanism.GetComponent<PlatformController>().Activate();
            }
            else if (mechanism.GetComponent<SpawnEnemyOnEvent>() != null)
            {
                mechanism.GetComponent<SpawnEnemyOnEvent>().Spawn();
            }
        }

    }
}