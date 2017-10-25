using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public float TurnSpeed = 20f;
    public Transform lineOfSight;
    public Transform line;
    public int currentCharge = 50;
    public int maxCharge = 100;
    public LayerMask gunLayer;
    public bool inControl = false;

    private Transform mTransform;
    private EnemyController enemyControlled;
    private LineRenderer mLineRenderer;
    

	void Start () {
        mTransform = GetComponent<Transform>();
        mLineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update () {
        Debug.DrawRay(transform.GetChild(0).position, (GetComponent<SpriteRenderer>().flipY) ? Vector2.left*100 : Vector2.right*100);
        RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).position,(GetComponent<SpriteRenderer>().flipY)?Vector2.left:Vector2.right, 1000f, gunLayer);
        //Debug.Log(hit.collider.gameObject.name+"    "+target.gameObject.name);
        if (hit.collider != null) {
            mLineRenderer.SetPosition(1, hit.transform.position);
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
            }

            if (Input.GetButtonDown("Fire1") && currentCharge < maxCharge) {

                //Nummero di collisioni che il fucile percepisce
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
