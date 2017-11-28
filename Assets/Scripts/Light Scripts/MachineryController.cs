using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineryController : MonoBehaviour {

    [Header("GameObject associated with linked mechanism")]
    [SerializeField]
    private GameObject mechanism;

    public float switchTime = 3f;
    public int powerCharge = 25;
    public bool powered = false;
    public bool changingStatus = false;

    private SpriteRenderer spriteRenderer;
    public Transform shooter = null;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        if (changingStatus) {
            if (!Input.GetButton("Fire1") && (shooter != null && shooter.CompareTag(Tags.player))) {
                StopCoroutine("SwitchingOn");
                StopCoroutine("SwitchingOff");
                changingStatus = false;
                shooter = null;
            }
        }
    }

    public void SwitchOnOff(Transform gun) {
        if (!powered)
            StartCoroutine("SwitchingOn", gun);
        else
            StartCoroutine("SwitchingOff", gun);
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
        spriteRenderer.color = Color.red;
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
        spriteRenderer.color = Color.white;
        powered = false;
        if (shooter.GetComponent<EnemyController>() != null) {
            shooter.GetComponent<EnemyController>().shootingLights = false;
        }
        changingStatus = false;
        Activate();
    }

    private void Activate()
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
