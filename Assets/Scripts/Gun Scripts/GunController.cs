using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public float TurnSpeed = 20f;
    public int currentCharge = 50;
    public int maxCharge = 100;
    public bool inControl = false;
    public Transform barrel;
    public Transform laserDirection;
    public Transform line;
    public LayerMask gunLayer;


    private Transform mTarget;
    private Transform mTransform;
    private EnemyController enemyControlled;
    private LineRenderer mLineRenderer;
    

	void Start () {
        mTransform = GetComponent<Transform>();
        mLineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update () {
        RaycastHit2D hit = Physics2D.Raycast(barrel.position,laserDirection.position, 10, gunLayer);
        if (hit.collider != null) {
            line.GetComponent<LineRenderer>().SetPosition(1, hit.point);
            mTarget = hit.transform;
        }
        else {
            line.GetComponent<LineRenderer>().SetPosition(1, laserDirection.position);
            mTarget = null;
        }

        if (!inControl) {
            //rotazione della pistola
            if (Input.GetButton("Vertical")) {

                float mVertical = Input.GetAxis("Vertical");

                mTransform.Rotate(new Vector3(0f, 0f, mVertical * TurnSpeed * Time.deltaTime));

                //mantiene la sprite dell'arma nel verso giusto
                if (mTransform.rotation.eulerAngles.z % 270 < 90 && mTransform.rotation.eulerAngles.z % 270 > 0) {
                    GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    GetComponent<SpriteRenderer>().flipY = true;
                }
                line.GetComponent<LineRenderer>().SetPosition(0, barrel.position );
            }

            if (Input.GetButtonDown("Fire1") && currentCharge < maxCharge) {

                /*//Nummero di collisioni che il fucile percepisce
                Collider2D[] pointsOfContact = new Collider2D[25];
                ContactFilter2D contactFilter = new ContactFilter2D();
                contactFilter.useTriggers = true;
                int numberOfContacts = lineOfSight.GetComponent<PolygonCollider2D>().OverlapCollider(contactFilter, pointsOfContact);

                for (int i = 0; i < numberOfContacts; i++) {
                    if (pointsOfContact[i].gameObject != null && pointsOfContact[i].gameObject.CompareTag(Tags.light)) {
                        LightController currentLight = pointsOfContact[i].GetComponent<LightController>();
                        if (InLineOfSight(pointsOfContact[i]) && !currentLight.changingStatus)
                            if(currentCharge>=currentLight.lightCharge || currentLight.lightStatus)
                                pointsOfContact[i].GetComponent<LightController>().SwitchOnOff(transform);
                        break;
                    }
                    else if (pointsOfContact[i].gameObject != null && pointsOfContact[i].gameObject.CompareTag(Tags.machinery)) {
                        MachineryController currentMachinery = pointsOfContact[i].GetComponent<MachineryController>();
                        if (InLineOfSight(pointsOfContact[i]) && !currentMachinery.changingStatus)
                            if(currentCharge>=currentMachinery.powerCharge || currentMachinery.powered)
                                currentMachinery.SwitchOnOff(transform);
                        break;
                    }
                    else if (pointsOfContact[i].gameObject != null && pointsOfContact[i].gameObject.CompareTag(Tags.enemy)) {
                        enemyControlled = pointsOfContact[i].GetComponent<EnemyController>();
                        if (InLineOfSight(pointsOfContact[i]) && currentCharge>=enemyControlled.controlCost)
                            enemyControlled.ControlledOnOff(transform);
                        break;
                    }
                }*/
                if (mTarget != null) {
                    if (mTarget.CompareTag(Tags.light)) {
                        LightController currentLight = mTarget.GetComponent<LightController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentLight.changingStatus)
                            if (currentCharge >= currentLight.lightCharge || currentLight.lightStatus)
                                mTarget.GetComponent<LightController>().SwitchOnOff(transform);
                    }
                    else if (mTarget.CompareTag(Tags.machinery)) {
                        MachineryController currentMachinery = mTarget.GetComponent<MachineryController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentMachinery.changingStatus)
                            if (currentCharge >= currentMachinery.powerCharge || currentMachinery.powered)
                                currentMachinery.SwitchOnOff(transform);
                    }
                    else if (mTarget.CompareTag(Tags.enemy)) {
                        enemyControlled = mTarget.GetComponent<EnemyController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && currentCharge >= enemyControlled.controlCost)
                            enemyControlled.ControlledOnOff(transform);
                    }
                }
            }
        }
	}

    public bool InLineOfSight(Collider2D target) {
        if (target != null) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.transform.position - transform.position),1000f,gunLayer);
            //Debug.Log(hit.collider.gameObject.name+"    "+target.gameObject.name);
            if (hit.collider != null)
                if (hit.collider.gameObject.name == target.gameObject.name) 
                    return true;
        }
        return false;
    }


}
