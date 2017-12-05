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

    private SpriteRenderer spriteRenderer;
    public Transform shooter = null;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (powered == false)
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.green;
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
                    spriteRenderer.color = Color.green;
                }
                else
                {
                    spriteRenderer.color = Color.red;
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
        spriteRenderer.color = Color.green;
        powered = true;
        if (shooter.GetComponent<Player>() != null) {
            gun.GetComponent<GunController>().currentCharge -= powerCharge;
        }
        else {
            gun.GetComponent<EnemyWeapon>().currentCharge -= powerCharge;
            shooter.GetComponent<EnemyController>().shootingLights = false;
        }
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
        spriteRenderer.color = Color.red;
        powered = false;
        if (shooter.GetComponent<EnemyController>() != null) {
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
                spriteRenderer.color = Color.green;
            }
            else
            {
                spriteRenderer.color = Color.red;

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
                spriteRenderer.color = Color.red;

            }
            else
            {
                spriteRenderer.color = Color.green;
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
        }
    }
}
