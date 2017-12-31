using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineryController : MonoBehaviour {

    [Header("GameObject associated with linked mechanism")]
    [SerializeField]
    private GameObject[] mechanisms;

    public float switchTime = 3f;
    public int powerCharge = 25;
    public bool powered = false;
    public bool changingStatus = false;
    public float flickeringRange;
    public Sprite[] sprites = new Sprite[2];
    public Transform shooter = null;
    public Sprite[] lampSprites;
    public SpriteRenderer Lamp;

    private SpriteRenderer spriteRenderer;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (powered == false)
        {
            spriteRenderer.sprite = sprites[1];
            Lamp.sprite = lampSprites[1];
        }
        else
        {
            spriteRenderer.sprite = sprites[0];
            Lamp.sprite = lampSprites[0];
        }
    }
	
	void Update () {
        if (changingStatus) {
            if (!Input.GetButton("Fire1") && (shooter != null && shooter.CompareTag(Tags.player))) {
                StopCoroutine("SwitchingOn");
                StopCoroutine("SwitchingOff");
                StopCoroutine("FlickeringLightOn");
                StopCoroutine("FlickeringLightOff");
                changingStatus = false;
                shooter = null;
                if (powered)
                {
                    spriteRenderer.sprite = sprites[0];
                    Lamp.sprite = lampSprites[0];
                }
                else
                {
                    spriteRenderer.sprite = sprites[1];
                    Lamp.sprite = lampSprites[1];
                }
            }
        }
    }

    public void SwitchOnOff(Transform gun) {
        if (!powered)
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

    public void InstantSwitchOn()
    {
        spriteRenderer.sprite = sprites[0];
        Lamp.sprite = lampSprites[0];
        powered = true;
        Activate();
    }

    IEnumerator SwitchingOn(Transform gun) {
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
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.sprite = sprites[0];
        Lamp.sprite = lampSprites[0];
        powered = true;
        if (shooter.GetComponent<Player>() != null) {
            gun.GetComponent<GunController>().currentCharge -= powerCharge;
        }
        else {
            gun.GetComponent<EnemyWeapon>().currentCharge -= powerCharge;
            shooter.GetComponent<EnemyController>().shootingLights = false;
        }
        GetComponent<ObjectSoundManager>().PlaySound();
        changingStatus = false;
        Activate();
    }

    IEnumerator SwitchingOff(Transform gun) {
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
        while (seconds > 0) {
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        spriteRenderer.sprite = sprites[1];
        Lamp.sprite = lampSprites[1];
        powered = false;
        if (shooter.GetComponent<Player>() != null)
        {
            gun.GetComponent<GunController>().currentCharge += powerCharge;
        }
        if (shooter.GetComponent<EnemyController>() != null)
        {
            gun.GetComponent<EnemyWeapon>().currentCharge += powerCharge;
            shooter.GetComponent<EnemyController>().shootingLights = false;
        }

        changingStatus = false;
        Activate();
    }

    IEnumerator FlickeringLightOn()
    {
        float startTime = Time.time;
        while ((Time.time - startTime) < switchTime)
        {
            if (Random.Range(Time.time - startTime, switchTime) > switchTime / flickeringRange)
            {
                spriteRenderer.sprite = sprites[0];
                Lamp.sprite = lampSprites[0];
            }
            else
            {
                spriteRenderer.sprite = sprites[1];
                Lamp.sprite = lampSprites[1];
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
                spriteRenderer.sprite = sprites[1];
                Lamp.sprite = lampSprites[1];
            }
            else
            {
                spriteRenderer.sprite = sprites[0];
                Lamp.sprite = lampSprites[0];
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
